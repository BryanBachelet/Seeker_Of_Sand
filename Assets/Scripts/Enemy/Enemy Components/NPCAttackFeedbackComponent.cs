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

                if (GamePullingSystem.instance == null)
                {
                    vfx = Instantiate(attackFeedbackData.Vfx, spawnPositon, Quaternion.Euler(0, transform.eulerAngles.y, 0));
                }
                else
                {
                    int id = attackFeedbackData.Vfx.GetInstanceID();
                    if (GamePullingSystem.instance.isObjectPoolExisting(id))
                    {
                        vfx =  GamePullingSystem.instance.SpawnObject(id);
                        vfx.transform.position = spawnPositon;
                        vfx.transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
                    }
                    else
                    {
                        vfx = Instantiate(attackFeedbackData.Vfx, spawnPositon, Quaternion.Euler(0, transform.eulerAngles.y, 0));
                    }
                }

                


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
