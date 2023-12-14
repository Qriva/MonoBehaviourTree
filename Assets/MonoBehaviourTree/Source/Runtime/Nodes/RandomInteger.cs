using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [AddComponentMenu("")]
    [MBTNode(name = "Tasks/Random Integer")]
    public class RandomInteger : Leaf
    {
        public IntReference min = new IntReference(0);
        public IntReference max = new IntReference(10);
        public IntReference output = new IntReference(VarRefMode.DisableConstant);

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
