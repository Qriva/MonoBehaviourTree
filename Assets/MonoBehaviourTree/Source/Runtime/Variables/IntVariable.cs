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
    public class IntReference : VariableReference<IntVariable>
    {
        
    }
}
