using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [AddComponentMenu("")]
    [MBTNode("Decorators/Time Limit")]
    public class TimeLimit : Decorator
    {
        public FloatReference time = new FloatReference(5f);
        public float randomDeviation = 0f;
        private bool limitReached;
        private float timeout;

        public override void OnAllowInterrupt()
        {
            StoreBTState();
        }

        public override void OnEnter()
        {
            // Reset block flag
            limitReached = false;
            timeout = Time.time + time.Value + ((randomDeviation == 0f)? 0f : Random.Range(-randomDeviation, randomDeviation));
            this.behaviourTree.onTick += OnBehaviourTreeTick;
        }

        public override NodeResult Execute()
        {
            Node node = GetChild();
            if (node == null || limitReached) {
                return NodeResult.failure;
            }
            if (node.status == Status.Success || node.status == Status.Failure) {
                return NodeResult.From(node.status);
            }
            return node.runningNodeResult;
        }

        public override void OnExit()
        {
            this.behaviourTree.onTick -= OnBehaviourTreeTick;
        }

        private void OnBehaviourTreeTick()
        {
            if (timeout <= Time.time)
            {
                timeout = float.MaxValue;
                limitReached = true;
                TryAbort(Abort.Self);
            }
        }

        void OnValidate()
        {
            if (time.isConstant)
            {
                // this is safe to use only when reference is constant
                time.Value = Mathf.Max(0f, time.GetConstant());
                randomDeviation = Mathf.Clamp(randomDeviation, 0f, time.GetConstant());
            }
            else
            {
                randomDeviation = Mathf.Clamp(randomDeviation, 0f, 600f);
            }
        }
    }
}
