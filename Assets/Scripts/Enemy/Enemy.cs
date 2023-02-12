using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace Enemies
{
    public class Enemy : MonoBehaviour
    {
        
        [SerializeField] private AgentStat m_agentStat = new AgentStat();
        [SerializeField] private Transform m_target;
        [SerializeField] private float m_speedThreshold;
        [SerializeField] private GameObject m_ExperiencePrefab;
        private HealthSystem m_healthSystem = new HealthSystem();
        private EnemyManager m_enemyManager;
        private HealthManager m_healthManager;
        private NavMeshAgent m_navAgent;
        private bool m_isDestroy;
        private Rigidbody m_rigidbody;

        private void Start()
        {
            m_healthSystem = new HealthSystem();
            m_navAgent = GetComponent<NavMeshAgent>();
            m_navAgent.speed = Random.Range(m_agentStat.speed -m_speedThreshold, m_agentStat.speed - m_speedThreshold);
            m_rigidbody = GetComponent<Rigidbody>();
            m_healthSystem.Setup(m_agentStat.healthMax);

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

        public void SetManager(EnemyManager manager, HealthManager healthManager)
        {
            m_enemyManager = manager;
            m_healthManager = healthManager;
        }

        public void HitEnemy(float damage, Vector3 direction,float power)
        {
            m_healthSystem.ChangeCurrentHealth(-damage);
            m_healthManager.CallDamageEvent(transform.position + Vector3.up * 1.5f,damage);


            if (m_healthSystem.health != 0) return;
           
            GetDestroy(direction, power);
        }

       

        public bool IsDestroing()
        {
            return m_isDestroy;
        }
        private void GetDestroy(Vector3 direction,float power)
        {
            m_navAgent.enabled = false;
            float magnitude = Random.Range(0.5f, 1.0f);
            m_rigidbody.isKinematic = false;
            m_rigidbody.constraints = RigidbodyConstraints.None;
            Instantiate(m_ExperiencePrefab, transform.position, transform.rotation);
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