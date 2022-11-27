using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Profiling;
using System.Linq;

namespace MBT
{
    [DisallowMultipleComponent]
    // [RequireComponent(typeof(Blackboard))]
    public class MonoBehaviourTree : MonoBehaviour
    {
        private static readonly ProfilerMarker _TickMarker = new ProfilerMarker("MonoBehaviourTree.Tick");

        [HideInInspector]
        public Node selectedEditorNode;
        public bool repeatOnFinish = false;
        public int maxExecutionsPerTick = 1000;
        public MonoBehaviourTree parent;
        
        /// <summary>
        /// Event triggered when tree is about to be updated
        /// </summary>
        public event UnityAction onTick = delegate {};
        private Root rootNode;
        private List<Node> executionNodes;
        private List<Node> executionLog;
        private List<Decorator> interruptingNodes = new List<Decorator>();
        
        void Awake()
        {   
            rootNode = GetComponent<Root>();
            if (rootNode == null) {
                Debug.LogWarning("Missing Root node in behaviour tree.", this);
            }
            
            // Find master parent tree and all nodes
            MonoBehaviourTree masterTree = this.GetMasterTree();
            Node[] nodes = GetComponents<Node>();
            if(masterTree == this)
            {
                // Create lists with capicity
                executionNodes = new List<Node>(8);
                executionLog = new List<Node>(nodes.Length);
                // Set start node when tree is created first time
                executionNodes.Add(rootNode);
                executionLog.Add(rootNode);
            }
            // Initialize nodes of tree/subtree
            for (int i = 0; i < nodes.Length; i++)
            {
                Node n = nodes[i];
                n.behaviourTree = masterTree;
                n.runningNodeResult = new NodeResult(Status.Running, n);
            }
        }

        private void EvaluateInterruptions()
        {
            if (interruptingNodes.Count == 0) {
                return;
            }

            // Find node with highest priority - closest to the root (the smallest number)
            Decorator abortingNode = interruptingNodes[0];
            for (int i = 1; i < interruptingNodes.Count; i++)
            {
                Decorator d = interruptingNodes[i];
                if (d.runtimePriority < abortingNode.runtimePriority) {
                    abortingNode = d;
                }
            }

            // Revert stack
            executionNodes.Clear();
            executionNodes.AddRange(abortingNode.GetStoredTreeSnapshot());
            // Restore flow of events in nodes after abort
            for (int i = 0; i < executionNodes.Count; i++)
            {
                Node node = executionNodes[i];
                if (node.status == Status.Running)
                {
                    // This node is still running and might need to restore the state
                    node.OnBehaviourTreeAbort();
                }
                else if (node.status == Status.Success || node.status == Status.Failure)
                {
                    // This node returned failure or success, so reenter it and call OnEnter
                    node.OnEnter();
                }
                // All nodes in execution stack should be in running state
                node.status = Status.Running;
            }
            
            int nodeIndex = abortingNode.runtimePriority - 1;
            // Sanity check
            if (abortingNode != executionLog[nodeIndex]) {
                Debug.LogWarning("Priority of node does not match with exectuion log");
            }
            // Abort nodes in log
            ResetNodesTo(abortingNode, true);
            // Reset aborting node
            abortingNode.status = Status.Ready;
            // Reset list and wait for new interruptions
            interruptingNodes.Clear();
        }

        /// <summary>
        /// Update tree state.
        /// </summary>
        public void Tick()
        {
            _TickMarker.Begin();
            // Fire Tick event
            onTick.Invoke();
            
            // Check if there are any interrupting nodes
            EvaluateInterruptions();

            // Max number of traversed nodes
            int executionLimit = maxExecutionsPerTick;
            List<Node> children = new List<Node>();
            // Traverse tree
            for (int i = 0; i< executionNodes.Count;)
            {
                Node currentNode = executionNodes[i];
                if (executionLimit == 0) {
                    _TickMarker.End();
                    return;
                }
                executionLimit -= 1;

                // Execute last element in stack
                NodeResult nodeResult = currentNode.Execute();
                // Set new status
                currentNode.status = nodeResult.status;
                children.Clear();
                if (nodeResult.status == Status.Running) {
                    // If node is running, then stop execution or continue children
                    if (currentNode is Parallel)
                    {
                        var readyChildren = currentNode.children.Where((child) =>
                        {
                            return child.status == Status.Ready;
                        });
                        children.AddRange(readyChildren);
                    }
                    else if (nodeResult.child != null)
                    {
                        if (nodeResult.child.status == Status.Ready)
                        {
                            children.Add(nodeResult.child);
                        }
                    }
                    
                    if (children.Count == 0) {
                        if (i == executionNodes.Count - 1)
                        {
                            // Stop execution and continue next tick
                            _TickMarker.End();
                            return;
                        }
                        i++;
                    }
                    else
                    {
                        // remove current
                        executionNodes.RemoveAt(i);
                        for (int j = 0; j < children.Count; j++)
                        {
                            var child = children[j];
                            if (child.status != Status.Running && child.status != Status.Ready)
                            {
                                continue;
                            }
                            // Add child to execution stack and execute it in next loop
                            executionNodes.Insert(i+j, child);
                            executionLog.Add(child);
                            // IMPORTANT: Priority must be > 0 and assigned in this order
                            child.runtimePriority = executionLog.Count;
                            child.OnAllowInterrupt();
                            child.OnEnter();
#if UNITY_EDITOR
                            // Stop execution if breakpoint is set on this node
                            if (child.breakpoint)
                            {
                                Debug.Break();
                                UnityEditor.Selection.activeGameObject = this.gameObject;
                                Debug.Log("MBT Breakpoint: " + child.title, this);
                                _TickMarker.End();
                                return;
                            }
#endif
                        }

                        continue;
                    }
                }
                else
                {
                    if (currentNode is Parallel && nodeResult.status == Status.Failure)
                    {
                        // remove fail children
                        List<Node> dfs = new List<Node>();
                        dfs.AddRange(currentNode.children);
                        while(dfs.Count > 0)
                        {
                            var current = dfs[0];
                            dfs.Remove(current);
                            current.status = Status.Failure;
                            dfs.AddRange(current.children);
                            var executionNodesIndex = executionNodes.IndexOf(current);
                            if (executionNodesIndex != -1)
                            {
                                current.OnExit();
                                executionNodes.RemoveAt(executionNodesIndex);
                                if (executionNodesIndex < i)
                                {
                                    i--;
                                }
                            }
                        }
                    }
                    // Remove last node from stack and move up (closer to root)
                    currentNode.OnExit();
                    executionNodes.RemoveAt(i);
                    if (currentNode.parent != null && currentNode.parent.status == Status.Running)
                    {
                        executionNodes.Insert(i,currentNode.parent);
                    }
                    // i++;
                }
            }
            
            // Run this when execution stack is empty and BT should repeat
            if (repeatOnFinish) {
                Restart();
            }
            _TickMarker.End();
        }

        /// <summary>
        /// This method should be called to abort tree to given node
        /// </summary>
        /// <param name="node">Abort and revert tree to this node</param>
        internal void Interrupt(Decorator node)
        {
            if (!interruptingNodes.Contains(node)) {
                interruptingNodes.Add(node);
            }
        }

        internal void ResetNodesTo(Node node, bool aborted = false)
        {
            int i = executionLog.Count - 1;
            // Reset status and get index of node
            while (i >= 0)
            {
                Node n = executionLog[i];
                if (n == node) {
                    break;
                }
                // If node is running (on exec stack) then call exit
                if (n.status == Status.Running) {
                    n.OnExit();
                    // IMPROVEMENT: Abort event can be added or abort param onExit
                }
                n.status = Status.Ready;
                n.OnDisallowInterrupt();
                i -= 1;
            }
            // Reset log
            i += 1;
            if (i >= executionLog.Count) {
                return;
            }
            executionLog.RemoveRange(i, executionLog.Count - i);
        }

        private void ResetNodes()
        {
            for (int i = 0; i < executionLog.Count; i++)
            {
                Node node = executionLog[i];
                if (node.status == Status.Running)
                {
                    node.OnExit();
                }
                node.OnDisallowInterrupt();
                node.status = Status.Ready;
            }
            executionLog.Clear();
            executionNodes.Clear();
        }

        /// <summary>
        /// Resets state to root node
        /// </summary>
        public void Restart()
        {
            ResetNodes();
            executionNodes.Add(rootNode);
            executionLog.Add(rootNode);
        }

        internal void GetStack(ref Node[] stack)
        {
            // Resize array when size is too small
            if (executionNodes.Count > stack.Length)
            {
                // Node should not change priority and position during runtime
                // It means the array will be resized once during first call of this method
                Array.Resize<Node>(ref stack, executionNodes.Count);
            }
#if UNITY_EDITOR
            // Additional sanity check in case nodes are reordered or changed in editor
            if (stack.Length > executionNodes.Count)
            {
                Debug.LogError("Changing order of MBT nodes during runtime might cause errors or unpredictable results.");
            }
#endif
            // Copy elements to provided array
            executionNodes.CopyTo(stack);
        }

        public Node GetRoot()
        {
            return rootNode;
        }

        public MonoBehaviourTree GetMasterTree()
        {
            if (parent == null)
            {
                return this;
            }
            return parent.GetMasterTree();
        }

        #if UNITY_EDITOR
        void OnValidate()
        {
            if (maxExecutionsPerTick <= 0)
            {
                maxExecutionsPerTick = 1;
            }
            
            if (parent != null)
            {
                if (parent == this)
                {
                    parent = null;
                    Debug.LogWarning("This tree cannot be its own parent.");
                    return;
                }
                if (transform.parent == null || parent.gameObject != transform.parent.gameObject)
                {
                    // parent = null;
                    Debug.LogWarning("Parent tree should be also parent of this gameobject.", this.gameObject);
                }
            }
        }
        #endif
    }
}
