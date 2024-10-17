using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace GuerhoubaGames.AI
{
    public class Attack : ActionNode
    {
        public int indexAttack = 0;
        public int sequenceIndex = 0;
        private short isSucces = 0; //  If equal 0 is Running, equal 1 is Succes , equal -1 is Fail
    
        protected override void OnStart()
        {

        
            if (agent.attackComponent.currentAttackState != GameEnum.AttackPhase.NONE)
            {
                isSucces = -1;

            
            }
            else
            {
                agent.state = Enemies.NpcState.ATTACK;
                agent.attackComponent.ActivePrepationAttack(indexAttack,sequenceIndex) ;
                agent.attackComponent.OnFinishAttack += IsFinish;
                isSucces = 0;
                Debug.Log("Start Attack : " + description);
       
            }
        }

        protected override void OnStop()
        {
            agent.attackComponent.OnFinishAttack -= IsFinish;
        }

        protected override State OnUpdate()
        {
            
            if (isSucces == 1)
                return State.SUCCESS;
            if (isSucces == -1)
                return State.FAILURE;



            return State.RUNNING;
        }

        private void IsFinish(bool result)
        {
            if (result)
                isSucces = 1;
            else
                isSucces = -1;
            if (debugTest)
            {
                Debug.LogError(" Test");
            }

        }
    }
}
