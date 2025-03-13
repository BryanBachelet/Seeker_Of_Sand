using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.GameEnum;
using UnityEngine.VFX;
using GuerhoubaGames.Resources;

namespace Enemies
{

    public struct AttackInfoData
    {
        public int attackIndex;
        public AttackPhase phase;
        public Vector3 scale;
        public float duration;
        public Vector3 positionAttack;
        public Transform target;
        public AreaType areaType;

        public bool isDelayValid;
        
    }


    public class NPCAttackFeedbackComponent : MonoBehaviour
    {
        public AttackFeedbackData[] attackFeedbackDataArr = new AttackFeedbackData[0];

        public void SpawnFeedbacks(AttackInfoData attackInfoData)
        {
            for (int i = 0; i < attackFeedbackDataArr.Length; i++)
            {
                AttackFeedbackData currFeedbackData = attackFeedbackDataArr[i];


                if (currFeedbackData.attackIndex != attackInfoData.attackIndex || currFeedbackData.attackPhase != attackInfoData.phase || attackInfoData.isDelayValid != currFeedbackData.isDelayed) continue;

                if (currFeedbackData.feedbackType == FeedbackType.VISUAL) SpawnVisualFeedback(currFeedbackData,attackInfoData);
                if (currFeedbackData.feedbackType == FeedbackType.SOUND) SpawnSoundFeedback(currFeedbackData,attackInfoData);


            }
        }


        private void SpawnSoundFeedback(AttackFeedbackData attackFeedbackData, AttackInfoData attackInfoData)
        {
            Vector3 spawnPositon = transform.position;
            if (attackFeedbackData.areaSpawnType == AttackFeedbackData.FeedbackPosition.Target)
                spawnPositon = attackInfoData.positionAttack;

            GlobalSoundManager.PlayOneShot(attackFeedbackData.sfxIndex, transform.position);
        }

        private void SpawnVisualFeedback(AttackFeedbackData attackFeedbackData,AttackInfoData attackInfoData)
        {
            GameObject vfx = null;
            if (!attackFeedbackData.isSpawn)
            {
                vfx = attackFeedbackData.Vfx;
                attackFeedbackData.Vfx.SetActive(true);
            }
            else
            {
                Vector3 spawnPositon = transform.position;

                if (attackFeedbackData.areaSpawnType == AttackFeedbackData.FeedbackPosition.Target)
                    spawnPositon = attackInfoData.positionAttack;
                if (attackFeedbackData.areaSpawnType == AttackFeedbackData.FeedbackPosition.LastHit)
                    spawnPositon = attackInfoData.positionAttack;


                spawnPositon = spawnPositon + Quaternion.Euler(0, transform.eulerAngles.y, 0) * attackFeedbackData.offsetSpawnPosition;
                vfx = GamePullingSystem.SpawnObject(attackFeedbackData.Vfx,spawnPositon, Quaternion.Euler(0, transform.eulerAngles.y,0));
 
            }

            GuerhoubaGames.VFX.VFXAttackMeta vfxMeta  = vfx.GetComponent<GuerhoubaGames.VFX.VFXAttackMeta>();
            GuerhoubaGames.VFX.VfxAttackData vfxData = new GuerhoubaGames.VFX.VfxAttackData();
            vfxData.scale = attackInfoData.scale;
            vfxData.duration = attackInfoData.duration;
            vfxData.isDestroying = attackFeedbackData.isSpawn;
            vfxData.parent = transform;
            vfxData.target = attackInfoData.target;
            vfxMeta.InitVFXObject(vfxData);


        }


    }
}
