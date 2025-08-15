using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace GuerhoubaGames.Enemies
{
    public class Enemy : MonoBehaviour
    {

        public ObjectState state ;
        [SerializeField] private AgentStat m_agentStat = new AgentStat();
        [SerializeField] private Transform m_target;
        [SerializeField] private float m_speedThreshold;
        [SerializeField] private GameObject m_ExperiencePrefab;
        [SerializeField] private int m_xpDrop = 1;
        [SerializeField] private bool m_debugActive = false;
        [SerializeField] private GameObject m_hitVfx;
        private HealthSystem m_healthSystem;
        private ArmorSystem m_armorSystem;
        private EnemyManager m_enemyManager;
        private HealthManager m_healthManager;
        private NavMeshAgent m_navAgent;
        private bool m_isDestroy;
        private Rigidbody m_rigidbody;
        private bool m_activeBehavior = true;
        private float distanceRefresh = 30f;
        [SerializeField] private float m_TimeBtwRefresh;
        private float tempsEcouleRefresh = 0;
        public Animator myAnimator;
        [SerializeField] private bool m_ActiveDeathAnim;
        private void Start()
        {
            state = new ObjectState();
            m_healthSystem = new HealthSystem();
            m_armorSystem = new ArmorSystem();
          //  myAnimator = this.GetComponent<Animator>();
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
            if(!state.isPlaying)
            {
                if (m_navAgent.isActiveAndEnabled) m_navAgent.isStopped = true;
                m_rigidbody.isKinematic = true;

                return;
            }else
            {
                if(m_navAgent.isActiveAndEnabled) m_navAgent.isStopped = false;
                m_rigidbody.isKinematic = false;

            }
            if(!m_debugActive)
            {
                if(Vector3.Distance(transform.position, m_target.position) > distanceRefresh)
                {
                    tempsEcouleRefresh += Time.deltaTime;
                }
                else
                {
                    if (m_navAgent.enabled && m_activeBehavior)
                    {
                        m_navAgent.destination = m_target.position;
                    }
                }
                if(tempsEcouleRefresh > m_TimeBtwRefresh)
                {
                    if (m_navAgent.enabled && m_activeBehavior)
                    {
                        m_navAgent.destination = m_target.position;
                    }
                    tempsEcouleRefresh = 0;
                }
                        
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

        public void HitEnemy(float damage, Vector3 direction,float power, int element)
        {
            Instantiate(m_hitVfx, transform.position, Quaternion.identity);
            GlobalSoundManager.PlayOneShot(12, transform.position);
            myAnimator.SetTrigger("TakeDamage");
            damage = m_armorSystem.ApplyArmor(damage, m_agentStat.armor);
            m_healthSystem.ChangeCurrentHealth(-damage);
            Vector3 positionOnScreen = transform.position;
            m_healthManager.CallDamageEvent(positionOnScreen, damage, element);


            if (m_healthSystem.health > 0) return;
           
            GetDestroy(direction, power);
        }

        public void UpdateGameState(bool newState)
        {
            state.isPlaying = newState;
            if(newState)
            {
                m_navAgent.isStopped = newState;
                myAnimator.speed = 1;
               // myAnimator.enabled = newState;
            }
            else
            {
                m_navAgent.isStopped = newState;
                myAnimator.speed = 0;
              //  myAnimator.enabled = newState;
            }
        }

        public bool IsDestroing()
        {
            return m_isDestroy;
        }
        private void GetDestroy(Vector3 direction,float power)
        {
            if(m_ActiveDeathAnim)
            {
                m_navAgent.isStopped = true;
                myAnimator.SetTrigger("Death");
            }
            else
            {
                m_navAgent.enabled = false;
                float magnitude = Random.Range(0.5f, 1.0f);
                m_rigidbody.isKinematic = false;
                m_rigidbody.constraints = RigidbodyConstraints.None;
                m_rigidbody.AddForce(direction.normalized * power, ForceMode.Impulse);
            }

            for(int i = 0; i < m_xpDrop; i++)
            {
                Instantiate(m_ExperiencePrefab, transform.position, transform.rotation);
            }


            m_isDestroy = true;

            Debug.Log("Call False on refresh serie !");
            StartCoroutine(Death());
        }

        private IEnumerator Death()
        {
            yield return new WaitForSeconds(1);
            //m_enemyManager.DestroyEnemy(this);
        }
    }
}