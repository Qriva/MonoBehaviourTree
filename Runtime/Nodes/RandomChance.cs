using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [AddComponentMenu("")]
    [MBTNode("Decorators/Random Chance")]
    public class RandomChance : Decorator
    {
        [Tooltip("Probability should be between 0 and 1")]
        public FloatReference probability = new FloatReference(0.5f);
        private float roll;

        public override void OnAllowInterrupt()
        {
            roll = Random.Range(0f, 1f);
        }

        public override NodeResult Execute()
        {
            Node node = GetChild();
            if (node == null) {
                return NodeResult.failure;
            }
            if (node.status == Status.Success || node.status == Status.Failure) {
                return new NodeResult(node.status);
            }
            if (roll > probability.Value) {
                return NodeResult.failure;
            }
            return new NodeResult(Status.Running, node);
        }

        void OnValidate()
        {
            if (probability.isConstant)
            {
                probability.Value = Mathf.Clamp(probability.GetConstant(), 0f, 1f);
            }
        }
    }
}
