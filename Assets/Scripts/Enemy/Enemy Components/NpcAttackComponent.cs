using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    public class NpcAttackComponent : MonoBehaviour
    {
        public enum AttackState
        {
            PREP = 0,
            CONTACT = 1,
            RECOVER = 2,
            NONE = 3,
        }

        [Header("Attack Parameter")]
        public int attackDamage = 1;
        public float rangeAttack = 1;
        public float prepTime = 1;
        public float attackTime = 1;
        public float recoverTime = 1;
        public bool isStunLockable = false;
        public bool isStopMovingAtPrep = false;

        private float m_timer;
        private NpcMetaInfos m_npcMetaInfos;
        private NPCEnemiAnimation m_NPCEnemiAnimation;

        [Header("Attack Objects")]
        public GameObject colliderObject;
        public GameObject decalsObject;
        public Transform baseTransform;

        [Header("Attack Infos")]
        public AttackState currentAttackState;
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
            currentAttackState = AttackState.NONE;
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
            if (!isStunLockable) return;

            OnFinishAttack?.Invoke(false);
            currentAttackState = AttackState.NONE;
        }

        #region  Active Phase Functions
        public void ActivePrepationAttack()
        {
            OnPrepAttack?.Invoke();
            currentAttackState = AttackState.PREP;
            decalsObject.SetActive(true);
            if (isStopMovingAtPrep) m_npcMetaInfos.state = NpcState.ATTACK;
             m_timer = 0.0f;

            if (isActiveDebug) Debug.Log($"Agent {transform.gameObject.name} is preparing to attack");
        }

        public void ActiveAttackContact()
        {
            OnContactAttack?.Invoke();
            currentAttackState = AttackState.CONTACT;
            colliderObject.SetActive(true);
            m_timer = 0.0f;
            if (isActiveDebug) Debug.Log($"Agent {transform.gameObject.name} is attacking the target");
            
        }

        public void ActiveRecoverPhase()
        {
            OnRecoverAttack?.Invoke();
            currentAttackState = AttackState.RECOVER;
            m_timer = 0.0f;
            if (isActiveDebug) Debug.Log($"Agent {transform.gameObject.name} is recoving from the attack launch");
        }

        #endregion

        #region  Finish Phase Functions
        public void FinishPreparationAttack()
        {
            decalsObject.SetActive(false);
            ActiveAttackContact();
        }

        public void FinishContactAttack()
        {
            colliderObject.SetActive(false);
            ActiveRecoverPhase();
        }

        public void FinishRecoverAttack()
        {
            OnFinishAttack?.Invoke(true);
            currentAttackState = AttackState.NONE;
            if (isActiveDebug) Debug.Log($"Agent {transform.gameObject.name} has finished to attack");
        }
        #endregion

        #region Update Phase Functions
        public void UpdatePrepAttack()
        {
            if (currentAttackState != AttackState.PREP) return;

            if (m_timer > prepTime)
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
            if (currentAttackState != AttackState.CONTACT) return;

            if (m_timer > attackTime)
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
            if (currentAttackState != AttackState.RECOVER) return;

            if (m_timer > recoverTime)
            {
                FinishRecoverAttack();
            }
            else
            {
                m_timer += Time.deltaTime;
            }
        }
        #endregion

    }
}