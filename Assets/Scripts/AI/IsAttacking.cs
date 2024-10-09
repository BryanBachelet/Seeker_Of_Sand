using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuerhoubaGames.AI
{
    public class IsAttacking : DecoratorNode
    {
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

            if (agent.state == Enemies.NpcState.ATTACK)
            {
                
                if (onlyOneTest && state == State.RUNNING)
                {
                    isLock = true;
                }
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
