using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [AddComponentMenu("")]
    [MBTNode("Decorators/Time Limit")]
    public class TimeLimit : Decorator
    {
        public float time = 5f;
        private Coroutine coroutine;
        private WaitForSeconds waitForSeconds;
        private bool limitReached;

        public override void OnAllowInterrupt()
        {
            StoreBTState();
        }

        public override void OnEnter()
        {
            // IMPROVEMENT: WaitForSeconds could be initialized in some special node init callback
            if (waitForSeconds == null)
            {
                // Create new WaitForSeconds
                OnValidate();
            }
            // Reset block flag
            limitReached = false;
            coroutine = StartCoroutine(ScheduleTimeLimit());
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
            if (coroutine == null)
            {
                return;
            }
            limitReached = false;
            StopCoroutine(coroutine);
            coroutine = null;
        }

        private IEnumerator ScheduleTimeLimit()
        {
            yield return waitForSeconds;
            limitReached = true;
            coroutine = null;
            TryAbort(Abort.Self);
        }

        void OnValidate()
        {
            time = Mathf.Max(0f, time);
            waitForSeconds = new WaitForSeconds(time);
        }
    }
}
