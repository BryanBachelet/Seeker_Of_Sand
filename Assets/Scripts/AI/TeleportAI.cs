using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GuerhoubaGames.AI
{


    public class TeleportAI : ActionNode
    {
        protected override void OnStart()
        {
            agent.state = Enemies.NpcState.MOVE;
            agent.moveComponent.isGoingAway = false;
        }

        protected override void OnStop()
        {
           
        }

        protected override State OnUpdate()
        {
            Vector3 direction = agent.transform.position - agent.moveComponent.targetData.baseTarget.position;
            agent.GetComponent<NavMeshAgent>().Warp(agent.moveComponent.targetData.baseTarget.position + direction.normalized*5);
            return State.SUCCESS;
        }

       
    }

}