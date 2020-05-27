using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonoBT
{
    public abstract class Decorator : Node, IParentNode, IChildrenNode
    {
        private Node[] stackState = new Node[0];

        public override void AddChild(Node node)
        {
            // Allow only one children
            if (this.children.Count > 0)
            {
                Node child = this.children[0];
                if (child == node) {
                    return;
                }
                child.parent.RemoveChild(child);
                this.children.Clear();
            }
            // Remove parent in case there is one already
            if (node.parent != null) {
                node.parent.RemoveChild(node);
            }
            this.children.Add(node);
            node.parent = this;
        }

        protected Node GetChild()
        {
            if (children.Count > 0) {
                return children[0];
            }
            return null;
        }

        public override void RemoveChild(Node node)
        {
            if (children.Contains(node))
            {
                children.Remove(node);
                node.parent = null;
            }
        }

        protected void StoreBTState()
        {
            stackState = behaviourTree.GetStack();
        }

        protected void DisposeBTState()
        {
            stackState = new Node[0];
        }

        internal Node[] GetStoredBTState()
        {
            return stackState;
        }

        /// <summary>
        /// Helper method used to abort nodes in valid case
        /// </summary>
        /// <param name="abort">Abort type</param>
        protected void TryAbort(Abort abort)
        {
            switch (abort)
            {
                case Abort.Self:
                    if (status == Status.Running) {
                        behaviourTree.Interrupt(this);
                    }
                    break;
                case Abort.LowerPriority:
                    if (status == Status.Success || status == Status.Failure) {
                        behaviourTree.Interrupt(this);
                    }
                    break;
                case Abort.Both:
                    behaviourTree.Interrupt(this);
                    break;
            }
        }
    }
}
