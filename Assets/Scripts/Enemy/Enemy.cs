using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private Transform m_target;
        private EnemyManager m_enemyManager;
        private NavMeshAgent m_navAgent;
        private bool m_isDestroy;

        private void Start()
        {
            m_navAgent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            m_navAgent.destination = m_target.position;
        }

        public Enemy(Transform targetTranform, EnemyManager manager)
        {
            m_target = targetTranform;
            m_enemyManager = manager;
        }

        public void SetTarget(Transform targetTranform)
        {
            m_target = targetTranform;
        }

        public void SetManager(EnemyManager manager)
        {
            m_enemyManager = manager;
        }


        public bool IsDestroing()
        {
            return m_isDestroy;
        }
        public void GetDestroy()
        {
            m_isDestroy = true;
            m_enemyManager.DestroyEnemy(this);
        }
    }
}