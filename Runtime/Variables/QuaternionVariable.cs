using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [AddComponentMenu("")]
    public class QuaternionVariable : Variable<Quaternion>
    {
        
    }

    [System.Serializable]
    public class QuaternionReference : VariableReference<QuaternionVariable, Quaternion>
    {
        public QuaternionReference(VarRefMode mode = VarRefMode.EnableConstant)
        {
            SetMode(mode);
        }
        
        public QuaternionReference(Quaternion defaultConstant)
        {
            useConstant = true;
            constantValue = defaultConstant;
        }

        public Quaternion Value
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
