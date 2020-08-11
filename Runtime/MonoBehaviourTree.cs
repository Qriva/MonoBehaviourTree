using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [DisallowMultipleComponent]
    // [RequireComponent(typeof(Blackboard))]
    public class MonoBehaviourTree : MonoBehaviour
    {
        [HideInInspector]
        public Node selectedEditorNode;
        public string description;
        public bool repeatOnFinish = false;
        public int maxExecutionsPerTick = 1000;
        public MonoBehaviourTree parent;
        
        private Root rootNode;
        private List<Node> executionStack = new List<Node>();
        private List<Node> executionLog = new List<Node>();
        private List<Decorator> interruptingNodes = new List<Decorator>();
        
        void Awake()
        {
            rootNode = GetComponent<Root>();
            if (rootNode == null) {
                Debug.LogWarning("Missing Root node in behaviour tree.", this);
            }
            
            // Find for master parent tree
            MonoBehaviourTree masterTree = this.GetMasterTree();
            if(masterTree == this)
            {
                // Set start node when tree is created first time
                executionStack.Add(rootNode);
                executionLog.Add(rootNode);
            }
            // Initialize nodes of tree/subtree
            Node[] nodes = GetComponents<Node>();
            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i].behaviourTree = masterTree;
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
            executionStack.Clear();
            executionStack.AddRange(abortingNode.GetStoredBTState());
            // Restore flow of events in nodes after abort
            for (int i = 0; i < executionStack.Count; i++)
            {
                Node node = executionStack[i];
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
            // Check if there are any interrupting nodes
            EvaluateInterruptions();

            // Max number of traversed nodes
            int executionLimit = maxExecutionsPerTick;
            // Traverse tree
            while (executionStack.Count > 0)
            {
                if (executionLimit == 0) {
                    return;
                }
                executionLimit -= 1;

                // Execute last element in stack
                Node currentNode = executionStack[executionStack.Count - 1];
                NodeResult nodeResult = currentNode.Execute();
                // Set new status
                currentNode.status = nodeResult.status;
                if (nodeResult.status == Status.Running) {
                    // If node is running, then stop execution or continue children
                    Node child = nodeResult.child;
                    if (child == null) {
                        // Stop execution and continue next tick
                        return;
                    } else {
                        // Add child to execution stack and execute it in next loop
                        executionStack.Add(child);
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
                            return;
                        }
                        #endif
                        continue;
                    }
                } else {
                    // Remove last node from stack and move up (closer to root)
                    currentNode.OnExit();
                    executionStack.RemoveAt(executionStack.Count - 1);
                }
            }
            
            // Run this when execution stack is empty and BT should repeat
            if (repeatOnFinish) {
                ResetNodes();
                executionStack.Add(rootNode);
                executionLog.Add(rootNode);
            }
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
                node.status = Status.Ready;
                node.OnDisallowInterrupt();
            }
            executionLog.Clear();
            executionStack.Clear();
        }

        internal Node[] GetStack()
        {
            return executionStack.ToArray();
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
