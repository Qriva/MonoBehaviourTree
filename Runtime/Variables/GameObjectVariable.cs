using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    // TODO: This should be GameObject
    [AddComponentMenu("")]
    public class GameObjectVariable : Variable<GameObject>
    {
        protected override bool ValueEquals(GameObject val1, GameObject val2)
        {
            return val1 == val2;
        }
    }

    [System.Serializable]
    public class GameObjectReference : VariableReference<GameObjectVariable, GameObject>
    {
        public GameObjectReference(VarRefMode mode = VarRefMode.EnableConstant)
        {
            SetMode(mode);
        }
        
        public GameObject Value
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
