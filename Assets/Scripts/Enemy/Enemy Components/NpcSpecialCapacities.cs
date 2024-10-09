using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GuerhoubaGames.AI;

namespace Enemies
{
    [System.Serializable]
    public class SpecialCapacity : MonoBehaviour
    {
        public int indexSpecialCapacity;
        public virtual void ActivateSkill()
        {

        }
        public virtual void UpdateSkill(float deltaTime)
        {

        }

        public virtual bool CanLaunchSkill()
        {
            return false;
        }

    }

    public class NpcSpecialCapacities : MonoBehaviour
    {
       public SpecialCapacity[] specialCapacities;

        public Action<bool> OnFinish;

        private int currentSpecialCapacityIndex;
        private bool isLaunchingSpecialCapacity;

        private NpcMetaInfos m_npcMetaInfos;
        private NpcAttackComponent m_npcAttackComponent;
        
        private BehaviorTreeComponent m_behaviorTreeComponent;

        public void Awake()
        {
            m_npcMetaInfos = GetComponent<NpcMetaInfos>();
            m_behaviorTreeComponent = GetComponent<GuerhoubaGames.AI.BehaviorTreeComponent>();
        }

        public void ActivateSpecialCapacity(int index)
        {
            m_npcMetaInfos.state = NpcState.SPECIAL_CAPACITIES;
            specialCapacities[index].ActivateSkill();
            specialCapacities[index].indexSpecialCapacity = index;
            currentSpecialCapacityIndex = index;
            isLaunchingSpecialCapacity = true;
            OnFinish += ResetSpecialCapacity;
        }

        public void Update()
        {
            if (!isLaunchingSpecialCapacity) return;
            specialCapacities[currentSpecialCapacityIndex].UpdateSkill(Time.deltaTime);

        }


        public void TriggerSpecialCapacityBehavior(int index)
        {
            m_behaviorTreeComponent.behaviorTree.blackboard.IsSpecialCapacityCall = true;
            m_behaviorTreeComponent.behaviorTree.blackboard.indexSpecialCapacityCall = index;
        }

        public void ResetSpecialCapacity(bool isFinish)
        {
            isLaunchingSpecialCapacity = false;
            currentSpecialCapacityIndex = -1;
            OnFinish -= ResetSpecialCapacity;
            m_npcAttackComponent.isGeneralAttackCooldownActive = true;
        }

        public bool CanActivateSpecialSkill(int index)
        {
          return specialCapacities[index].CanLaunchSkill();
        }

    }

}
