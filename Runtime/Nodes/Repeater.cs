using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [AddComponentMenu("")]
    [MBTNode(name = "Decorators/Repeater")]
    public class Repeater : Decorator
    {
        public int loops = 1;
        public bool infinite = false;
        public bool breakOnFailure = false;
        private int count;
        
        public override void OnEnter()
        {
            count = loops;
        }

        public override NodeResult Execute()
        {
            if (!TryGetChild(out Node node))
            {
                return NodeResult.failure;
            }
            if (breakOnFailure && node.status == Status.Failure)
            {
                return NodeResult.failure;
            }
            if (infinite || count > 0) {
                // Repeat children
                behaviourTree.ResetNodesTo(this);
                count -= 1;
                return node.runningNodeResult;
            }
            return NodeResult.success;
        }
    }
}
