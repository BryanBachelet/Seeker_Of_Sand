using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GuerhoubaGames.AI
{
    public class WaitNode : ActionNode
    {
        public float duration = 1;
        private float m_startTime;

        protected override void OnStart()
        {
            m_startTime = Time.time;
        }

        protected override void OnStop()
        {
           
        }

        protected override State OnUpdate()
        {
           if(Time.time - m_startTime < duration )
            {
                return State.RUNNING;
            }

            return State.SUCCESS;
        }

    }
}
