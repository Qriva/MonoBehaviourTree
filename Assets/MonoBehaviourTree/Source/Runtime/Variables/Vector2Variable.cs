using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [AddComponentMenu("")]
    public class Vector2Variable : Variable<Vector2>
    {
        protected override bool ValueEquals(Vector2 val1, Vector2 val2)
        {
            return val1 == val2;
        }
    }

    [System.Serializable]
    public class Vector2Reference : VariableReference<Vector2Variable, Vector2>
    {
        public Vector2Reference(VarRefMode mode = VarRefMode.EnableConstant)
        {
            SetMode(mode);
        }
        
        public Vector2Reference(Vector2 defaultConstant)
        {
            useConstant = true;
            constantValue = defaultConstant;
        }
        
        public Vector2 Value
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
