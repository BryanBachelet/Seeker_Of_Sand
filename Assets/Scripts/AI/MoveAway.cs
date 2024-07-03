using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GuerhoubaGames.AI
{
    public class MoveAway : ActionNode
    {
        public float outSideRange;

        protected override void OnStart()
        {
            agent.state = Enemies.NpcState.MOVE;
            agent.moveComponent.awayDistance = outSideRange;
            agent.moveComponent.isGoingAway = true;
            agent.moveComponent.ComputeFleePosition(true);
        }

        protected override void OnStop()
        {
            agent.moveComponent.isGoingAway = false;
        }

        protected override State OnUpdate()
        {
            if (agent.moveComponent.IsOutsideRange(outSideRange) && agent.moveComponent.IsFacingTarget())
            {
                return State.SUCCESS;
            }
            else
            {
                return State.RUNNING;
            }

        }

    }
}