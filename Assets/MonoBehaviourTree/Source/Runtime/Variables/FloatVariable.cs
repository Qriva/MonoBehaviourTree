using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [AddComponentMenu("")]
    public class FloatVariable : Variable<float>
    {
        
    }

    [System.Serializable]
    public class FloatReference : VariableReference<FloatVariable>
    {
        
    }
}
