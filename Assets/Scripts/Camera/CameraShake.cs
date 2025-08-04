using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Render.Camera
{

    public class CameraShake : CameraEffect
    {

        [Range(0, 0.1f)] [SerializeField] private float m_shakeAmount = 0.01f;
        [Range(10, 100.0f)] [SerializeField] private float m_shakeYaw = 20f;
        [Range(10, 100.0f)] [SerializeField] private float m_shakeRoll = 20f;
        [Range(10, 100.0f)] [SerializeField] private float m_shakePitch = 20f;
        [HideInInspector] private float m_shakeOffsetX = 1f;
        [HideInInspector] private float m_shakeOffsetY = 1f;
        [HideInInspector] private float m_shakeOffsetZ = 1f;

        private Vector3 m_shakeOffset;
        private Vector3 m_shakeAngleOffset;
        private float m_timePast;

        public override Vector3 GetEffectPos()
        {
            return m_shakeOffset;
        }

        public override Vector3 GetEffectRot()
        {
            return m_shakeAngleOffset;
        }

        public IEnumerator ShakeEffect(float duration)
        {

            while (m_timePast < duration)
            {
                Vector3 angles = new Vector3();
                Vector3 pos = new Vector3();

                pos.x = m_shakeOffsetX * m_shakeAmount * Random.Range(-1.0f, 1.0f);
                pos.y = m_shakeOffsetY * m_shakeAmount * Random.Range(-1.0f, 1.0f);
                pos.z = m_shakeOffsetZ * m_shakeAmount * Random.Range(-1.0f, 1.0f);


                m_shakeOffset = pos;

                angles.x = m_shakePitch * m_shakeAmount * Random.Range(-1.0f, 1.0f);
                angles.y = m_shakeYaw * m_shakeAmount * Random.Range(-1.0f, 1.0f);
                angles.z = m_shakeRoll * m_shakeAmount * Random.Range(-1.0f, 1.0f);

                m_shakeAngleOffset = angles;

                m_timePast += Time.deltaTime;
                yield return Time.deltaTime;
            }


            m_timePast = 0;
            m_shakeOffset = Vector3.zero;
            m_shakeAngleOffset = Vector3.zero;


            yield return null;
        }
    }
}