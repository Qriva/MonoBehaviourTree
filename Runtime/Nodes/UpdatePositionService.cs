using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [AddComponentMenu("")]
    [MBTNode("Services/Update Position Service")]
    public class UpdatePositionService : Service
    {
        public TransformReference sourceTransform;
        public Vector3Reference position = new Vector3Reference(VarRefMode.DisableConstant);

        public override void Task()
        {
            Transform t = sourceTransform.Value;
            if (t == null)
            {
                return;
            }
            position.Value = t.position;
        }
    }
}
