using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [AddComponentMenu("")]
    [MBTNode(name = "Decorators/Cooldown")]
    public class Cooldown : Decorator
    {
        public AbortTypes abort = AbortTypes.None;
        [Space]
        public FloatReference time = new FloatReference(1f);

        private Coroutine coroutine;
        private float lastExit = float.NegativeInfinity;
        private bool entered = false;
        public enum AbortTypes
        {
            None, LowerPriority
        }

        public override void OnAllowInterrupt()
        {
            if (abort == AbortTypes.LowerPriority)
            {
                StoreBTState();
            }
        }

        public override NodeResult Execute()
        {
            Node node = GetChild();
            if(node == null) {
                return NodeResult.failure;
            }
            if (node.status == Status.Success || node.status == Status.Failure) {
                return NodeResult.From(node.status);
            }
            if (Time.time - lastExit >= time.Value) {
                entered = true;
                return node.runningNodeResult;
            } else {
                return NodeResult.failure;
            }
        }

        public override void OnExit()
        {
            // Record exit time when there was no failure
            if (entered)
            {
                entered = false;
                lastExit = Time.time;
                // For LowerPriority try to abort after given time
                if (abort == AbortTypes.LowerPriority && coroutine == null)
                {
                    coroutine = StartCoroutine(ScheduleCooldown());
                }
            }
        }

        public override void OnDisallowInterrupt()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }

        private IEnumerator ScheduleCooldown()
        {
            yield return new WaitForSeconds(time.Value);
            coroutine = null;
            TryAbort(Abort.LowerPriority);
        }
    }
}
