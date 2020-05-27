using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{ 
    public abstract class Node : MonoBehaviour
    {
        public static readonly float NODE_DEFAULT_WIDTH = 200f;

        public string title;
        [HideInInspector]
        public Rect rect = new Rect(0, 0, NODE_DEFAULT_WIDTH, 50);
        public Node parent;
        public List<Node> children = new List<Node>();
        [System.NonSerialized]
        public Status status = Status.Ready;
        [System.NonSerialized]
        public MonoBehaviourTree behaviourTree;
        [System.NonSerialized]
        public int runtimePriority = 0;
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

        public void SortChildren()
        {
            this.children.Sort((c, d) => c.rect.x.CompareTo(d.rect.x));
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
    }

    public interface IChildrenNode{
        // void SetParent(Node node);
    }

    public interface IParentNode{
        // void AddChild(Node node);
    }
}
