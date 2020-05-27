using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [MBTNode(name = "Is Set Condition")]
    public class IsSetCondition : Condition
    {
        public Abort abort;
        public bool invert = false;
        public Type type = Type.Boolean;
        public BoolReference boolReference;
        public ObjectReference objectReference;
        public TransformReference transformReference;
        
        public override bool Check()
        {
            switch (type)
            {
                case Type.Boolean:
                    return (boolReference.Get().Value == true) ^ invert;
                case Type.Object:
                    return (objectReference.Get().Value != null) ^ invert;
                case Type.Transform:
                    return (transformReference.Get().Value != null) ^ invert;
            }
            return invert;
        }

        public override void OnAllowInterrupt()
        {
            if (abort != Abort.None)
            {
                switch (type)
                {
                    case Type.Boolean:
                        boolReference.Get().AddListener(OnVariableChange);
                        break;
                    case Type.Object:
                        objectReference.Get().AddListener(OnVariableChange);
                        break;
                    case Type.Transform:
                        transformReference.Get().AddListener(OnVariableChange);
                        break;
                }
            }
        }

        public override void OnDisallowInterrupt()
        {
            if (abort != Abort.None)
            {
                switch (type)
                {
                    case Type.Boolean:
                        boolReference.Get().RemoveListener(OnVariableChange);
                        break;
                    case Type.Object:
                        objectReference.Get().RemoveListener(OnVariableChange);
                        break;
                    case Type.Transform:
                        transformReference.Get().RemoveListener(OnVariableChange);
                        break;
                }
            }
        }

        private void OnVariableChange(bool oldValue, bool newValue)
        {
            EvaluateConditionAndTryAbort(abort);
        }

        private void OnVariableChange(Object oldValue, Object newValue)
        {
            EvaluateConditionAndTryAbort(abort);
        }

        private void OnVariableChange(Transform oldValue, Transform newValue)
        {
            EvaluateConditionAndTryAbort(abort);
        }

        public enum Type
        {
            Boolean, Object, Transform
        }
    }
}
