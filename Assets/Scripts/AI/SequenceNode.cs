using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuerhoubaGames.AI
{
    /// <summary>
    /// Sequence class allow the AI to do multiple tasks in a row.
    /// </summary>
   // [CreateAssetMenu(fileName = "Sequence", menuName = "BehaviorTree/Sequence", order = 2)]
    public class SequenceNode : CompositeNode
    {
        int current = 0;
        protected override void OnStart()
        {
            current = 0;
        }

        protected override void OnStop()
        {
            
        }

        protected override State OnUpdate()
        {
            Node child = children[current];
            switch (child.Evaluate())
            {
                case State.RUNNING:
                    return State.RUNNING;

                case State.FAILURE:
                    return State.FAILURE;
           
                case State.SUCCESS:
                    current++;
                    break;
            }

            return current == children.Count ? State.SUCCESS : State.RUNNING;
        }
    }
}
