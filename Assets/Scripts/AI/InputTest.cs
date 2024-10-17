using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.AI;

public class InputTest : DecoratorNode
{

    protected override void OnStart()
    {
        
    }

    protected override void OnStop()
    {
       
    }

    protected override State OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("Test;");
            return State.SUCCESS;
        }

        return State.FAILURE;
    }

   
}
