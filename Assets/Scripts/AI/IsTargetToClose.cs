using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuerhoubaGames.AI
{
    public class IsTargetToClose : DecoratorNode
    {

        public float minDistance = 0;
        public bool isCheckingOrientation;

        protected override void OnStart()
        {
            
        }

        protected override void OnStop()
        {
            child.StopNode();
        }

        protected override State OnUpdate()
        {
            if (DistanceTest())
            {
                child.Evaluate();
                return State.RUNNING;
            }
            else
            {
                return State.FAILURE;
            }
        }

        private bool DistanceTest()
        {
            if (isCheckingOrientation)
            {
                return  !agent.moveComponent.IsOutsideRange(minDistance) || !agent.moveComponent.IsFacingTarget();
            }
            else
            {
                return !agent.moveComponent.IsOutsideRange(minDistance);
            }

        }

    }
}
