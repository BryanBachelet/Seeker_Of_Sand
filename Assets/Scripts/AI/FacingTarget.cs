using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuerhoubaGames.AI
{


    public class FacingTarget : ActionNode
    {
        protected override void OnStart()
        {
            agent.state = Enemies.NpcState.IDLE;

        }

        protected override void OnStop()
        {
            
        }

        protected override State OnUpdate()
        {
            if (agent.moveComponent.IsFacingTarget())
            {
                return State.SUCCESS;
            }else
            {
                agent.moveComponent.RotateToTarget();
                return State.RUNNING;
            }
        }
    }
}