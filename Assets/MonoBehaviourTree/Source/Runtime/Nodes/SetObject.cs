using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [AddComponentMenu("")]
    [MBTNode(name = "Tasks/Set Object")]
    public class SetObject : Leaf
    {
        [SerializeField]
        private Type type = Type.Transform;
        public TransformReference sourceTransform;
        public GameObjectReference sourceGameObject;
        public TransformReference destinationTransform = new TransformReference(VarRefMode.DisableConstant);
        public GameObjectReference destinationGameObject = new GameObjectReference(VarRefMode.DisableConstant);

        public override NodeResult Execute()
        {
            if (type == Type.Transform)
            {
                destinationTransform.Value = sourceTransform.Value;
            }
            else
            {
                destinationGameObject.Value = sourceGameObject.Value;
            }
            return NodeResult.success;
        }

        public override bool IsValid()
        {
            // Custom validation to allow nulls in source objects
            switch (type)
            {
                case Type.Transform: return !( sourceTransform == null || IsInvalid(sourceTransform) || destinationTransform.isInvalid);
                case Type.GameObject: return !( sourceGameObject == null || IsInvalid(sourceGameObject) || destinationGameObject.isInvalid);
                default: return true;
            }
        }

        private static bool IsInvalid(BaseVariableReference variable)
        {
            // Custom validation to allow null objects without warnings
            return (variable.isConstant)? false : variable.blackboard == null || string.IsNullOrEmpty(variable.key);
        }

        private enum Type
        {
            Transform, GameObject
        }
    }
}
