using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [AddComponentMenu("")]
    [MBTNode(name = "Tasks/Set Boolean")]
    public class SetBoolean : Leaf
    {
        public BoolReference source = new BoolReference(true);
        // [SerializeField]
        // private Operation operation = Operation.Set;
        public BoolReference destination = new BoolReference(VarRefMode.DisableConstant);
        
        public override NodeResult Execute()
        {
            destination.Value = source.Value;
            return NodeResult.success;
        }

        // private enum Operation
        // {
        //     [InspectorName("=")]
        //     Set, 
        //     [InspectorName("|=")]
        //     Or, 
        //     [InspectorName("^=")]
        //     Xor, 
        //     [InspectorName("&=")]
        //     And
        // }
    }
}
