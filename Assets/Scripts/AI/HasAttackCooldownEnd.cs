using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuerhoubaGames.AI
{
    public class HasAttackCooldownEnd : DecoratorNode
    {
        public int indexAttack;
        protected override void OnStart()
        {
            
        }

        protected override void OnStop()
        {
            child.StopNode();
        }

        protected override State OnUpdate()
        {
            if (!agent.attackComponent.IsAttackOnCooldown(indexAttack))
            {
                return child.Evaluate();
            }
            else
            {
                return State.FAILURE;
            }
        }


    }
}
