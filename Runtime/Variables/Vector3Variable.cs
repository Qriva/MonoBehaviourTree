using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [AddComponentMenu("")]
    public class Vector3Variable : Variable<Vector3>
    {
        protected override bool ValueEquals(Vector3 val1, Vector3 val2)
        {
            return val1 == val2;
        }
    }

    [System.Serializable]
    public class Vector3Reference : VariableReference<Vector3Variable, Vector3>
    {
        public Vector3Reference(VarRefMode mode = VarRefMode.EnableConstant)
        {
            SetMode(mode);
        }
        
        public Vector3Reference(Vector3 defaultConstant)
        {
            useConstant = true;
            constantValue = defaultConstant;
        }

        public Vector3 Value
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
