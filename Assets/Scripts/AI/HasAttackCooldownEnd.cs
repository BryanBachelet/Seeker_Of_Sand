using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuerhoubaGames.AI
{
    public class HasAttackCooldownEnd : DecoratorNode
    {
        public int indexAttack;

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


            if (!agent.attackComponent.IsAttackOnCooldown(indexAttack))
            {
          
                State state = child.Evaluate();
                if (onlyOneTest && state == State.RUNNING)
                {
                    isLock = true;
                }

                return state;

            }
            else
            {
              
                return State.FAILURE;
            }
        }


    }
}
