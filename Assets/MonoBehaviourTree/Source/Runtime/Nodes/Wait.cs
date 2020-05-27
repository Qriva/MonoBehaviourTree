using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [MBTNode(name = "Wait")]
    public class Wait : Leaf
    {
        [Tooltip("Wait time in seconds")]
        public float time = 1f;
        public bool continueOnRestart = false;
        private float timer;
        
        public override void OnEnter()
        {
            if (!continueOnRestart) {
                timer = 0;
            }
        }

        public override NodeResult Execute()
        {
            timer += Time.deltaTime;
            if (timer > time) {
                // Reset timer in case continueOnRestart option is active
                timer = 0;
                return new NodeResult(Status.Success);
            }
            return new NodeResult(Status.Running);
        }
    }
}
