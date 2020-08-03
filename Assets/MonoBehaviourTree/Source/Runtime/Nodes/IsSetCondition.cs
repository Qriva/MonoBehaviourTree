using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [AddComponentMenu("")]
    [MBTNode(name = "Conditions/Is Set Condition")]
    public class IsSetCondition : Condition
    {
        public Abort abort;
        public bool invert = false;
        public Type type = Type.Boolean;
        public BoolReference boolReference = new BoolReference(VarRefMode.DisableConstant);
        public ObjectReference objectReference = new ObjectReference(VarRefMode.DisableConstant);
        public TransformReference transformReference = new TransformReference(VarRefMode.DisableConstant);
        
        public override bool Check()
        {
            switch (type)
            {
                case Type.Boolean:
                    return (boolReference.Value == true) ^ invert;
                case Type.Object:
                    return (objectReference.Value != null) ^ invert;
                case Type.Transform:
                    return (transformReference.Value != null) ^ invert;
            }
            return invert;
        }

        public override void OnAllowInterrupt()
        {
            if (abort != Abort.None)
            {
                StoreBTState();
                switch (type)
                {
                    case Type.Boolean:
                        boolReference.GetVariable().AddListener(OnVariableChange);
                        break;
                    case Type.Object:
                        objectReference.GetVariable().AddListener(OnVariableChange);
                        break;
                    case Type.Transform:
                        transformReference.GetVariable().AddListener(OnVariableChange);
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
                        boolReference.GetVariable().RemoveListener(OnVariableChange);
                        break;
                    case Type.Object:
                        objectReference.GetVariable().RemoveListener(OnVariableChange);
                        break;
                    case Type.Transform:
                        transformReference.GetVariable().RemoveListener(OnVariableChange);
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
