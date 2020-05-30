using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    public abstract class Condition : Decorator
    {
        protected bool lastConditionCheckResult = false;

        public override NodeResult Execute()
        {
            Node node = GetChild();
            if (node == null) {
                return NodeResult.failure;
            }
            if (node.status == Status.Success || node.status == Status.Failure) {
                return new NodeResult(node.status);
            }
            lastConditionCheckResult = Check();
            if (lastConditionCheckResult == false) {
                return NodeResult.failure;
            }
            return new NodeResult(Status.Running, node);
        }

        /// <summary>
        /// Reevaluate condition and try to abort the tree if required
        /// </summary>
        /// <param name="abort">Abort type</param>
        protected void EvaluateConditionAndTryAbort(Abort abortType)
        {
            bool c = Check();
            if (c != lastConditionCheckResult)
            {
                lastConditionCheckResult = c;
                TryAbort(abortType);
            }
        }

        /// <summary>
        /// Method called to check condition
        /// </summary>
        /// <returns>Condition result</returns>
        public abstract bool Check();
    }
}
