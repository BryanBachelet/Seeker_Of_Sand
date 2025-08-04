using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuerhoubaGames.AI
{
    public class WaitIdle : ActionNode
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
            return State.SUCCESS;
        }

    }
}