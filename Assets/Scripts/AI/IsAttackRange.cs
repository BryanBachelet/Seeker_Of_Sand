using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuerhoubaGames.AI
{
    public class IsAttackRange : DecoratorNode
    {
        

        protected override void OnStart()
        {
           
        }

        protected override void OnStop()
        {
          
        }

        protected override State OnUpdate()
        {
            if (agent.attackComponent.GetAttackRange() >  Vector3.Distance(agent.transform.position,blackboard.moveToObject.transform.position))
            {
                child.Evaluate();
                return State.RUNNING;
            }
            else
            {
                return State.FAILURE;
            }

        }
    }
}
