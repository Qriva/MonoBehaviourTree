using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;

namespace MBTExample
{
    [MBTNode("Example/Move Towards")]
    [AddComponentMenu("")]
    public class MoveTowards : Leaf
    {
        public Vector3Reference targetPosition;
        public TransformReference transformToMove;
        public float speed = 0.1f;

        public override NodeResult Execute()
        {
            Vector3 target = targetPosition.Value;
            Transform obj = transformToMove.Value;
            float dist = Vector3.Distance(target, obj.position);
            if (dist > 0)
            {
                obj.position = Vector3.MoveTowards(
                    obj.position, 
                    target, 
                    (speed > dist)? dist : speed 
                );
                return NodeResult.running;
            }
            else
            {
                return NodeResult.success;
            }
        }
    }
}
