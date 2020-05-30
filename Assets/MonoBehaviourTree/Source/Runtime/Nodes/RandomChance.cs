using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [AddComponentMenu("")]
    [MBTNode("Decorators/Random Chance")]
    public class RandomChance : Decorator
    {
        [Range(0f, 1f)]
        public float probability = 0.5f;

        public override NodeResult Execute()
        {
            Node node = GetChild();
            if (node == null) {
                return NodeResult.failure;
            }
            if (node.status == Status.Success || node.status == Status.Failure) {
                return new NodeResult(node.status);
            }
            float roll = Random.Range(0f, 1f);
            if (roll > probability) {
                return NodeResult.failure;
            }
            return new NodeResult(Status.Running, node);
        }
    }
}
