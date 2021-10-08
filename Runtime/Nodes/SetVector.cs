using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [AddComponentMenu("")]
    [MBTNode(name = "Tasks/Set Vector")]
    public class SetVector : Leaf
    {
        [SerializeField]
        private Type type = Type.Vector3;

        public Vector2Reference sourceVector2 = new Vector2Reference();
        public Vector3Reference sourceVector3 = new Vector3Reference();

        public Vector2Reference destinationVector2 = new Vector2Reference(VarRefMode.DisableConstant);
        public Vector3Reference destinationVector3 = new Vector3Reference(VarRefMode.DisableConstant);

        public override NodeResult Execute()
        {
            if (type == Type.Vector3)
            {
                destinationVector3.Value = sourceVector3.Value;
            }
            else
            {
                destinationVector2.Value = sourceVector2.Value;
            }
            return NodeResult.success;
        }

        public override bool IsValid()
        {
            switch (type)
            {
                case Type.Vector3: return !(sourceVector3.isInvalid || destinationVector3.isInvalid);
                case Type.Vector2: return !(sourceVector2.isInvalid || destinationVector2.isInvalid);
                default: return true;
            }
        }

        private enum Type
        {
            Vector2, Vector3
        }
    }
}
