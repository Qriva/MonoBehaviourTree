using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [AddComponentMenu("")]
    public class BoolVariable : Variable<bool>
    {
        
    }

    [System.Serializable]
    public class BoolReference : VariableReference<BoolVariable, bool>
    {
        public BoolReference(VarRefMode mode = VarRefMode.EnableConstant)
        {
            SetMode(mode);
        }
        
        public BoolReference(bool defaultConstant)
        {
            useConstant = true;
            constantValue = defaultConstant;
        }

        public bool Value
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
