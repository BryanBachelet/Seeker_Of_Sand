using System.Collections;
using System.Collections.Generic;
using GuerhoubaGames.Enemies;
using UnityEngine;
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

        public VisualEffect circleVFX;

        private bool isCircleInstantiate;

        public void Awake()
        {
            m_vfxAttackMeta = GetComponent<VFXAttackMeta>();
            m_vfxAttackMeta.OnStart += InitVFX;
        }


        public void Update()
        {
            if (!this.enabled) return;




            if (endPointTransform.gameObject.activeSelf && m_npcAttackComponent)
            {
                endPointTransform.position = m_npcAttackComponent.raycastHitPoint;
            }

            if (m_lifeTimer.GetRatio() > 0.98f)
            {
                if (vfx) vfx.SetFloat("Thickness_I", scaleYInTime.Evaluate(0));
                //vfx.SetFloat("Thickness_II", scaleYInTime.Evaluate(0) * 0.9f);
                //vfx.SetFloat("Thickness_III", scaleYInTime.Evaluate(0) * 0.65f);
            }
            else
            {
                if(vfx) vfx.SetFloat("Thickness_I", scaleYInTime.Evaluate(m_lifeTimer.GetRatio()));
                //vfx.SetFloat("Thickness_II", scaleYInTime.Evaluate(m_lifeTimer.GetRatio()) * 0.9f);
                //vfx.SetFloat("Thickness_III", scaleYInTime.Evaluate(m_lifeTimer.GetRatio()) * 0.65f);
            }


            ActiveCircleVfx();

            if (m_lifeTimer.UpdateTimer())
            {

                this.gameObject.SetActive(false);
                m_lifeTimer.DeactivateClock();
            }
        }

        private void ActiveCircleVfx()
        {
            if (m_npcAttackComponent && m_npcAttackComponent.isStopRotationFirstFrame && !isCircleInstantiate)
            {
                isCircleInstantiate = true;
                circleVFX.gameObject.SetActive(isCircleInstantiate);
                // Set the circle VFX
                if (circleVFX != null)
                {
                    circleVFX.SetVector3("Size", m_vfxAttackMeta.vfxData.scale * 0.5f);
                    circleVFX.SetFloat("TempsRealese", m_vfxAttackMeta.vfxData.duration - m_lifeTimer.GetTimer());
                    circleVFX.SendEvent("ActiveArea");

                    RaycastHit hit = new RaycastHit();

                    if (Physics.Raycast(circleVFX.transform.position + Vector3.up * 3, -Vector3.up, out hit, 100, GameLayer.instance.groundLayerMask))
                    {
                        float angle = Vector3.SignedAngle(Vector3.up, hit.normal, Vector3.forward);

                        circleVFX.transform.rotation = Quaternion.Euler(angle, circleVFX.transform.eulerAngles.y, circleVFX.transform.eulerAngles.z);
                        circleVFX.transform.position = hit.point;
                    }
                }
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

            isCircleInstantiate = false;
            circleVFX.gameObject.SetActive(isCircleInstantiate);

        }
    }
}
