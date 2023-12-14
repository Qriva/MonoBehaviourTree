using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    public abstract class Service : Decorator, IMonoBehaviourTreeTickListener
    {
        public float interval = 1f;
        public float randomDeviation = 0f;
        public bool callOnEnter = true;
        /// <summary>
        /// Time of the next update of the task
        /// </summary>
        protected float nextScheduledTime;

        public override void OnEnter()
        {
            // Set time of the next update
            nextScheduledTime = Time.time + interval + Random.Range(-randomDeviation, randomDeviation);
            behaviourTree.AddTickListener(this);
            if (callOnEnter)
            {
                Task();
            }
        }

        public override NodeResult Execute()
        {
            Node node = GetChild();
            if (node == null) {
                return NodeResult.failure;
            }
            if (node.status == Status.Success || node.status == Status.Failure) {
                return NodeResult.From(node.status);
            }
            return node.runningNodeResult;
        }

        public abstract void Task();

        public override void OnExit()
        {
            behaviourTree.RemoveTickListener(this);
        }

        void IMonoBehaviourTreeTickListener.OnBehaviourTreeTick()
        {
            if (nextScheduledTime <= Time.time)
            {
                // Set time of the next update and run the task
                nextScheduledTime = Time.time + interval + Random.Range(-randomDeviation, randomDeviation);
                Task();
            }
        }

        protected virtual void OnValidate()
        {
            interval = Mathf.Max(0f, interval);
            randomDeviation = Mathf.Clamp(randomDeviation, 0f, interval);
        }
    }
}
