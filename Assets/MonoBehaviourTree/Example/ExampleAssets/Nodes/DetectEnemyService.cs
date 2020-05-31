using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;

namespace MBTExample
{
    [AddComponentMenu("")]
    [MBTNode("Example/Detect Enemy Service")]
    public class DetectEnemyService : Service
    {
        public LayerMask mask = -1;
        public float range = 15;
        public TransformReference variableToSet;

        public override void Task()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, range, mask, QueryTriggerInteraction.Ignore);
            if (colliders.Length > 0)
            {
                variableToSet.Get().Value = colliders[0].transform;
            }
            else
            {
                variableToSet.Get().Value = null;
            }
        }
    }
}
