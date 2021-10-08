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
        public GameObjectReference objectReference = new GameObjectReference(VarRefMode.DisableConstant);
        public TransformReference transformReference = new TransformReference(VarRefMode.DisableConstant);
        
        public override bool Check()
        {
            switch (type)
            {
                case Type.Boolean:
                    return (boolReference.Value == true) ^ invert;
                case Type.GameObject:
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
                ObtainTreeSnapshot();
                switch (type)
                {
                    case Type.Boolean:
                        boolReference.GetVariable().AddListener(OnVariableChange);
                        break;
                    case Type.GameObject:
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
                    case Type.GameObject:
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

        private void OnVariableChange(GameObject oldValue, GameObject newValue)
        {
            EvaluateConditionAndTryAbort(abort);
        }

        private void OnVariableChange(Transform oldValue, Transform newValue)
        {
            EvaluateConditionAndTryAbort(abort);
        }

        public override bool IsValid()
        {
            switch (type)
            {
                case Type.Boolean: return !boolReference.isInvalid;
                case Type.GameObject: return !objectReference.isInvalid;
                case Type.Transform: return !transformReference.isInvalid;
                default: return true;
            }
        }

        public enum Type
        {
            Boolean, GameObject, Transform
        }
    }
}
