using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GuerhoubaGames.AI
{
    public class CanLaunchSpecialCapacity : DecoratorNode
    {
        public int indexCapacitySpecial;
        private bool isLock;

        protected override void OnStart()
        {
            
        }

        protected override void OnStop()
        {
            
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


            // Check if the special capacity can be launch
            if (agent.specialCapacities.CanActivateSpecialSkill(indexCapacitySpecial))
            {
                State state = child.Evaluate();
                if ( state == State.RUNNING)
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