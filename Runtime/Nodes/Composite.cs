using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    public abstract class Composite : Node, IParentNode, IChildrenNode
    {
        private static readonly System.Random rng = new System.Random();
        
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

        protected static void ShuffleList<T>(List<T> list)
        {  
            int n = list.Count;  
            while (n > 1) {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
