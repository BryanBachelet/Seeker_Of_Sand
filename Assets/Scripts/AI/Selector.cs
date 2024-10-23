using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuerhoubaGames.AI
{
    /// <summary>
    ///  Selector class allow the AI to choose between possible depending if they condition are valid or not
    /// </summary>

    public class Selector : CompositeNode
    {
        Node childChoose;
        protected override void OnStart()
        {

        }

        protected override void OnStop()
        {
            
        }

        protected override State OnUpdate()
        {
            foreach (Node child in children)
            {

                State nodeState = child.Evaluate();

                switch (nodeState)
                {
                    case State.RUNNING:
                        ManageChild(child);
                        return State.RUNNING;
                        break;
                    case State.FAILURE:

                        break;
                    case State.SUCCESS:
                        ManageChild(child);
                        return State.SUCCESS;
                        break;
                    default:
                        break;
                }

            }

            return State.FAILURE;

        }

        public void ManageChild(Node child)
        {
           
            if (childChoose != child)
            {
             
                if (childChoose)
                    childChoose.StopNode();
                childChoose = child;
            }
        }
    }
}

