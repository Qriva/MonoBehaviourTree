using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [MBTNode(name = "Repeater")]
    public class Repeater : Decorator
    {
        public int loops = 1;
        public bool infinite = false;
        private int count;
        
        public override void OnEnter()
        {
            count = loops;
        }

        public override NodeResult Execute()
        {
            Node node = GetChild();
            if(node == null) {
                return new NodeResult(Status.Failure);
            }
            // TODO: Check if this node works correctly
            if (infinite || count > 0) {
                // Repeat children
                behaviourTree.ResetNodesTo(this);
                count -= 1;
                return new NodeResult(Status.Running, node);
            }
            return new NodeResult(Status.Success);
        }
    }
}
