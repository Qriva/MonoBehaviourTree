using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonoBT
{
    [DisallowMultipleComponent]
    public class Blackboard : MonoBehaviour
    {
        public List<BlackboardVariable> variables = new List<BlackboardVariable>();
        private Dictionary<string, BlackboardVariable> dictionary = new Dictionary<string, BlackboardVariable>();

        void Awake()
        {
            // Initialize variables by keys
            dictionary.Clear();
            for (int i = 0; i < variables.Count; i++)
            {
                BlackboardVariable var = variables[i];
                dictionary.Add(var.key, var);
            }
        }

        public BlackboardVariable[] GetAllVariables()
        {
            return variables.ToArray();
        }

        public T GetVariable<T>(string key) where T : BlackboardVariable
        {
            return (T)dictionary[key];
        }
    }
}
