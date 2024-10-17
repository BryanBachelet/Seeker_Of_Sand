using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuerhoubaGames.AI
{
    /// <summary>
    /// This class is use in the behavior tree system as a base class for condition to validate
    /// </summary>
    public class Decorator
    {
        public Node parent;
        public bool isSkipable = false;

        public virtual bool Evaluate()
        {
            return true;
        }

    }

    /// <summary>
    /// AINode is the base class for most of the behavior tree elements
    /// </summary>
    public abstract class Node : ScriptableObject
    {
        public enum State
        {
            RUNNING,
            FAILURE,
            SUCCESS
        }
        [HideInInspector] public State state = State.RUNNING;
        [HideInInspector] public bool started = false;
        [HideInInspector] public string guid;
        [HideInInspector] public Vector2 position;
        [HideInInspector] public Blackboard blackboard;
        [HideInInspector] public Enemies.NpcMetaInfos agent;
        [TextArea] public string description;
        public bool debugTest;

        protected bool once;

        public virtual State Evaluate()
        {


            if (!started)
            {
                OnStart();
                started = true;
                once = false;
            }
            state = OnUpdate();
            if (state == State.FAILURE || state == State.SUCCESS)
            {
                OnStop();
                started = false;
                if (debugTest && !once)
                {
                    once = true;
                    Debug.LogError("Stop Attack");
                }
            }
            return state;
        }
        
        public void StopNode()
        {

         
            OnStop();
            started = false;
        }

        protected abstract void OnStart();
        protected abstract void OnStop();
        protected abstract State OnUpdate();

        public virtual Node Clone()
        {
            return Instantiate(this);
        }

    }



}
