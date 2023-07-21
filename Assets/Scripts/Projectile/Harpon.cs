using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harpon : Projectile
{
    [Tooltip("Range minimun for activate the impalement effect")]
    [Range(0, 100)]
    [SerializeField] private float m_minRangeToImpale = 20.0f;
    [SerializeField] private float m_impalementDamageRatio = 1.5f;
    [SerializeField] private float m_wallHitDamageRatio = 2.0f;
    private Enemies.Enemy m_enemyImpale;
    private bool m_firstHit;
    private float m_currentDistance;

    public void Start()
    {
        transform.rotation *= Quaternion.AngleAxis(90, Vector3.right);
    }

    protected override void Move()
    {
        if(m_enemyImpale == null)
        {
            m_enemyImpale = null;
        }
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(transform.position, m_direction.normalized,out hit, m_speed * Time.deltaTime, m_layer))
        {
            if (m_firstHit )
            {
                m_enemyImpale.HitEnemy(m_damage * m_impalementDamageRatio, m_enemyImpale.transform.position - transform.position, m_power);
                m_enemyImpale.ChangeActiveBehavior(true);
            }
            Destroy(this.gameObject);
        }

        transform.position += m_direction.normalized * m_speed * Time.deltaTime;
        m_currentDistance += m_speed * Time.deltaTime;
        if (m_firstHit && m_enemyImpale != null)
            m_enemyImpale.transform.position = transform.position;
    }
    private void OnTriggerEnter(Collider other)
    {
        CollisionEvent(other);
    }



    public override void CollisionEvent(Collider other)
    {
        EnemyCollision(other);
    }

    private void EnemyCollision(Collider other)
    {
        if (other.tag != "Enemy") return;

        GlobalSoundManager.PlayOneShot(9, transform.position);
        Enemies.Enemy enemyTouch = other.GetComponent<Enemies.Enemy>();
        //if (enemyTouch.IsDestroing()) return;

        if (!m_firstHit && m_currentDistance < m_minRangeToImpale)
        {
            enemyTouch.HitEnemy(m_damage * m_impalementDamageRatio, enemyTouch.transform.position - transform.position, m_power);

            //if (enemyTouch.IsDestroing()) return;

            m_firstHit = true;
            m_enemyImpale = enemyTouch;
            m_enemyImpale.ChangeActiveBehavior(false);
        }
        if (m_firstHit || m_currentDistance > m_minRangeToImpale)
        {
            enemyTouch.HitEnemy(m_damage, enemyTouch.transform.position - transform.position, m_power);
        }

    }


}
