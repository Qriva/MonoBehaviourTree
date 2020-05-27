using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonoBT
{
    [MBTNode("Succeeder")]
    public class Succeeder : Decorator
    {
        public override NodeResult Execute()
        {
            Node node = GetChild();
            if (node != null && (node.status == Status.Success || node.status == Status.Failure)) {
                return new NodeResult(Status.Success);
            }
            return new NodeResult(Status.Running, node);
        }
    }
}
