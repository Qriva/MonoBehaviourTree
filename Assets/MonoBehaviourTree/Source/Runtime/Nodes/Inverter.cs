using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [MBTNode("Inverter")]
    public class Inverter : Decorator
    {
        public override NodeResult Execute()
        {
            Node node = GetChild();
            if (node == null) {
                return NodeResult.failure;
            }
            if (node.status == Status.Success) {
                return NodeResult.failure;
            } else if (node.status == Status.Failure) {
                return NodeResult.success;
            }
            return new NodeResult(Status.Running, node);
        }
    }
}
