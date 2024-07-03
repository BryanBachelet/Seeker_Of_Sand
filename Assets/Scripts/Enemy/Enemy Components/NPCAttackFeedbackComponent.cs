using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.GameEnum;
using UnityEngine.VFX;

namespace Enemies
{

    public struct AttackInfoData
    {
        public int attackIndex;
        public AttackPhase phase;
        public AttackNPCData attackNPCData;
        public float duration;
        public Vector3 positionAttack;
        
    }


    public class NPCAttackFeedbackComponent : MonoBehaviour
    {
        public AttackFeedbackData[] attackFeedbackDataArr = new AttackFeedbackData[0];

        public void SpawnFeedbacks(AttackInfoData attackInfoData)
        {
            for (int i = 0; i < attackFeedbackDataArr.Length; i++)
            {
                AttackFeedbackData currFeedbackData = attackFeedbackDataArr[i];

                if (currFeedbackData.attackIndex != attackInfoData.attackIndex || currFeedbackData.attackPhase != attackInfoData.phase) continue;

                if (currFeedbackData.feedbackType == FeedbackType.VISUAL) SpawnVisualFeedback(currFeedbackData,attackInfoData);
                if (currFeedbackData.feedbackType == FeedbackType.SOUND) SpawnSoundFeedback(currFeedbackData,attackInfoData);


            }
        }


        private void SpawnSoundFeedback(AttackFeedbackData attackFeedbackData, AttackInfoData attackInfoData)
        {
            Vector3 spawnPositon = transform.position;
            if (attackFeedbackData.areaSpawnType == AttackFeedbackData.FeedbackPosition.Target)
                spawnPositon = attackInfoData.positionAttack;

            GlobalSoundManager.PlayOneShot(attackFeedbackData.sfxIndex, spawnPositon);
        }

        private void SpawnVisualFeedback(AttackFeedbackData attackFeedbackData,AttackInfoData attackInfoData)
        {
            GameObject vfx = null;
            if (!attackFeedbackData.isSpawn)
            {
                attackFeedbackData.Vfx.SetActive(true);
                vfx = attackFeedbackData.Vfx;
            }
            else
            {
                Vector3 spawnPositon = transform.position;

                if (attackFeedbackData.areaSpawnType == AttackFeedbackData.FeedbackPosition.Target)
                    spawnPositon = attackInfoData.positionAttack;

                vfx = Instantiate(attackFeedbackData.Vfx, spawnPositon, Quaternion.identity);
               

            }


           // vfx.GetComponent<VisualEffect>().Play();
            GuerhoubaGames.VFX.VFXAttackMeta vfxMeta  = vfx.GetComponent<GuerhoubaGames.VFX.VFXAttackMeta>();
            GuerhoubaGames.VFX.VfxAttackData vfxData = new GuerhoubaGames.VFX.VfxAttackData();
            vfxData.attackRange = attackInfoData.attackNPCData.radius;
            vfxData.duration = attackInfoData.duration;
            vfxMeta.InitVFXObject(vfxData);



        }


    }
}
