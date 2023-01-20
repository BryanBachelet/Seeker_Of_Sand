using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{

    [Range(0, 1.0f)] [SerializeField] private float m_shakeAmount = 0.5f;
    [Range(0, 180.0f)] [SerializeField] private float m_shakeYaw = 5f;
    [Range(0, 180.0f)] [SerializeField] private float m_shakeRoll = 5f;
    [Range(0, 180.0f)] [SerializeField] private float m_shakePitch = 5f;
    [SerializeField] private float m_shakeOffsetX = 5f;
    [SerializeField] private float m_shakeOffsetY = 5f;
    [SerializeField] private float m_shakeOffsetZ = 5f;

    private Vector3 m_shakeOffset;
    private Vector3 m_shakeAngleOffset;
    private float m_timePast;

    public Vector3 GetShakeOffset()
    {
        return m_shakeOffset;
    }

    public Vector3 GetShakeEuleurAngle()
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
