using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{ 
    public abstract class Node : MonoBehaviour
    {
        public const float NODE_DEFAULT_WIDTH = 160f;

        public string title;
        [HideInInspector]
        public Rect rect = new Rect(0, 0, NODE_DEFAULT_WIDTH, 50);
        [HideInInspector]
        public Node parent;
        [HideInInspector]
        public List<Node> children = new List<Node>();
        [System.NonSerialized]
        public Status status = Status.Ready;
        [HideInInspector]
        public MonoBehaviourTree behaviourTree;
        [HideInInspector]
        public int runtimePriority = 0;
        [HideInInspector]
        public bool breakpoint = false;
        private bool _selected = false;
        public bool selected
        {
            get { return _selected; }
            set { _selected = value; }
        }
        
        public virtual void OnAllowInterrupt() {}
        public virtual void OnEnter() {}
        public abstract NodeResult Execute();
        public virtual void OnExit() {}
        public virtual void OnDisallowInterrupt() {}

        public virtual void OnBehaviourTreeAbort() {}

        public abstract void AddChild(Node node);
        public abstract void RemoveChild(Node node);

        public virtual Node GetParent()
        {
            return parent;
        }

        public virtual List<Node> GetChildren()
        {
            return children;
        }

        public bool IsDescendantOf(Node node)
        {
            if (this.parent == null) {
                return false;
            } else if (this.parent == node) {
                return true;
            }
            return this.parent.IsDescendantOf(node);
        }

        public List<Node> GetAllSuccessors()
        {
            List<Node> result = new List<Node>();
            for (int i = 0; i < children.Count; i++)
            {
                result.Add(children[i]);
                result.AddRange(children[i].GetAllSuccessors());
            }
            return result;
        }

        public void SortChildren()
        {
            this.children.Sort((c, d) => c.rect.x.CompareTo(d.rect.x));
        }

        /// <summary>
        /// Check if node setup is valid
        /// </summary>
        /// <returns>Returns true if node is configured correctly</returns>
        public virtual bool IsValid()
        {
            #if UNITY_EDITOR
            System.Reflection.FieldInfo[] propertyInfos = this.GetType().GetFields();
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                if (propertyInfos[i].FieldType.IsSubclassOf(typeof(BaseVariableReference)))
                {
                    BaseVariableReference varReference = propertyInfos[i].GetValue(this) as BaseVariableReference;
                    if (varReference != null && varReference.isInvalid)
                    {
                        return false;
                    }
                }
            }
            #endif
            return true;
        }
    }

    public enum Status
    {
        Success, Failure, Running, Ready
    }

    public enum Abort
    {
        None, Self, LowerPriority, Both
    }

    public class NodeResult
    {
        public Status status {get; private set;}
        public Node child {get; private set;}

        public NodeResult(Status status, Node child = null)
        {
            this.status = status;
            this.child = child;
        }

        public static readonly NodeResult success = new NodeResult(Status.Success);
        public static readonly NodeResult failure = new NodeResult(Status.Failure);
        public static readonly NodeResult running = new NodeResult(Status.Running);
    }

    public interface IChildrenNode{
        // void SetParent(Node node);
    }

    public interface IParentNode{
        // void AddChild(Node node);
    }
}
