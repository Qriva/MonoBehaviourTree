using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [AddComponentMenu("")]
    public class TransformVariable : Variable<Transform>
    {
        protected override bool ValueEquals(Transform val1, Transform val2)
        {
            return val1 == val2;
        }
    }

    [System.Serializable]
    public class TransformReference : VariableReference<TransformVariable, Transform>
    {
        public TransformReference(VarRefMode mode = VarRefMode.EnableConstant)
        {
            SetMode(mode);
        }

        protected override bool isConstantValid
        {
            get { return constantValue != null; }
        }

        public Transform Value
        {
            get
            {
                return (useConstant)? constantValue : this.GetVariable().Value;
            }
            set
            {
                if (useConstant)
                {
                    constantValue = value;
                }
                else
                {
                    this.GetVariable().Value = value;
                }
            }
        }
    }
}
