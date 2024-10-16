using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GuerhoubaGames.AI
{
    public class IsInAttackSequence : DecoratorNode
    {
        public int sequenceValidIndex;
        protected override void OnStart()
        {
            
        }

        protected override void OnStop()
        {
            
        }

        protected override State OnUpdate()
        {
            if (agent.attackComponent.isInAttackSequence && agent.attackComponent.sequenceIndex != sequenceValidIndex)
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