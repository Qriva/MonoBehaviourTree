using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [AddComponentMenu("")]
    [MBTNode(name = "Root", order = 200)]
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
                    return NodeResult.From(child.status);
                }
                // Set last tick to current time because execution just started (this reduces first deltaTime to 0)
                behaviourTree.LastTick = Time.time;
                return child.runningNodeResult;
            }
            return NodeResult.failure;
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
