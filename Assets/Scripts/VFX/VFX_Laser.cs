using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemies;
using GuerhoubaTools.Gameplay;
using UnityEngine.VFX;
namespace GuerhoubaGames.VFX
{

    public class VFX_Laser : MonoBehaviour
    {
        private VFXAttackMeta m_vfxAttackMeta;
        private NpcAttackComponent m_npcAttackComponent;
        public Transform startTransform;
        public Transform endPointTransform;
        private VisualEffect vfx;
        private ClockTimer m_lifeTimer = new ClockTimer();

        public AnimationCurve scaleYInTime;

        public void Awake()
        {
            m_vfxAttackMeta = GetComponent<VFXAttackMeta>();
            m_vfxAttackMeta.OnStart += InitVFX;
        }


        public void Update()
        {
            if (!this.enabled) return;


          
            if(endPointTransform.gameObject.activeSelf) { endPointTransform.position = m_npcAttackComponent.raycastHitPoint; }

            if (m_lifeTimer.GetRatio() > 0.98f)
            {
                vfx.SetFloat("Thickness_I", scaleYInTime.Evaluate(0));
                vfx.SetFloat("Thickness_II", scaleYInTime.Evaluate(0) * 0.9f);
                vfx.SetFloat("Thickness_III", scaleYInTime.Evaluate(0) * 0.65f);
            }
            else
            {
                vfx.SetFloat("Thickness_I", scaleYInTime.Evaluate(m_lifeTimer.GetRatio()));
                vfx.SetFloat("Thickness_II", scaleYInTime.Evaluate(m_lifeTimer.GetRatio()) * 0.9f);
                vfx.SetFloat("Thickness_III", scaleYInTime.Evaluate(m_lifeTimer.GetRatio()) * 0.65f);
            }
            if (m_lifeTimer.UpdateTimer())
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
                vfx = GetComponent<VisualEffect>();
                m_npcAttackComponent = m_vfxAttackMeta.vfxData.parent.GetComponent<NpcAttackComponent>();
                m_lifeTimer.SetTimerDuration(m_npcAttackComponent.currentAttackData.prepationTime);
                m_lifeTimer.ActiaveClock();
            }
            endPointTransform.position = m_npcAttackComponent.raycastHitPoint;

        }
    }
}
