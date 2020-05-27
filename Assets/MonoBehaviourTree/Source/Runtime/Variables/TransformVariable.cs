using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonoBT
{
    public class TransformVariable : Variable<Transform>
    {
        
    }

    [System.Serializable]
    public class TransformReference : VariableReference<TransformVariable>
    {
        
    }
}
