using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Enemies
{
    public class TankAttackComponent : MonoBehaviour
    {
        [Header("Attack Parameter")]
        public float radiusOfAttack = 4;
        public float damage = 5.0f;
        public float timeOfCharge = 1.0f;
        public float timeofRecuperation = 0.5f;

        public GameObject vfxRangeAttack;

        private float m_timerOfCharge;
        private float m_timerOfRecuperation;

        public Transform m_monsterBodyTransform;
        private Transform m_targetTransform;
        private NpcHealthComponent m_npcHealthComponent;


        public void Start()
        {
            m_npcHealthComponent = GetComponent<NpcHealthComponent>();
            m_npcHealthComponent.destroyEvent += OnDeath;
            m_targetTransform = m_npcHealthComponent.target;
        }

        public void Update()
        {
            if (m_npcHealthComponent.npcState == NpcState.MOVE && Vector3.Distance(m_targetTransform.position, m_monsterBodyTransform.position) < radiusOfAttack)
            {
                m_npcHealthComponent.npcState = NpcState.PREP_ATTACK;
                vfxRangeAttack.SetActive(true);
            }

            if (m_npcHealthComponent.npcState == NpcState.PREP_ATTACK)
            {
                if (m_timerOfCharge > timeOfCharge)
                {
                    m_npcHealthComponent.npcState = NpcState.ATTACK;
                    m_timerOfCharge = 0;
                    AttackTank();
                    vfxRangeAttack.SetActive(false);
                }
                else
                {
                    m_timerOfCharge += Time.deltaTime;
                }
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
            }

        }

        public void AttackTank()
        {
            if (Vector3.Distance(m_targetTransform.position, m_monsterBodyTransform.position) < radiusOfAttack)
            {
                if (m_targetTransform.tag == "Player")
                {
                    m_targetTransform.GetComponent<health_Player>().GetDamageLeger(damage);
                   
                }
            }
            m_npcHealthComponent.npcState = NpcState.RECUPERATION;


        }

        public void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(m_monsterBodyTransform.position, radiusOfAttack);
        }

        public void OnDeath( Vector3 direction,float power)
        {
            this.enabled = false;
        }



    }
}
