using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [MBTNode(name = "Is Variable Set")]
    public class IsVariableSet : Decorator
    {
        public TransformReference transformReference;

        public override NodeResult Execute()
        {
            Node node = GetChild();
            if (node == null) {
                return new NodeResult(Status.Failure);
            }
            if (node.status == Status.Success || node.status == Status.Failure) {
                return new NodeResult(node.status);
            }
            return new NodeResult(Status.Running, node);
        }
    }
}
