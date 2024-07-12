using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.VFX;


namespace Enemies
{

    [System.Serializable]
    public struct AreaDataObject
    {
        public GameObject area;
        public float timerTrigger;
        public float lifetimer;
        public float areaSize;
    }

    public class NPCDivideArea : MonoBehaviour
    {
        public enum TypeArea
        {
            VFX = 0,
            ATTACK = 1,
        }
        public TypeArea typeArea;

        private NpcAttackMeta m_attackMetaInfo;
        private VFXAttackMeta m_vfxAttackMeta;
        public List<AreaDataObject> areasData;

        public float totalTimer = 0.0f;
        private bool m_canStart = false;
        private float m_objLifetimer = 0.0f;


        public void Awake()
        {

            if (typeArea == TypeArea.ATTACK)
            {
                m_attackMetaInfo = GetComponent<NpcAttackMeta>();
                m_attackMetaInfo.OnStart += ValidateStart;
            }
            else
            {
                m_vfxAttackMeta = GetComponent<VFXAttackMeta>();
                m_vfxAttackMeta.OnStart += ValidateStart;

            }


        }

        public void ValidateStart()
        {
            m_canStart = true;
            if (typeArea == TypeArea.ATTACK)
            {
                DestroyAfterBasic destroyAfterBasic = GetComponent<DestroyAfterBasic>();
                destroyAfterBasic.m_DestroyAfterTime = 3f;
               
            }
            else
            {
                DestroyAfterBasic destroyAfterBasic = GetComponent<DestroyAfterBasic>();
                destroyAfterBasic.m_DestroyAfterTime = m_vfxAttackMeta.vfxData.duration;
            }

            
        }

        public void Update()
        {
            m_objLifetimer += Time.deltaTime;

            for (int i = 0; i < areasData.Count; i++)
            {
                if (m_objLifetimer >= areasData[i].timerTrigger)
                {
                    if (!areasData[i].area.activeSelf)
                    {
                        SetupElement(areasData[i], i);
                        areasData.Remove(areasData[i]);
                    }


                }

            }

        }

        #region Setup Elements Functions
        private void SetupElement(AreaDataObject areaData, int index)
        {
            areaData.area.SetActive(true);
            if (typeArea == TypeArea.ATTACK)
            {
                SetupAttack(areaData, index);
            }
            else
            {
                SetupVFxSign(areaData, index);
            }
        }

        private void SetupVFxSign(AreaDataObject areaData, int index)
        {
            VFXAttackMeta vFXAttackMeta = areaData.area.GetComponent<VFXAttackMeta>();

            VfxAttackData vfxAttackData = new VfxAttackData();
            vfxAttackData.attackRange = areaData.areaSize;
            vfxAttackData.duration = areaData.lifetimer;
            vfxAttackData.isDestroying = true;

            vFXAttackMeta.vfxData = vfxAttackData;
            vFXAttackMeta.InitVFXObject(vfxAttackData);
        }

        private void SetupAttack(AreaDataObject areaData, int index)
        {
            NpcAttackMeta npcAttackMeta = areaData.area.GetComponent<NpcAttackMeta>();

            AttackObjMetaData attackObjMetaData = m_attackMetaInfo.attackObjMetaData;

            npcAttackMeta.InitAttackObject(attackObjMetaData);

        }
        #endregion


    }
}
