using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private Transform m_target;
        [SerializeField] private float m_minSpeed = 3.0f;
        [SerializeField] private float m_maxSpeed = 4.0f;
        private EnemyManager m_enemyManager;
        private NavMeshAgent m_navAgent;
        private bool m_isDestroy;
        private Rigidbody m_rigidbody;


        private void Start()
        {
            
            m_navAgent = GetComponent<NavMeshAgent>();
            m_navAgent.speed = Random.Range(m_minSpeed, m_maxSpeed);
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
            float magnitude = Random.Range(0.5f, 1.0f);
            m_rigidbody.isKinematic = false;
            m_rigidbody.constraints = RigidbodyConstraints.None;
            m_rigidbody.AddForce(direction.normalized  * power, ForceMode.Impulse);
            StartCoroutine(Death());
        }

        private IEnumerator Death()
        {
            m_isDestroy = true;
            yield return new WaitForSeconds(2);
            m_enemyManager.DestroyEnemy(this);
        }
    }
}