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
    public class ObjectReference : VariableReference<ObjectVariable>
    {
        
    }
}
