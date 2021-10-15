using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [AddComponentMenu("")]
    [MBTNode("Services/Calculate Distance Service")]
    public class CalculateDistanceService : Service
    {
        [Space]
        public TransformReference transform1;
        public TransformReference transform2;
        public FloatReference variable = new FloatReference(VarRefMode.DisableConstant);
        
        public override void Task()
        {
            Transform t1 = transform1.Value;
            Transform t2 = transform2.Value;
            if (t1 == null || t2 == null)
            {
                return;
            }
            variable.Value = Vector3.Distance(t1.position, t2.position);
        }
    }
}
