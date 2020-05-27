using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefVars : MonoBehaviour
{
    [SerializeReference]
    public List<BaseVar> variables = new List<BaseVar>();

    void Reset()
    {
        Vector3Var var1 = new Vector3Var();
        var1.key = "Vector #1";
        var1.value = Vector3.up;
        variables.Add(var1);

        IntVar var2 = new IntVar();
        var2.key = "Int #1";
        var2.value = 1;
        variables.Add(var2);

        Vector3Var var3 = new Vector3Var();
        var3.key = "Vector #2";
        var3.value = Vector3.forward;
        variables.Add(var3);
    }
}

[System.Serializable]
public abstract class BaseVar{
    public string key;
}

[System.Serializable]
public abstract class Var<T>: BaseVar{
    public T value;
}

[System.Serializable]
public class IntVar: Var<int>{
}

[System.Serializable]
public class Vector3Var: Var<Vector3>{
}
