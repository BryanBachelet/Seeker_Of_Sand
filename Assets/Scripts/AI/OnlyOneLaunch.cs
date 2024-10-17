using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.AI;

public class OnlyOneLaunch : DecoratorNode
{
    private bool isAlreadyLaunch =false;
    protected override void OnStart()
    {

    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        if (!isAlreadyLaunch)
        {
            isAlreadyLaunch = true;
            return child.Evaluate();
        }
        else
        {
            return State.FAILURE;
        }

    }


}
