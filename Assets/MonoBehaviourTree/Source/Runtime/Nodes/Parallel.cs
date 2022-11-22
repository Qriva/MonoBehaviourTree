using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [AddComponentMenu("")]
    [MBTNode(name = "Parallel", order = 150)]
    public class Parallel : Composite
    {
        //private int index;

        public override void OnAllowInterrupt()
        {
            if (random)
            {
                ShuffleList(children);
            }
        }
        
        //public override void OnEnter()
        //{
        //    index = 0;
        //}

        //public override void OnBehaviourTreeAbort()
        //{
        //    // Do not continue from last index
        //    index = 0;
        //}

        public override NodeResult Execute()
        {
            int successCount = 0;
            foreach (var child in children)
            {
                switch (child.status)
                {
                    case Status.Success:
                        successCount++;
                        break;
                        //return NodeResult.success;
                    case Status.Failure:
                        return NodeResult.failure;
                }
            }
            if (successCount == children.Count)
            {
                return NodeResult.success;
            }
            return NodeResult.running;
        }
    }
}
