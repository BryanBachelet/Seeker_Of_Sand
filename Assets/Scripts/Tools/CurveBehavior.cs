using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CurveInfo
{
    public float duration;
    public float angleOfTrajectory;
    public Vector3 destination;
    public Vector3 startPosition;
}

public class CurveBehavior 
{
    private CurveInfo m_curveInfo;
    private float m_distanceDest;
    private Vector3 m_directionHeight;
    private Vector3 m_direction;
    public float m_speed;
    private float m_gravityForce;
    private Vector3 m_prevPosition;
    private float m_lifeTimer;

    private bool m_isDurationOver =false;

    public void SendCurveInfo(CurveInfo info)
    {
        m_curveInfo = info;
        InitTrajectory();
        m_isDurationOver = false;
        m_lifeTimer = 0; 
        m_prevPosition = Vector3.zero;
    }

    public bool IsCurveFinish() { return m_isDurationOver; }

    private void InitTrajectory()
    {
        Vector3 direction = (m_curveInfo.destination - m_curveInfo.startPosition);
        m_distanceDest = direction.magnitude;
        Vector3 rightDirection = Quaternion.AngleAxis(90, Vector3.up) * direction.normalized;
        m_directionHeight = Quaternion.AngleAxis(-90, rightDirection.normalized) * direction.normalized;

        m_direction = direction.normalized;
        m_direction.Normalize();
        m_speed = GetSpeed(m_distanceDest, m_curveInfo.duration, m_curveInfo.angleOfTrajectory);
        m_gravityForce = GetGravity(m_speed, m_curveInfo.duration, m_curveInfo.angleOfTrajectory, 0);
    }

    private float GetSpeed(float distance, float timeMax, float angle)
    {
        angle = Mathf.Deg2Rad * angle;
        float speedZero = distance / (timeMax * Mathf.Cos(angle));
        return speedZero;
    }
    private float GetGravity(float speed, float timeMax, float angle, float deltaHeight)
    {
        angle = Mathf.Deg2Rad * angle;
        float gravitySpeed = 2 * (speed * Mathf.Sin(angle) * timeMax + deltaHeight);
        gravitySpeed = gravitySpeed / (timeMax * timeMax);
        return gravitySpeed;
    }

    public Vector3 UpdateCurveBehavior()
    {
        Vector3 position = CurveTrajectory();
        Duration();
        return position;
    }

    private Vector3 CurveTrajectory()
    {
        float timer = (m_lifeTimer * m_lifeTimer) / 2.0f;
        float xPos = m_speed * Mathf.Cos(m_curveInfo.angleOfTrajectory * Mathf.Deg2Rad) * m_lifeTimer;
        float yPos = -m_gravityForce * timer + m_speed * Mathf.Sin(m_curveInfo.angleOfTrajectory * Mathf.Deg2Rad) * m_lifeTimer;

        Vector3 pos = m_direction.normalized * xPos + m_directionHeight.normalized * yPos;
        Vector3 finalPos = (pos - m_prevPosition);
        m_prevPosition = pos;
        return finalPos;
    }

    private void Duration()
    {
        if (m_lifeTimer > m_curveInfo.duration)
        {
            m_isDurationOver = true;
        }
        else
        {
            m_lifeTimer += Time.deltaTime;
        }
    }

}
