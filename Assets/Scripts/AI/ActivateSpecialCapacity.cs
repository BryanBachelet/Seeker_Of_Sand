using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuerhoubaGames.AI
{
    public class ActivateSpecialCapacity : ActionNode
    {
        public int indexCapacitySpecial = 0;
        private short isSucces = 0; //  If equal 0 is Running, equal 1 is Succes , equal -1 is Fail

        protected override void OnStart()
        {
            agent.specialCapacities.ActivateSpecialCapacity(indexCapacitySpecial);
            agent.specialCapacities.OnFinish += IsFinish;
            isSucces = 0;
        }

        protected override void OnStop()
        {
            agent.specialCapacities.OnFinish -= IsFinish;
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

            agent.specialCapacities.OnFinish -= IsFinish;

        }


    }
}