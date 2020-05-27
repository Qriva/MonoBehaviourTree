using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonoBT
{
    public class ObjectVariable : Variable<UnityEngine.Object>
    {
        
    }
    
    [System.Serializable]
    public class ObjectReference : VariableReference<ObjectVariable>
    {
        
    }
}
