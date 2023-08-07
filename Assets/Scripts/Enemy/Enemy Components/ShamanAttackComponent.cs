using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Enemies
{

    public class ShamanAttackComponent : MonoBehaviour
    {
        [Header("Attack Parameters")]
        public float attackRange = 50.0f;
        public float damage = 5.0f;
        public float timeOfCharge = 1.0f;
        public float timeofRecuperation = 0.5f;
        public GameObject vfxFeedback;
        public LayerMask layer;

        [Header("Projectile Parameters")]
        public float projectileLifeTime =1.5f;
        public float angleTrajectory = 45.0f;
        public float radiusOfAttack = 4.0f;

        [SerializeField] private GameObject m_projectileThrow;

        private float m_timerOfCharge;
        private float m_timerOfRecuperation;

        private Transform m_targetTransform;
        private NpcHealthComponent m_npcHealthComponent;
    


        void Start()
        {
            m_npcHealthComponent = GetComponent<NpcHealthComponent>();
            m_npcHealthComponent.destroyEvent += OnDeath;
            m_targetTransform = m_npcHealthComponent.target;

        }

        // Update is called once per frame
        void Update()
        {
            if (m_npcHealthComponent.npcState == NpcState.MOVE && Vector3.Distance(m_targetTransform.position, transform.position) < attackRange)
            {
                m_npcHealthComponent.npcState = NpcState.PREP_ATTACK;
               // vfxFeedback.SetActive(true);
                return;
            }

            if (m_npcHealthComponent.npcState == NpcState.PREP_ATTACK)
            {
                if (m_timerOfCharge > timeOfCharge)
                {
                    m_npcHealthComponent.npcState = NpcState.ATTACK;
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

            if (m_npcHealthComponent.npcState == NpcState.RECUPERATION)
            {
                if (m_timerOfRecuperation > timeofRecuperation)
                {
                    m_npcHealthComponent.npcState = NpcState.MOVE;
                    timeofRecuperation = 0;
                }
                else
                {
                    m_timerOfRecuperation += Time.deltaTime;
                }
                return;
            }
        }

        public void Attack()
        {
            GameObject projectileInstance = Instantiate(m_projectileThrow, transform.position, Quaternion.identity);
            ProjectileCurveData data = new ProjectileCurveData();
            data.damage = damage;
            data.destination = m_targetTransform.position;
            data.angleTrajectory = angleTrajectory;
            data.lifetime = projectileLifeTime;
            data.target = m_targetTransform;
            data.radiusOfAttack = radiusOfAttack;
            RaycastHit hit = new RaycastHit();
            if(Physics.Raycast(m_targetTransform.position,Vector3.down,out hit,10, layer))
            {
                data.destination = hit.point;
            }

            ProjectileMortar m_projectileMortar = projectileInstance.GetComponent<ProjectileMortar>();
            m_projectileMortar.InitProjectile(data);
            m_npcHealthComponent.npcState = NpcState.RECUPERATION;
        }

        public void OnDeath(Vector3 direction, float power)
        {
            this.enabled = false;
        }
    }
}
