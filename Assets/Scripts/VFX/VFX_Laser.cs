using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemies;
using GuerhoubaTools.Gameplay;
namespace GuerhoubaGames.VFX
{

    public class VFX_Laser : MonoBehaviour
    {
        private VFXAttackMeta m_vfxAttackMeta;
        private NpcAttackComponent m_npcAttackComponent;
        public Transform endPointTransform;

        private ClockTimer m_lifeTimer = new ClockTimer();

        public void Awake()
        {
            m_vfxAttackMeta = GetComponent<VFXAttackMeta>();
        }
        public void OnEnable()
        {
            InitVFX();
            endPointTransform.position = m_npcAttackComponent.raycastHitPoint;
        }
        public void  Update()
        {
            if (!this.enabled) return;

            endPointTransform.position = m_npcAttackComponent.raycastHitPoint;
            if(m_lifeTimer.UpdateTimer())
            {
                this.gameObject.SetActive(false);
                m_lifeTimer.DeactivateClock();
            }
        }

        public void InitVFX()
        {
            m_vfxAttackMeta = GetComponent<VFXAttackMeta>();
            if (m_vfxAttackMeta.vfxData.parent)
            {
                m_npcAttackComponent = m_vfxAttackMeta.vfxData.parent.GetComponent<NpcAttackComponent>();
                m_lifeTimer.SetTimerDuration(m_npcAttackComponent.currentAttackData.prepationTime);
                m_lifeTimer.ActiaveClock();
            }
                
        }
    }
}
