using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainBullet : Projectile
{
    [SerializeField] private float m_timeDamageTick = 0.3f;
    private float m_timerDamageTick;

    [SerializeField] private float m_radiusArea = 2.0f;
    private List<Enemies.Enemy> m_enemiesList = new List<Enemies.Enemy>();
    private SphereCollider m_collider;

    public void Start()
    {
        m_collider = GetComponent<SphereCollider>();
    }

    public void Update()
    {
        if (TimerDamageTick())
        {
            for (int i = 0; i < m_enemiesList.Count; i++)
            {
                ApplyDamage(m_enemiesList[i]);
            }
        }
        Duration();
    }

    private bool TimerDamageTick()
    {
        if (m_timerDamageTick > m_timeDamageTick)
        {
            m_timerDamageTick = 0;
            return true;
        }
        else
        {
            m_timerDamageTick += Time.deltaTime;
            return false;
        }
    }
    private void ApplyDamage(Enemies.Enemy enemy)
    {
        if (Vector3.Distance(enemy.transform.position, transform.position) > ((m_collider.radius/2.0f) - m_radiusArea))
        {
            enemy.HitEnemy(m_damage, (enemy.transform.position - transform.position).normalized, m_power);
            
            if (enemy.IsDestroing())
                m_enemiesList.Remove(enemy);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        CollisionEvent(other);
    }

    public void OnTriggerExit(Collider other)
    {
        ExitCollisionEvent(other);
    }

    public override void CollisionEvent(Collider other)
    {
        if (other.gameObject.tag != "Enemy") return;

        Enemies.Enemy enemyTouch = other.GetComponent<Enemies.Enemy>();

        if (enemyTouch.IsDestroing()) return;
        m_enemiesList.Add(enemyTouch);
    }

    public void ExitCollisionEvent(Collider other)
    {
        if (other.gameObject.tag != "Enemy") return;

        Enemies.Enemy enemyTouch = other.GetComponent<Enemies.Enemy>();

        if (enemyTouch.IsDestroing()) return;
        m_enemiesList.Remove(enemyTouch);
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, (m_collider.radius/ 2.0f));
        Gizmos.DrawWireSphere(transform.position, (m_collider.radius -m_radiusArea)/2.0f);
    }
}
