using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonoBT
{
    [MBTNode(name = "Sequence", order = 200)]
    public class Sequence : Composite
    {
        private int index;
        
        public override void OnEnter()
        {
            index = 0;
        }

        public override NodeResult Execute()
        {
            // TODO: Missing random sequence implementation
            while (index < children.Count)
            {
                Node child = children[index];
                switch (child.status)
                {
                    case Status.Success:
                        index += 1;
                        continue;
                    case Status.Failure:
                        return new NodeResult(Status.Failure); 
                }
                return new NodeResult(Status.Running, child);
            }
            return new NodeResult(Status.Success);
        }
    }
}
