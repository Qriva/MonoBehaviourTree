using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonoBT
{
    public abstract class Leaf : Node, IChildrenNode
    {
        public override void AddChild(Node node)
        {
            return;
        }

        public override void RemoveChild(Node node)
        {
            return;
        }
    }
}
