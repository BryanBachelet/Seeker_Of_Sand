using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.AI;

public class CallEvent : ActionNode
{
    protected override void OnStart()
    {

    }

    protected override void OnStop()
    {
        
    }

    protected override State OnUpdate()
    {
        if (blackboard.event1 != null)
        {
            blackboard.event1?.Invoke();
            return State.SUCCESS;
        }
        else
        {
            return State.FAILURE;
        }

    }
}
