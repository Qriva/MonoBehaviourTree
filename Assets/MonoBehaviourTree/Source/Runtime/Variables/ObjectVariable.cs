using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    // TODO: This should be GameObject
    [AddComponentMenu("")]
    public class ObjectVariable : Variable<UnityEngine.Object>
    {
        protected override bool ValueEquals(Object val1, Object val2)
        {
            return val1 == val2;
        }
    }

    [System.Serializable]
    public class ObjectReference : VariableReference<ObjectVariable, UnityEngine.Object>
    {
        public ObjectReference(VarRefMode mode = VarRefMode.EnableConstant)
        {
            SetMode(mode);
        }
        
        public UnityEngine.Object Value
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
