using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [AddComponentMenu("")]
    public class Vector3Variable : Variable<Vector3>
    {
        
    }
    
    [System.Serializable]
    public class Vector3Reference : VariableReference<Vector3Variable, Vector3>
    {
        public Vector3 Value
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
