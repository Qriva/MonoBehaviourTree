using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [System.Obsolete("This is an obsolete node")]
    [MBTNode(name = "Test/Test leaf node")]
    public class TestLeaf : Leaf
    {
        public int test;
        public int time = 0;
        public IntReference intVariable;
        // public StringReference stringReference;
        private int currentTime;

        public override NodeResult Execute()
        {
            if (currentTime > 0) {
                currentTime -= 1;
                return new NodeResult(Status.Running);
            }
            return new NodeResult(Status.Success);
        }

        public override void OnEnter()
        {
            currentTime = time;
            Debug.Log("TEST LEAF");
        }
    }
}
