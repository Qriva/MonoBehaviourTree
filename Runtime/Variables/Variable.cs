using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    // [RequireComponent(typeof(Blackboard))]
    public abstract class Variable<T> : BlackboardVariable, Observable<T>
    {
        // [HideInInspector]
        [SerializeField]
        protected T val = default(T);
        protected event ChangeListener<T> listeners = delegate {};

        protected abstract bool ValueEquals(T val1, T val2);

        public void AddListener(ChangeListener<T> listener)
        {
            listeners += listener;
        }

        public void RemoveListener(ChangeListener<T> listener)
        {
            listeners -= listener;
        }

        public T Value
        {
            get
            {
                return val;
            }
            set
            {
                if (!ValueEquals(val, value)) {
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
    public class VariableReference<T, U> : BaseVariableReference where T : BlackboardVariable
    {
        // Cache
        protected T value = null;
        [SerializeField]
        protected U constantValue = default(U);

        /// <summary>
        /// Returns observable Variable or null if it doesn't exists on blackboard.
        /// </summary>
        public T GetVariable()
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

        public U GetConstant()
        {
            return constantValue;
        }
    }

    public enum VarRefMode { EnableConstant, DisableConstant }

    [System.Serializable]
    public abstract class BaseVariableReference
    {
        [SerializeField]
        protected bool useConstant = false;
        // Additional editor feature to lock switch
        [SerializeField]
        protected VarRefMode mode = VarRefMode.EnableConstant;
        public Blackboard blackboard;
        public string key;

        public virtual bool isConstant
        {
            get { return useConstant; }
        }

        /// <summary>
        /// Returns true when constant value is valid
        /// </summary>
        /// <value></value>
        protected virtual bool isConstantValid
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Returns true when variable setup is invalid
        /// </summary>
        public bool isInvalid
        {
            get { return (isConstant)? !isConstantValid : blackboard == null || string.IsNullOrEmpty(key); }
        }

        protected void SetMode(VarRefMode mode)
        {
            this.mode = mode;
            useConstant = (mode == VarRefMode.DisableConstant)? false : useConstant;
        }
    }
}
