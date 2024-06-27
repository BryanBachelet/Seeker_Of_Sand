using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.GameEnum;

namespace Enemies
{
    public class NpcAttackComponent : MonoBehaviour
    {
        [Header("Attack Objects")]
        public Transform baseTransform;
        public AttackEnemiesObject[] attackEnemiesObjectsArr = new AttackEnemiesObject[0];
        public Collider[] colliderAttackArray = new Collider[0];
        public GameObject[] projectileAttackArrray  = new GameObject[0];
        public GameObject[] deacalAttackArrray  = new GameObject[0];


        private AttackNPCData currentAttackData;
        private int currentAttackIndex;
        private float m_timer;
        private NpcMetaInfos m_npcMetaInfos;
        private NPCEnemiAnimation m_NPCEnemiAnimation;

      
        [Header("Attack Infos")]
        public AttackPhase currentAttackState;
        public bool isActiveDebug = false;

        // Event for each attack step
        public Action OnPrepAttack;
        public Action OnContactAttack;
        public Action OnRecoverAttack;
        public Action<bool> OnFinishAttack;

        #region MonoBehavior Functions
        public void Start()
        {
            m_npcMetaInfos = GetComponent<NpcMetaInfos>();
            m_NPCEnemiAnimation = GetComponent<NPCEnemiAnimation>();
            currentAttackState = AttackPhase.NONE;
            if (m_NPCEnemiAnimation)
            {
                OnPrepAttack += m_NPCEnemiAnimation.CallCloseAnimation;
                OnPrepAttack += m_NPCEnemiAnimation.CallAnimAttack;


                OnRecoverAttack += m_NPCEnemiAnimation.ResetAnimAttack;
                OnRecoverAttack += m_NPCEnemiAnimation.ResetCloseAnimation;
            }
        }

        public void Update()
        {
            UpdatePrepAttack();
            UpdateContactAttack();
            UpdateRecoverAttack();
        }

        #endregion

        public void CancelAttack()
        {
            if (!currentAttackData.isStunLockingAttack) return;

            OnFinishAttack?.Invoke(false);
            currentAttackState = AttackPhase.NONE;
        }

        #region  Active Phase Functions
        public void ActivePrepationAttack()
        {
            OnPrepAttack?.Invoke();
            currentAttackState = AttackPhase.PREP;
            deacalAttackArrray[currentAttackIndex].SetActive(true);
            if (currentAttackData.isStopMovingAtPrep) m_npcMetaInfos.state = NpcState.ATTACK;
            m_timer = 0.0f;

            if (isActiveDebug) Debug.Log($"Agent {transform.gameObject.name} is preparing to attack");
        }

        public void ActiveAttackContact()
        {
            OnContactAttack?.Invoke();
            currentAttackState = AttackPhase.CONTACT;
            colliderAttackArray[currentAttackIndex].gameObject.SetActive(true);
            m_timer = 0.0f;
            if (isActiveDebug) Debug.Log($"Agent {transform.gameObject.name} is attacking the target");

        }

        public void ActiveRecoverPhase()
        {
            OnRecoverAttack?.Invoke();
            currentAttackState = AttackPhase.RECOVERY;
            m_timer = 0.0f;
            if (isActiveDebug) Debug.Log($"Agent {transform.gameObject.name} is recoving from the attack launch");
        }

        #endregion

        #region  Finish Phase Functions
        public void FinishPreparationAttack()
        {
            deacalAttackArrray[currentAttackIndex].SetActive(false);
            ActiveAttackContact();
        }

        public void FinishContactAttack()
        {
            colliderAttackArray[currentAttackIndex].gameObject.SetActive(false);
            ActiveRecoverPhase();
        }

        public void FinishRecoverAttack()
        {
            OnFinishAttack?.Invoke(true);
            currentAttackState = AttackPhase.NONE;
            if (isActiveDebug) Debug.Log($"Agent {transform.gameObject.name} has finished to attack");
        }
        #endregion

        #region Update Phase Functions
        public void UpdatePrepAttack()
        {
            if (currentAttackState != AttackPhase.PREP) return;

            if (m_timer > currentAttackData.prepationTime)
            {
                FinishPreparationAttack();
            }
            else
            {
                m_timer += Time.deltaTime;
            }
        }

        public void UpdateContactAttack()
        {
            if (currentAttackState != AttackPhase.CONTACT) return;

            if (m_timer > currentAttackData.contactTime)
            {
                FinishContactAttack();
            }
            else
            {
                m_timer += Time.deltaTime;
            }
        }

        public void UpdateRecoverAttack()
        {
            if (currentAttackState != AttackPhase.RECOVERY) return;

            if (m_timer > currentAttackData.recoverTime)
            {
                FinishRecoverAttack();
            }
            else
            {
                m_timer += Time.deltaTime;
            }
        }
        #endregion

        #region Attack Data Function

        public float GetAttackRange() { return currentAttackData.attackRange; }

        #endregion

    }
}