using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [AddComponentMenu("")]
    [MBTNode("Decorators/Force Result")]
    public class ForceResult : Decorator
    {
        [SerializeField] private ForcedResult result = ForcedResult.Success;

        public override NodeResult Execute()
        {
            if (!TryGetChild(out Node node))
            {
                return NodeResult.failure;
            }

            if (node.status == Status.Success || node.status == Status.Failure)
            {
                return result == ForcedResult.Success ? NodeResult.success : NodeResult.failure;
            }

            return node.runningNodeResult;
        }

        private enum ForcedResult
        {
            Success, Failure
        }
    }
}
