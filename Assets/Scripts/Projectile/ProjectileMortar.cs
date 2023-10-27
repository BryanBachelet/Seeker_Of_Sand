using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ProjectileCurveData
{
    public Vector3 destination;
    public float angleTrajectory;
    public float lifetime;
    public float damage;
    public float radiusOfAttack;
    public Transform target;
}

public class ProjectileMortar : MonoBehaviour
{
    public LayerMask layerMask;
    public GameObject m_vfx;
    private ProjectileCurveData m_projectileData;

    private float m_distanceDest;
    private Vector3 m_directionHeight;
    private Vector3 m_direction;
    private float m_speed;
    private float m_gravityForce;
    private Vector3 m_prevPosition;
    private float m_lifeTimer;

    public void InitProjectile(ProjectileCurveData data)
    {
        m_projectileData = data;
    }

    public void Start()
    {
        //InitTrajectory();
        transform.position = m_projectileData.destination;
        
    }
    private void InitTrajectory()
    {
        Vector3 direction = (m_projectileData.destination - transform.position);
        m_distanceDest = direction.magnitude;
        Vector3 rightDirection = Quaternion.AngleAxis(90, Vector3.up) * direction.normalized;
        m_directionHeight = Quaternion.AngleAxis(-90, rightDirection.normalized) * direction.normalized;

        m_direction = direction.normalized;
        m_direction.Normalize();
        m_speed = GetSpeed(m_distanceDest, m_projectileData.lifetime , m_projectileData.angleTrajectory);
        m_gravityForce = GetGravity(m_speed, m_projectileData.lifetime, m_projectileData.angleTrajectory, 0);
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

    private void CurveTrajectory()
    {
        float timer = (m_lifeTimer * m_lifeTimer) / 2.0f;
        float xPos = m_speed * Mathf.Cos(m_projectileData.angleTrajectory * Mathf.Deg2Rad) * m_lifeTimer;
        float yPos = -m_gravityForce * timer + m_speed * Mathf.Sin(m_projectileData.angleTrajectory * Mathf.Deg2Rad) * m_lifeTimer;

        Vector3 pos = m_direction.normalized * xPos + m_directionHeight.normalized * yPos;
        transform.position += (pos - m_prevPosition);
        m_prevPosition = pos;
    }

    public void Update()
    {
        //CurveTrajectory();
        Duration();
        m_vfx.transform.localPosition = transform.InverseTransformPoint( m_projectileData.destination);
    }

    protected  void Duration()
    {
        if (m_lifeTimer > m_projectileData.lifetime)
        {
            if (Vector3.Distance(m_projectileData.target.position, transform.position) < m_projectileData.radiusOfAttack)
            {
                if (m_projectileData.target.tag == "Player")
                {
                    m_projectileData.target.GetComponent<health_Player>().GetDamageLeger(m_projectileData.damage,transform.position);

                }
                if (m_projectileData.target.tag == "Altar")
                {
                    m_projectileData.target.GetComponent<ObjectHealthSystem>().TakeDamage((int)m_projectileData.damage);

                }
            }
            Destroy(gameObject);
        }
        else
        {
            m_lifeTimer += Time.deltaTime;
        }
    }

    public void OnTriggerEnter(Collider collision)
    {
         if (collision.gameObject.tag == "Ground")
        {
            if (Vector3.Distance(m_projectileData.target.position, transform.position) < m_projectileData.radiusOfAttack)
            {
                if (m_projectileData.target.tag == "Player")
                {
                    m_projectileData.target.GetComponent<health_Player>().GetDamageLeger(m_projectileData.damage,transform.position);
                    
                }
            }
            Destroy(gameObject);
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_projectileData.radiusOfAttack);
    }
}
