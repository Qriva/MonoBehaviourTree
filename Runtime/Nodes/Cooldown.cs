using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [MBTNode(name = "Cooldown")]
    [AddComponentMenu("")]
    public class Cooldown : Decorator
    {
        public float time = 1f;
        public Abort abort = Abort.None;

        private Coroutine coroutine;
        private bool ready = true;
        private bool canAbort = false;

        public override void OnAllowInterrupt()
        {
            if (abort != Abort.None) {
                StoreBTState();
                canAbort = true;
            }
        }

        public override void OnDisallowInterrupt()
        {
            canAbort = false;
            DisposeBTState();
        }

        public override NodeResult Execute()
        {
            Node node = GetChild();
            if(node == null) {
                return new NodeResult(Status.Failure);
            }
            if (node.status == Status.Success || node.status == Status.Failure) {
                return new NodeResult(node.status);
            }
            if (ready) {
                // Reset cooldown after given time
                coroutine = StartCoroutine(ScheduleCooldown(time));
                ready = false;
                return new NodeResult(Status.Running, node);
            } else {
                return new NodeResult(Status.Failure);
            }
        }

        private IEnumerator ScheduleCooldown(float t)
        {
            yield return new WaitForSeconds(t);
            ready = true;
            coroutine = null;
            if (canAbort) {
                TryAbort(abort);
            }
        }
    }
}
