using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GuerhoubaGames.AI
{
    public class IsSpecialCapacityActive : DecoratorNode
    {
        public int indexSpecialCapacity;

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
                if (state == State.FAILURE && debugTest)
                {
                    Debug.LogError(child.name + " has failed");
                }
                if (state == State.FAILURE || state == State.SUCCESS)
                {
                    isLock = false;
                }
                return state;
            }


            if (blackboard.IsSpecialCapacityCall && blackboard.indexSpecialCapacityCall == indexSpecialCapacity)
            {
                State state = child.Evaluate();
                if (state == State.FAILURE && debugTest)
                {
                    Debug.LogError(child.name + " has failed");
                }
                if (onlyOneTest && state == State.RUNNING)
                {
                    isLock = true;
                    blackboard.IsSpecialCapacityCall = false;
                    blackboard.indexSpecialCapacityCall = -1;
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