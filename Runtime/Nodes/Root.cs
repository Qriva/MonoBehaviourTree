using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonoBT
{
    [MBTNode(name = "Root")]
    public class Root : Node, IParentNode
    {
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

        public override NodeResult Execute()
        {
            if (children.Count == 1) {
                Node child = children[0];
                if (child.status == Status.Success || child.status == Status.Failure) {
                    return new NodeResult(child.status);
                }
                return new NodeResult(Status.Running, child);
            }
            return new NodeResult(Status.Failure);
        }

        public override void RemoveChild(Node node)
        {
            if (children.Contains(node))
            {
                children.Remove(node);
                node.parent = null;
            }
        }
    }
}
