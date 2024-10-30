using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuerhoubaGames.AI
{
    public class IsAttacking : DecoratorNode
    {
        [Tooltip("Active the check if the attack allow the movemement ")]
        public bool activeTestMovingState;
        public bool onlyOneTest;
        private bool isLock;
        protected override void OnStart()
        {
          
        }

        protected override void OnStop()
        {
            child.StopNode();
        }

        protected override State OnUpdate()
        {
            if (isLock)
            {
                State state = child.Evaluate();
                if (state == State.FAILURE || state == State.SUCCESS)
                {
                    isLock = false;
                }
                return state;
            }

            if (agent.state == Enemies.NpcState.ATTACK || activeTestMovingState && agent.attackComponent.currentAttackData.isStopMovingAtPrep)
            {
                
             
                return State.FAILURE;
            }
            else
            {
                State state = child.Evaluate();
                if (onlyOneTest && state == State.RUNNING)
                {
                    isLock = true;
                }
             
                return state;
            }
        }
    }
}
