using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [AddComponentMenu("")]
    [MBTNode(name = "Tasks/Random Float")]
    public class RandomFloat : Leaf
    {
        public FloatReference min = new FloatReference(0f);
        public FloatReference max = new FloatReference(1f);
        public FloatReference output = new FloatReference(VarRefMode.DisableConstant);

        public override NodeResult Execute()
        {
            output.Value = Random.Range(min.Value, max.Value);
            return NodeResult.success;
        }

        void OnValidate()
        {
            if (min.isConstant && max.isConstant)
            {
                min.Value = Mathf.Min(min.GetConstant(), max.GetConstant());
            }
        }
    }
}
