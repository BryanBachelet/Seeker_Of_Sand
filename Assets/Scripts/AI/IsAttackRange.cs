using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuerhoubaGames.AI
{
    public class IsAttackRange : DecoratorNode
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
            if (agent.attackComponent.GetAttackRange(indexAttack) >  Vector3.Distance(agent.transform.position,blackboard.moveToObject.transform.position))
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
