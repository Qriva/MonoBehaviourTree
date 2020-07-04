using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MBT;

namespace MBTExample
{
    [AddComponentMenu("")]
    [MBTNode("Example/Move Navmesh Agent")]
    public class MoveNavmeshAgent : Leaf
    {
        public TransformReference destination;
        public NavMeshAgent agent;
        public float stopDistance = 2f;
        public float updateTime = 1f;
        private float time = 0;

        public override void OnEnter()
        {
            time = 0;
            agent.isStopped = false;
            agent.SetDestination(destination.Value.position);
        }
        
        public override NodeResult Execute()
        {
            time += Time.deltaTime;
            if (time > updateTime)
            {
                time = 0;
                agent.SetDestination(destination.Value.position);
            }
            // Check if path is ready
            if (agent.pathPending)
            {
                return NodeResult.running;
            }
            // Check if agent is very close to destination
            if (agent.remainingDistance < stopDistance)
            {
                return NodeResult.success;
            }
            // Check if there is any path
            if (agent.hasPath)
            {
                return NodeResult.running;
            }
            // By default return failure
            return NodeResult.failure;
        }

        public override void OnExit()
        {
            agent.isStopped = true;
            // agent.ResetPath();
        }
    }
}
