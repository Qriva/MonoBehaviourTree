using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    public abstract class Variable<T> : BlackboardVariable, Observable<T>
    {
        // [HideInInspector]
        [SerializeField]
        protected T val = default(T);
        protected event ChangeListener<T> listeners = delegate {};

        public void AddListener(ChangeListener<T> listener)
        {
            listeners += listener;
        }

        public void RemoveListener(ChangeListener<T> listener)
        {
            listeners -= listener;
        }

        public T Value{
            get
            {
                return val;
            }
            set
            {
                if (!Object.Equals(val, value)) {
                    T oldValue = val;
                    val = value;
                    listeners.Invoke(oldValue, value);
                }
            }
        }
    }

    public delegate void ChangeListener<T>(T oldValue, T newValue);

    public interface Observable<T>
    {
        void AddListener(ChangeListener<T> listener);
        void RemoveListener(ChangeListener<T> listener);
    }

    [System.Serializable]
    public class VariableReference<T> : BaseVariableReference where T : BlackboardVariable
    {
        // Cache
        private T value = null;

        public T Get()
        {
            if (value != null) {
                return value;
            }
            if (blackboard == null || string.IsNullOrEmpty(key)) {
                return null;
            }
            value = blackboard.GetVariable<T>(key);
            #if UNITY_EDITOR
            if (value == null)
            {
                Debug.LogWarningFormat(blackboard, "Variable '{0}' does not exists on blackboard.", key);
            }
            #endif
            return value;
        }
    }

    [System.Serializable]
    public abstract class BaseVariableReference
    {
        public Blackboard blackboard;
        public string key;
    }
}
