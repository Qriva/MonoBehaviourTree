using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [AddComponentMenu("")]
    public class IntVariable : Variable<int>
    {
        protected override bool ValueEquals(int val1, int val2)
        {
            return val1 == val2;
        }
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
