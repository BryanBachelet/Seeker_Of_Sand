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
        private Rigidbody m_rigidbody;


        private void Start()
        {
            m_navAgent = GetComponent<NavMeshAgent>();
            m_rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if(m_navAgent.enabled)
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
        public void GetDestroy(Vector3 direction,float power)
        {
            m_navAgent.enabled = false;
            float magnitude = Random.Range(0, 1); 
            m_rigidbody.AddForce(direction.normalized* magnitude * power, ForceMode.Impulse);
            StartCoroutine(Death());
        }

        private IEnumerator Death()
        {
            yield return new WaitForSeconds(2);
            m_isDestroy = true;
            m_enemyManager.DestroyEnemy(this);
        }
    }
}