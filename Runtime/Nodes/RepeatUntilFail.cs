using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [MBTNode("Repeat Until Fail")]
    public class RepeatUntilFail : Decorator
    {
        public override NodeResult Execute()
        {
            Node node = GetChild();
            if (node.status == Status.Failure) {
                return new NodeResult(Status.Success);
            }
            // Repeat children
            behaviourTree.ResetNodesTo(this);
            return new NodeResult(Status.Running, node);   
        }
    }
}
