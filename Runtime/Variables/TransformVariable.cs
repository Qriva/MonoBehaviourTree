using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [AddComponentMenu("")]
    public class TransformVariable : Variable<Transform>
    {
        
    }

    [System.Serializable]
    public class TransformReference : VariableReference<TransformVariable, Transform>
    {
        public TransformReference(VarRefMode mode = VarRefMode.EnableConstant)
        {
            SetMode(mode);
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
