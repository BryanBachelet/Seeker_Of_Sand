using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemies;

namespace GuerhoubaGames.AI
{

    public class IsState : DecoratorNode
    {
        public NpcState stateToTest;
        public bool inverse;


        protected override void OnStart()
        {
            
        }

        protected override void OnStop()
        {
           
        }

        protected override State OnUpdate()
        {
            if(!inverse)
            {
                bool result = agent.state == stateToTest;
                if (result)
                {
                    return child.Evaluate();
                }
                else
                {
                    return State.FAILURE;
                }
            }
            else
            {
                bool result = agent.state != stateToTest;
                if (result)
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
}