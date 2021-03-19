using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-1000)]
    public class Blackboard : MonoBehaviour
    {
        public List<BlackboardVariable> variables = new List<BlackboardVariable>();
        private Dictionary<string, BlackboardVariable> dictionary = new Dictionary<string, BlackboardVariable>();
        [Tooltip("When set, this blackboard will replace variables with matching names from target parent")]
        public Blackboard masterBlackboard;

        // IMPROVEMENT: https://docs.unity3d.com/ScriptReference/ISerializationCallbackReceiver.html
        void Awake()
        {
            // Initialize variables by keys
            dictionary.Clear();
            for (int i = 0; i < variables.Count; i++)
            {
                BlackboardVariable var = variables[i];
                dictionary.Add(var.key, var);
            }
            // Replace variables from master blackboard
            if (masterBlackboard != null)
            {
                List<BlackboardVariable> parentVars = masterBlackboard.variables;
                for (int i = 0; i < parentVars.Count; i++)
                {
                    // Find if there is variable with the same key
                    BlackboardVariable parentVar = parentVars[i];
                    if (dictionary.TryGetValue(parentVar.key, out BlackboardVariable currentVar))
                    {
                        // Ensure that both of them are the same type
                        if (currentVar.GetType().IsAssignableFrom(parentVar.GetType()))
                        {
                            // There are matching variables, replace current one with master blackboard var
                            dictionary[parentVar.key] = parentVar;
                        }
                        else
                        {
                            Debug.LogErrorFormat(this, 
                                "Blackboard variable key '{0}' cannot be replaced. " + 
                                "Master blackboard variable of type {1} cannot be assigned to {2}.", 
                                currentVar.key,
                                parentVar.GetType().Name,
                                currentVar.GetType().Name
                            );
                        }
                    }
                }
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

        void OnValidate()
        {
            if (masterBlackboard == this)
            {
                Debug.LogWarning("Master blackboard cannot be the same instance.");
                masterBlackboard = null;
            }
        }
        #endif
    }
}
