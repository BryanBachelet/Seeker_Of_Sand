using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies
{
    // Create a struct of data for Custom Movement
    public struct CustomMovementData
    {
        public NavMeshAgent agent;
        public Transform transform;
        public NavMeshPath path;
        public float speedMax;
    }

    [System.Serializable]
    public class NpcCustomMouvement : ScriptableObject
    {
        protected NavMeshPath m_path;
        protected Transform m_transform;
        protected NavMeshAgent m_agent;
        protected float m_speedMax;

        public virtual void SetupMove(CustomMovementData customMovementData)
        {
            m_agent = customMovementData.agent;
            m_transform = customMovementData.transform;
            m_speedMax = customMovementData.speedMax;
            m_path = customMovementData.path;
        }
        public virtual void Move(NavMeshPath path)
        {

        }

        public virtual bool CanStopMoving()
        {
            return false;
        }


    }
}