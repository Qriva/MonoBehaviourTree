using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [AddComponentMenu("")]
    [MBTNode(name = "Tasks/Set Number")]
    public class SetNumber : Leaf
    {
        [SerializeField]
        private Type type = Type.Float;
        public Operation operation = Operation.Set;

        public FloatReference sourceFloat = new FloatReference(0f);
        public IntReference sourceInt = new IntReference(0);

        public FloatReference destinationFloat = new FloatReference(VarRefMode.DisableConstant);
        public IntReference destinationInt = new IntReference(VarRefMode.DisableConstant);
        
        public override NodeResult Execute()
        {
            if (type == Type.Float)
            {
                // Set float
                switch (operation)
                {
                    case Operation.Set: destinationFloat.Value = sourceFloat.Value;
                        break;
                    case Operation.Add: destinationFloat.Value += sourceFloat.Value;
                        break;
                    case Operation.Multiply: destinationFloat.Value *= sourceFloat.Value;
                        break;
                }
            }
            else
            {
                // Set int
                switch (operation)
                {
                    case Operation.Set: destinationInt.Value = sourceInt.Value;
                        break;
                    case Operation.Add: destinationInt.Value += sourceInt.Value;
                        break;
                    case Operation.Multiply: destinationInt.Value *= sourceInt.Value;
                        break;
                }
            }
            return NodeResult.success;
        }

        public override bool IsValid()
        {
            switch (type)
            {
                case Type.Float: return !(sourceFloat.isInvalid || destinationFloat.isInvalid);
                case Type.Int: return !(sourceInt.isInvalid || destinationInt.isInvalid);
                default: return true;
            }
        }

        private enum Type
        {
            Float, Int
        }

        public enum Operation
        {
            [InspectorName("=")]
            Set, 
            [InspectorName("+=")]
            Add, 
            [InspectorName("*=")]
            Multiply
        }
    }
}
