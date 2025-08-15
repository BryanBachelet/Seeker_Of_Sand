using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GuerhoubaGames.Enemies
{

    public class ShamanAttackComponent : MonoBehaviour
    {
        [Header("Attack Parameters")]
        public float attackRange = 50.0f;
        public float minAttackRange = 10.0f;
        public float damage = 5.0f;
        public float timeOfCharge = 1.0f;
        public float timeofRecuperation = 0.5f;
        public GameObject vfxFeedback;
        public LayerMask layer;

        [Header("Projectile Parameters")]
        public float projectileLifeTime = 1.5f;
        public float angleTrajectory = 45.0f;
        public float radiusOfAttack = 4.0f;
        public Vector3 attackScale = Vector3.zero;

        [SerializeField] private GameObject m_projectileThrow;

        private float m_timerOfCharge;
        private float m_timerOfRecuperation;

        private Transform m_targetTransform;
        private NpcHealthComponent m_npcHealthComponent;
        private NpcMouvementComponent m_npcMvtComponent;
        private NavMeshAgent m_navMeshAgent;
        private NpcMetaInfos m_npcMeta;

        private Animator animator;

        [SerializeField] private UnityEngine.VFX.VisualEffect m_signAttack;
        private bool m_SignAttackReset = true;

        void Start()
        {
            m_npcHealthComponent = GetComponent<NpcHealthComponent>();
            m_npcMvtComponent = GetComponent<NpcMouvementComponent>();
            m_npcMeta = GetComponent<NpcMetaInfos>();
            m_navMeshAgent = GetComponent<NavMeshAgent>();
            m_npcHealthComponent.destroyEvent += OnDeath;
            m_targetTransform = m_npcHealthComponent.targetData.target;
            animator = GetComponentInChildren<Animator>();

        }

        // Update is called once per frame
        void Update()
        {
            
            if (m_npcMeta.state == NpcState.MOVE && Vector3.Distance(m_targetTransform.position, transform.position) < minAttackRange)
            {
                Vector3 direction = transform.position - m_targetTransform.position;
                Vector3 position = m_targetTransform.position + direction.normalized * minAttackRange * 2;
                NavMeshHit hit = new NavMeshHit();
                NavMesh.SamplePosition(position, out hit, 1000, NavMesh.AllAreas);
                m_navMeshAgent.SetDestination(hit.position);
                return;
            }

            if (m_npcMeta.state == NpcState.MOVE && Vector3.Distance(m_targetTransform.position, transform.position) < attackRange)
            {
                m_npcMeta.state = NpcState.PREP_ATTACK;
                m_navMeshAgent.isStopped = true;
                // vfxFeedback.SetActive(true);
                return;
            }

            if (m_npcMeta.state == NpcState.PREP_ATTACK)
            {
                transform.LookAt(m_targetTransform);
                if (m_timerOfCharge - 0.25f < timeOfCharge && m_SignAttackReset)
                {
                    m_signAttack.Play();
                    m_SignAttackReset = false;

                }
                if (m_timerOfCharge > timeOfCharge)
                {
                    m_npcMeta.state = NpcState.ATTACK;
                    m_timerOfCharge = 0;
                    Attack();
                    // vfxFeedback.SetActive(false);
                }
                else
                {
                    m_timerOfCharge += Time.deltaTime;
                    //  vfxFeedback.transform.position = m_targetTransform.position;
                }
                return;
            }

            if (m_npcMeta.state == NpcState.RECUPERATION)
            {
                transform.LookAt(m_targetTransform);
                if (m_timerOfRecuperation > timeofRecuperation)
                {
                    m_npcMeta.state = NpcState.MOVE;
                    if (Vector3.Distance(m_targetTransform.position, transform.position) < minAttackRange)
                    {
                        Vector3 direction = transform.position - m_targetTransform.position;
                        Vector3 position = m_targetTransform.position + direction.normalized * minAttackRange * 2;
                        NavMeshHit hit = new NavMeshHit();
                        NavMesh.SamplePosition(position, out hit, 1000, NavMesh.AllAreas);
                        m_navMeshAgent.SetDestination(hit.position);

                    }
                    m_navMeshAgent.SetDestination(m_targetTransform.position);  
                    timeofRecuperation = 0;
                    m_navMeshAgent.isStopped = false;
                    m_SignAttackReset = true;
                }
                else
                {
                    m_timerOfRecuperation += Time.deltaTime;
                }
                return;
            }
        }


        public void LateUpdate()
        {
            if (m_npcHealthComponent.m_hasChangeTarget)
            {
                m_targetTransform = m_npcHealthComponent.targetData.target;
                m_npcHealthComponent.m_hasChangeTarget = false;
            }
        }
        public void Attack()
        {
            //animator.SetTrigger("Attacking");
            GameObject projectileInstance = Instantiate(m_projectileThrow, m_targetTransform.position, Quaternion.identity);
     
            AttackTrainingArea attackObjectArea = projectileInstance.GetComponent<AttackTrainingArea>();
            attackObjectArea.playerTransform = m_targetTransform;

            m_npcMeta.state = NpcState.RECUPERATION;
            //animator.ResetTrigger("Attacking");
        }

        public void AttackOnPlayer()
        {
            //animator.SetTrigger("Attacking");
            GameObject projectileInstance = Instantiate(m_projectileThrow, transform.position, Quaternion.identity);
            ProjectileCurveData data = new ProjectileCurveData();
            data.damage = damage;
            data.destination = m_targetTransform.position;
            data.angleTrajectory = angleTrajectory;
            data.lifetime = projectileLifeTime;
            data.target = m_targetTransform;
            data.radiusOfAttack = radiusOfAttack;
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(m_targetTransform.position, Vector3.down, out hit, 10, layer))
            {
                data.destination = hit.point;
            }
            ProjectileMortar m_projectileMortar = projectileInstance.GetComponent<ProjectileMortar>();
            m_projectileMortar.InitProjectile(data);
            m_npcMeta.state = NpcState.RECUPERATION;
            //animator.ResetTrigger("Attacking");
        }
        public void OnDeath(Vector3 direction, float power)
        {
            this.enabled = false;
        }
    }
}
