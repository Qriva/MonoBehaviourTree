using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonoBT
{
    [MBTNode("Random Chance")]
    public class RandomChance : Decorator
    {
        public float probability = 0.5f;

        public override NodeResult Execute()
        {
            Node node = GetChild();
            if (node == null) {
                return new NodeResult(Status.Failure);
            }
            if (node.status == Status.Success || node.status == Status.Failure) {
                return new NodeResult(node.status);
            }
            float roll = Random.Range(0f, 1f);
            if (roll > probability) {
                return new NodeResult(Status.Failure);
            }
            return new NodeResult(Status.Running, node);
        }
    }
}
