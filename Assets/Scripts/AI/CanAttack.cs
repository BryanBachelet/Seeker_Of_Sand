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
                if (debugTest && !once)
                {
                    once = true;
                    Debug.LogError("Stop Attack");
                }
                return State.FAILURE;
            }
            else
            {
                if (debugTest) once = false;
                State state = child.Evaluate();
                return state;
            }
        }
    }
}
