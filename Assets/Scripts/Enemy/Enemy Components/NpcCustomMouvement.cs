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
        public Transform m_transform;
        public NavMeshPath m_path;
        public float m_speedMax;
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
            m_transform = customMovementData.m_transform;
            m_speedMax = customMovementData.m_speedMax;
            m_agent = customMovementData.agent;
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