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
