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
    public class BoolReference : VariableReference<BoolVariable>
    {
        
    }
}
