using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [AddComponentMenu("")]
    public class ObjectVariable : Variable<UnityEngine.Object>
    {
        
    }
    
    [System.Serializable]
    public class ObjectReference : VariableReference<ObjectVariable, UnityEngine.Object>
    {
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
