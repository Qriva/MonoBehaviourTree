using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [AddComponentMenu("")]
    [MBTNode("Decorators/Succeeder")]
    public class Succeeder : Decorator
    {
        public override NodeResult Execute()
        {
            if (!TryGetChild(out Node node))
            {
                return NodeResult.failure;
            }
            if (node.status == Status.Success || node.status == Status.Failure)
            {
                return NodeResult.success;
            }
            return node.runningNodeResult;
        }
    }
}
