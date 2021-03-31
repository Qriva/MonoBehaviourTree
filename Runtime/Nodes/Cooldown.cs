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
        private float cooldownTime = 0f;
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
            if (cooldownTime <= Time.time) {
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
                cooldownTime = Time.time + time.Value;
                // For LowerPriority try to abort after given time
                if (abort == AbortTypes.LowerPriority)
                {
                    behaviourTree.onTick += OnBehaviourTreeTick;
                }
            }
        }

        public override void OnDisallowInterrupt()
        {
            behaviourTree.onTick -= OnBehaviourTreeTick;
        }

        private void OnBehaviourTreeTick()
        {
            if (cooldownTime <= Time.time)
            {
                // Task should be aborted, so there is no need to listen anymore
                behaviourTree.onTick -= OnBehaviourTreeTick;
                TryAbort(Abort.LowerPriority);
            }
        }
    }
}
