using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [AddComponentMenu("")]
    [MBTNode(name = "Selector", order = 100)]
    public class Selector : Composite
    {
        private int index;

        public override void OnAllowInterrupt()
        {
            if (random)
            {
                ShuffleList(children);
            }
        }
        
        public override void OnEnter()
        {
            index = 0;
        }

        public override void OnBehaviourTreeAbort()
        {
            // Do not continue from last index
            index = 0;
        }

        public override NodeResult Execute()
        {
            while (index < children.Count)
            {
                Node child = children[index];
                switch (child.status)
                {
                    case Status.Success:
                        return NodeResult.success;
                    case Status.Failure:
                        index += 1;
                        continue;
                }
                return new NodeResult(Status.Running, child);
            }
            return NodeResult.failure;
        }
    }
}
