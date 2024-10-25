using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.AI;
using GuerhoubaTools.Gameplay;
namespace Enemies
{

    public class NPC_TSCustomComponent : MonoBehaviour
    {
        private BehaviorTreeComponent m_behaviorTreeComponent;
        private NpcAttackComponent m_npcAttackComponent;
        private NpcMouvementComponent m_npcMovementComponent;
        private NpcMetaInfos m_npcMetaComponent;
        private TS_RingSkill m_tsRingSkill;

        [Header("Low Life Parameter ")]
        public float baseAttackCooldownUpdateValue = 0.25f;
        [Tooltip("Reduce specific cooldown of each attack")]
        public float cooldownAttackReduction = 0.25f;
        public float maxSpeedUpdateValue = 100;
        public float minSpeedUpdateValue = 40;


        [Header("Low Life Animation")]
        [SerializeField] private float m_lowLifeAnimationDuration;
        private bool m_isLowLifeState;
        private ClockTimer m_lowlifeAnimTimer = new ClockTimer();
        [SerializeField] private Animator m_animatorTS;


        private bool m_isSubscribe;
        #region Unity Functions 

        public void Awake()
        {
            m_npcAttackComponent = GetComponent<NpcAttackComponent>();
            m_behaviorTreeComponent = GetComponent<BehaviorTreeComponent>();
            m_npcMovementComponent = GetComponent<NpcMouvementComponent>();
            m_npcMetaComponent = GetComponent<NpcMetaInfos>();
            m_tsRingSkill = GetComponent<TS_RingSkill>();
           m_npcMetaComponent.OnStart += SetupComponent;
        }

        public void Update()
        {
            if(m_isLowLifeState)
            {

                if(m_lowlifeAnimTimer.UpdateTimer())
                {
                    m_lowlifeAnimTimer.DeactivateClock();
                    m_npcMetaComponent.state = NpcState.IDLE;
                    m_animatorTS.ResetTrigger("LowLifeTrigger");
                    m_isLowLifeState = false;
                }
            }
        }


        #endregion


        public void SetupComponent()
        {
            // Rotate character to the player
            transform.rotation = Quaternion.FromToRotation(transform.forward, (m_npcMovementComponent.targetData.baseTarget.position - transform.position).normalized);
            if(m_isSubscribe)m_behaviorTreeComponent.behaviorTree.blackboard.event1 -= LowLifeEvent;
            m_behaviorTreeComponent.behaviorTree.blackboard.event1 += LowLifeEvent;
            m_isSubscribe = true;
        }

        public void LowLifeEvent()
        {
            m_npcAttackComponent.baseCooldownAttack = baseAttackCooldownUpdateValue;
            m_npcAttackComponent.attackCooldownReduction = cooldownAttackReduction;
            Movement.SinusoMouvement sinuso = (Movement.SinusoMouvement)m_npcMovementComponent.customMouvement;
            sinuso.maxSpeed = maxSpeedUpdateValue;
            sinuso.minSpeed = minSpeedUpdateValue;
            m_tsRingSkill.UpgradeRingVisual();
            Debug.Log("Low life event has been call");
            m_lowlifeAnimTimer.SetTimerDuration(m_lowLifeAnimationDuration);
            m_lowlifeAnimTimer.ActiaveClock();
            m_npcMetaComponent.state = NpcState.SPECIAL_ACTION;
            m_animatorTS.SetTrigger("LowLifeTrigger");
            m_isLowLifeState = true;
        }

        public void OnDrawGizmos()
        {
            if(m_isLowLifeState)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(transform.position, new Vector3(5, 10.0f, 5));

            }

        }

    }
}