using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [AddComponentMenu("")]
    [MBTNode("Decorators/Inverter")]
    public class Inverter : Decorator
    {
        public override NodeResult Execute()
        {
            if (!TryGetChild(out Node node))
            {
                return NodeResult.failure;
            }
            if (node.status == Status.Success) {
                return NodeResult.failure;
            } else if (node.status == Status.Failure) {
                return NodeResult.success;
            }
            return node.runningNodeResult;
        }
    }
}
