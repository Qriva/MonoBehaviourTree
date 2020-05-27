using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonoBT
{
    [MBTNode("Inverter")]
    public class Inverter : Decorator
    {
        public override NodeResult Execute()
        {
            Node node = GetChild();
            if (node == null) {
                return new NodeResult(Status.Failure);
            }
            if (node.status == Status.Success) {
                return new NodeResult(Status.Failure);
            } else if (node.status == Status.Failure) {
                return new NodeResult(Status.Success);
            }
            return new NodeResult(Status.Running, node);
        }
    }
}
