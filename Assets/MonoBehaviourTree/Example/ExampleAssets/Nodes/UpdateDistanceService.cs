using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;

namespace MBTExample
{
    [AddComponentMenu("")]
    [MBTNode("Example/Update Distance Service")]
    public class UpdateDistanceService : Service
    {
        public TransformReference transform1;
        public TransformReference transform2;
        public FloatReference distanceVariable;

        public override void Task()
        {
            Transform t1 = transform1.Get().Value;
            Transform t2 = transform2.Get().Value;
            if (t1 == null || t2 == null)
            {
                return;
            }
            distanceVariable.Get().Value = Vector3.Distance(t1.position, t2.position);
        }
    }
}
