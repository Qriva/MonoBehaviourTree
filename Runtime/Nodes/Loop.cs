using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [AddComponentMenu("")]
    [MBTNode(name = "Decorators/Loop")]
    public class Loop : Decorator
    {
        public IntReference loops = new IntReference(3);
        public BoolReference infinite = new BoolReference(false);
        [Tooltip("Break loop when selected result is returned by child.")]
        public BreakMode breakOnStatus = BreakMode.Disabled;
        [Space]
        [Tooltip("Result returned by this node after the loop ends.")]
        public ResultRemapMode resultOnFinish = ResultRemapMode.Success;
        [Tooltip("The result returned by this node when loop is broken.")]
        public ResultRemapMode resultOnBreak = ResultRemapMode.Failure;
        private int count;

        public enum ResultRemapMode
        {
            Success = 0,
            Failure = 1,
            Inherit = 2,
            InheritInverted = 3,
        }

        /// <summary>
        /// Enum mapped to Status enum. Disabled is casted to 'running' as this state is never returned by child.
        /// </summary>
        public enum BreakMode
        {
            Disabled = 2,
            Success = 0,
            Failure = 1,
        }

        public override void OnEnter()
        {
            count = loops.Value;
        }

        public override NodeResult Execute()
        {
            if (!TryGetChild(out Node node))
            {
                return NodeResult.failure;
            }

            if (node.status == (Status)breakOnStatus)
            {
                return RemapResult(resultOnBreak, node.status);
            }

            if (count > 0 || infinite.Value)
            {
                // Repeat children
                behaviourTree.ResetNodesTo(this);
                count -= 1;
                return node.runningNodeResult;
            }

            return RemapResult(resultOnFinish, node.status);
        }

        private NodeResult RemapResult(ResultRemapMode mode, Status childStatus)
        {
            switch (mode)
            {
                case ResultRemapMode.Success: return NodeResult.success;
                case ResultRemapMode.Failure: return NodeResult.failure;
                case ResultRemapMode.Inherit: return childStatus == Status.Success ? NodeResult.success : NodeResult.failure;
                case ResultRemapMode.InheritInverted: return childStatus == Status.Success ? NodeResult.failure : NodeResult.success;
                default: Debug.LogError("Unexpected behaviour", this); return NodeResult.failure;
            }
        }
    }
}
