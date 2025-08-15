using GuerhoubaGames.Enemies;
using System.Collections.Generic;
using UnityEngine;

namespace GuerhoubaGames.Resources
{
    [System.Serializable]
    public struct PullConstructionData
    {
        public GameObject instance;
        public int idInstance;
        public int count;

        public PullConstructionData( GameObject instanceObj, int count = 1)
        {
            idInstance =  GamePullingSystem.GetDeterministicHashCode(instanceObj.name);
            instance = instanceObj;
            this.count = count;
        }

        public override bool Equals(object obj)
        {

            if (obj is PullConstructionData)
            {
                return idInstance == ((PullConstructionData)(obj)).idInstance;
            }
            else
            {
                return false;
            }

        }

        public override int GetHashCode()
        {
            return GamePullingSystem.GetDeterministicHashCode(instance.name);
        }
    }

    [System.Serializable]
    public class NPCPullingData : MonoBehaviour
    {
        public List<PullConstructionData> pullDataList = new List<PullConstructionData>();

        private NPCAttackFeedbackComponent m_NPCAttackFeedbackComponent;
        private NpcAttackComponent m_NpcAttackComponent;

        public void Reset()
        {
            pullDataList.Clear();

        }

        public void Search()
        {
            if (m_NPCAttackFeedbackComponent == null) m_NPCAttackFeedbackComponent = GetComponent<NPCAttackFeedbackComponent>();
            if (m_NpcAttackComponent == null) m_NpcAttackComponent = GetComponent<NpcAttackComponent>();


            // ------------ Get Feedback to create a pull -------------
            AttackFeedbackData[] attackFeedbackData = m_NPCAttackFeedbackComponent.attackFeedbackDataArr;

            for (int i = 0; i < attackFeedbackData.Length; i++)
            {
                if (!attackFeedbackData[i].isSpawn) continue;

                PullConstructionData instancePullData = new PullConstructionData( attackFeedbackData[i].Vfx);
                if (pullDataList.Contains(instancePullData)) continue;

                pullDataList.Add(instancePullData);
            }

            // ------------------ Get Attack to create a pull ----------------

            for (int i = 0; i < m_NpcAttackComponent.attackEnemiesObjectsArr.Length; i++)
            {
                AttackNPCData attackNPCData = m_NpcAttackComponent.attackEnemiesObjectsArr[i].data;

                
                if (attackNPCData.typeAttack != GameEnum.AttackType.PROJECTILE_OBJ) continue;

                PullConstructionData instancePullData = new PullConstructionData( attackNPCData.projectileLaunch, 1);

                if (pullDataList.Contains(instancePullData)) continue;

                pullDataList.Add(instancePullData);

            }

        }
    }
}
