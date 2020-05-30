using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    public abstract class Composite : Node, IParentNode, IChildrenNode
    {
        public bool random = false;

        public override void AddChild(Node node)
        {
            if (!children.Contains(node))
            {
                // Remove parent in case there is one already
                if (node.parent != null) {
                    node.parent.RemoveChild(node);
                }
                children.Add(node);
                node.parent = this;
            }
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
