using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [AddComponentMenu("")]
    public class FloatVariable : Variable<float>
    {
        
    }

    [System.Serializable]
    public class FloatReference : VariableReference<FloatVariable, float>
    {
        public FloatReference(VarRefMode mode = VarRefMode.EnableConstant)
        {
            SetMode(mode);
        }
        
        public FloatReference(float defaultConstant)
        {
            useConstant = true;
            constantValue = defaultConstant;
        }
        
        public float Value
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
