using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Enemies
{
    [System.Serializable]
    public class SpecialCapacity : MonoBehaviour
    {
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

        public void Awake()
        {
            m_npcMetaInfos = GetComponent<NpcMetaInfos>();
        }

        public void ActivateSpecialCapacity(int index)
        {
            m_npcMetaInfos.state = NpcState.SPECIAL_CAPACITIES;
            specialCapacities[index].ActivateSkill();
            currentSpecialCapacityIndex = index;
            isLaunchingSpecialCapacity = true;
            OnFinish += ResetSpecialCapacity;
        }

        public void Update()
        {
            if (!isLaunchingSpecialCapacity) return;
            specialCapacities[currentSpecialCapacityIndex].UpdateSkill(Time.deltaTime);

        }

        public void ResetSpecialCapacity(bool isFinish)
        {
            isLaunchingSpecialCapacity = false;
            currentSpecialCapacityIndex = -1;
            OnFinish -= ResetSpecialCapacity;
        }

        public bool CanActivateSpecialSkill(int index)
        {
          return specialCapacities[index].CanLaunchSkill();
        }

    }

}
