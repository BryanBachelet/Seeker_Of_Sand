using GuerhoubaGames.AI;
using UnityEngine;

public class IsHealthInferiorTo : DecoratorNode
{
    [Range(0, 100)] public float healthCondition;
    protected override void OnStart()
    {

    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        if(agent.m_healthComponent.GetCurrentLifePercent() < healthCondition *0.01f )
        {
            return child.Evaluate();
        }else
        {
            return State.FAILURE;
        }
    }

}
