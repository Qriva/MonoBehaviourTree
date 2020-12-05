using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [AddComponentMenu("")]
    [MBTNode(name = "Sequence", order = 150)]
    public class Sequence : Composite
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
                        index += 1;
                        continue;
                    case Status.Failure:
                        return NodeResult.failure; 
                }
                return child.runningNodeResult;
            }
            return NodeResult.success;
        }
    }
}
