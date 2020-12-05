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
                return NodeResult.From(root.status);
            }
            return root.runningNodeResult;
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
