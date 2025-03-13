using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuerhoubaGames.AI
{
    public class IsAttackRange : DecoratorNode
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

            if (agent.attackComponent.GetAttackRange(indexAttack) >  Vector3.Distance(agent.transform.position,blackboard.moveToObject.transform.position))
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
