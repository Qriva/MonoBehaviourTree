using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
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
            return (dictionary.TryGetValue(key, out BlackboardVariable val)) ? (T)val : null;
        }

        #if UNITY_EDITOR
        [ContextMenu("Delete all variables", false)]
        protected void DeleteAllVariables()
        {
            for (int i = 0; i < variables.Count; i++)
            {
                UnityEditor.Undo.DestroyObjectImmediate(variables[i]);
            }
            variables.Clear();
        }

        [ContextMenu("Delete all variables", true)]
        protected bool HasVariables()
        {
            return variables.Count > 0;
        }
        #endif
    }
}
