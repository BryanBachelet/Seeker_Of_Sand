using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuerhoubaGames.AI
{
    public class CanAttack : DecoratorNode
    {
        protected override void OnStart()
        {
           
        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            if (agent.attackComponent.IsGeneralRecoveringFromAttackActive() )
            {
       
                return State.FAILURE;
            }
            else
            {
                State state = child.Evaluate();
                return state;
            }
        }
    }
}
