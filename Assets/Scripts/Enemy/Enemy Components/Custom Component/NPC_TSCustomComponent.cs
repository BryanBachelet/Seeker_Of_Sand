using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.AI;
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

        public void SetupComponent()
        {
            m_behaviorTreeComponent.behaviorTree.blackboard.event1 += LowLifeEvent;
        }


        #endregion

        public void LowLifeEvent()
        {
            m_npcAttackComponent.baseCooldownAttack = baseAttackCooldownUpdateValue;
            m_npcAttackComponent.attackCooldownReduction = cooldownAttackReduction;
            Movement.SinusoMouvement sinuso = (Movement.SinusoMouvement)m_npcMovementComponent.customMouvement;
            sinuso.maxSpeed = maxSpeedUpdateValue;
            sinuso.minSpeed = minSpeedUpdateValue;
            m_tsRingSkill.UpgradeRingVisual();
            Debug.Log("Low life event has been call");
        }

    }
}