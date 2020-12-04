using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [AddComponentMenu("")]
    [MBTNode(name = "SubTree", order = 250)]
    public class SubTree : Node, IChildrenNode
    {
        public MonoBehaviourTree tree;
        
        public override void AddChild(Node node)
        {
            return;
        }

        public override NodeResult Execute()
        {
            // Return fialure when subtree is not defined
            if (tree == null) {
                return NodeResult.failure;
            }
            Node root = tree.GetRoot();
            if (root.status == Status.Success || root.status == Status.Failure) {
                return new NodeResult(root.status);
            }
            return new NodeResult(Status.Running, root);
        }

        public override void RemoveChild(Node node)
        {
            return;
        }

        public override bool IsValid()
        {
            return tree != null;
        }
    }
}
