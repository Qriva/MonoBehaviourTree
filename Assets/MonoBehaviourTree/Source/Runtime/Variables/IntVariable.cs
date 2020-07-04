using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [AddComponentMenu("")]
    public class IntVariable : Variable<int>
    {
        
    }

    [System.Serializable]
    public class IntReference : VariableReference<IntVariable, int>
    {
        public IntReference(VarRefMode mode = VarRefMode.EnableConstant)
        {
            SetMode(mode);
        }
        
        public IntReference(int defaultConstant)
        {
            useConstant = true;
            constantValue = defaultConstant;
        }
        
        public int Value
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
