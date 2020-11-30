using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [AddComponentMenu("")]
    [MBTNode(name = "Tasks/Wait")]
    public class Wait : Leaf
    {
        [Tooltip("Wait time in seconds")]
        public FloatReference time = new FloatReference(1f);
        public float randomDeviation = 0f;
        public bool continueOnRestart = false;
        private float timer;
        
        public override void OnEnter()
        {
            if (!continueOnRestart) {
                timer = (randomDeviation == 0f)? 0f : Random.Range(-randomDeviation, randomDeviation);
            }
        }

        public override NodeResult Execute()
        {
            timer += Time.deltaTime;
            if (timer >= time.Value) {
                // Reset timer in case continueOnRestart option is active
                if (continueOnRestart)
                {
                    timer = (randomDeviation == 0f)? 0f : Random.Range(-randomDeviation, randomDeviation);
                }
                return NodeResult.success;
            }
            return NodeResult.running;
        }

        void OnValidate()
        {
            if (time.isConstant)
            {
                randomDeviation = Mathf.Clamp(randomDeviation, 0f, time.GetConstant());
            }
            else
            {
                randomDeviation = Mathf.Clamp(randomDeviation, 0f, 600f);
            }
        }
    }
}
