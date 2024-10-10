using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies.Movement
{
    [CreateAssetMenu(fileName = "Sinusoidale Move", menuName = "Enemmis/Move/Custom/Sinusoidale Movement", order = 1)]
    public class SinusoMouvement : NpcCustomMouvement
    {
        public float minDistanceToApplyPattern = 15;
        public float frequence = 1;
        public float angleCurve = 45;
        public float stopDistance = 10;

        public float maxDistanceToMaxSpeed;
        public float maxSpeed;
        public float minSpeed;

        private Vector3 m_lastPosition;
        private float m_timeMoving;

        public override void SetupMove(CustomMovementData customMovementData)
        {
            base.SetupMove(customMovementData);
            m_timeMoving = 0.0f;
            if (customMovementData.path != null)
            {
                m_path = customMovementData.path;

            }
            SetupSpeed();
        }

        private void SetupSpeed()
        {
            if (m_path == null || m_path.corners.Length == 0) return;
            float distance = Vector3.Distance(m_transform.position, m_lastPosition);
            float ratio = (distance - +stopDistance + m_agent.stoppingDistance) / (maxDistanceToMaxSpeed);
            ratio = Mathf.Clamp(ratio, 0, 1);
            m_agent.speed = Mathf.Lerp(minSpeed, maxSpeed, ratio);
          
        }

        public override void Move(NavMeshPath path)
        {
            if (path != null)
            {
                m_path = path;
            }

            if (m_path == null || m_path.corners.Length == 0) return;
            m_lastPosition = m_path.corners[m_path.corners.Length - 1];
            Vector3 offsetPosition = Vector3.zero;
            float distance = Vector3.Distance(m_transform.position, m_lastPosition);
            SetupSpeed();
            Vector3 direction = m_lastPosition - m_transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            m_transform.rotation = Quaternion.Slerp(m_transform.rotation, lookRotation, Time.deltaTime * 5);
          
            if (distance < minDistanceToApplyPattern + stopDistance + m_agent.stoppingDistance)
            {
              
                offsetPosition = direction.normalized * m_agent.speed * Time.deltaTime;
                if (distance > stopDistance + m_agent.stoppingDistance)
                        m_agent.Move(offsetPosition);
                else
                {
                    SetupSpeed();
                    m_agent.isStopped = true;
                }
                return;
            }


            float sin = Mathf.Sin(1 + frequence * m_timeMoving);
            float sign = Mathf.Sign(sin);
            sin = sin * sign;
            sin = Mathf.Clamp(sin, -1.0f, 1.0f);
            m_timeMoving += Time.deltaTime;

            NavMeshHit hit = new NavMeshHit();
            bool result = NavMesh.SamplePosition(m_transform.position, out hit, 100.0f, NavMesh.AllAreas);

            Vector3 axis = Vector3.up;
            //if (result)
            //    axis = hit.normal;

            float t = sin / 2.0f + 0.5f;
            float angle = Mathf.Lerp(-angleCurve, angleCurve, t);
            Vector3 dir = m_lastPosition - m_transform.position;
            dir = Quaternion.AngleAxis(angle, axis) * dir.normalized;

             lookRotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
            m_transform.rotation = Quaternion.Slerp(m_transform.rotation, lookRotation, Time.deltaTime * 5);
            offsetPosition = dir.normalized * m_agent.speed * Time.deltaTime;
            m_agent.Move(offsetPosition);
            m_agent.destination = m_lastPosition;


        }

        public override bool CanStopMoving()
        {
            float distance = Vector3.Distance(m_transform.position, m_lastPosition);
            bool canStop = distance < stopDistance + m_agent.stoppingDistance;
            if (canStop)
            {
                Debug.Log("test");
            }
            return canStop;
        }
    }
}