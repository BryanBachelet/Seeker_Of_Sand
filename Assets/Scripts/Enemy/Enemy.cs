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
        [SerializeField] private int m_xpDrop = 1;
        [SerializeField] private bool m_debugActive = false;
        private HealthSystem m_healthSystem;
        private ArmorSystem m_armorSystem;
        private EnemyManager m_enemyManager;
        private HealthManager m_healthManager;
        private NavMeshAgent m_navAgent;
        private bool m_isDestroy;
        private Rigidbody m_rigidbody;
        private bool m_activeBehavior = true;



        private void Start()
        {
            m_healthSystem = new HealthSystem();
            m_armorSystem = new ArmorSystem();
            if(!m_debugActive)
            {
                m_navAgent = GetComponent<NavMeshAgent>();
                m_navAgent.speed = Random.Range(m_agentStat.speed - m_speedThreshold, m_agentStat.speed - m_speedThreshold);
            }
            else
            {
                m_healthManager = GameObject.Find("Enemy Manager").GetComponent<HealthManager>();
            }
            m_rigidbody = GetComponent<Rigidbody>();
            m_healthSystem.Setup(m_agentStat.healthMax);
            m_activeBehavior = true;
        }

        private void Update()
        {
            if(!m_debugActive)
            {
                if (m_navAgent.enabled && m_activeBehavior)
                    m_navAgent.destination = m_target.position;
            }

        }
    
        public void ChangeActiveBehavior(bool state) { m_activeBehavior = state; }

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
            damage = m_armorSystem.ApplyArmor(damage, m_agentStat.armor);
            m_healthSystem.ChangeCurrentHealth(-damage);
            m_healthManager.CallDamageEvent(transform.position + Vector3.up * 1.5f,damage);


            if (m_healthSystem.health > 0) return;
           
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
            for(int i = 0; i < m_xpDrop; i++)
            {
                Instantiate(m_ExperiencePrefab, transform.position, transform.rotation);
            }

            m_rigidbody.AddForce(direction.normalized  * power, ForceMode.Impulse);
            m_isDestroy = true;
            StartCoroutine(Death());
        }

        private IEnumerator Death()
        {
            yield return new WaitForSeconds(2);
            m_enemyManager.DestroyEnemy(this);
        }
    }
}