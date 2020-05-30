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
            Node node = GetChild();
            if (node != null && (node.status == Status.Success || node.status == Status.Failure)) {
                return NodeResult.success;
            }
            return new NodeResult(Status.Running, node);
        }
    }
}
