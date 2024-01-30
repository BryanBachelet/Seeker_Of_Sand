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
    private Enemies.NpcHealthComponent m_enemyImpale;
    private bool m_firstHit;
    private float m_currentDistance;

    public void Start()
    {
        Vector3 scale = transform.localScale;
        // transform.rotation *= Quaternion.AngleAxis(90, Vector3.right);
        transform.localScale = new Vector3(scale.x, scale.y, scale.z);

        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, Mathf.Infinity, m_layer))
        {
            if (Vector3.Distance(transform.position, hit.point) < 1.5f)
            {
                transform.position += (transform.position - hit.point).normalized * 1.5f;            
                return;
            }

            if (Vector3.Distance(transform.position, hit.point) > 2f)
            {
                transform.position += (hit.point - transform.position).normalized * 1.5f;
            }
        }
    }

    protected override void Move()
    {
        if(m_enemyImpale == null || m_enemyImpale.npcState == Enemies.NpcState.DEATH)
        {
            m_enemyImpale = null;
        }
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(transform.position, transform.forward, out hit, m_speed * Time.deltaTime, m_layer))
        {
            if (m_enemyImpale != null && m_firstHit && m_enemyImpale.npcState != Enemies.NpcState.DEATH)
            {
                m_enemyImpale.ReceiveDamage(m_damage * m_impalementDamageRatio, m_enemyImpale.transform.position - transform.position, m_power);
                m_enemyImpale.npcState = Enemies.NpcState.PAUSE;
            }
            Destroy(this.gameObject);
        }

        transform.position += transform.forward * m_speed * Time.deltaTime;
        m_currentDistance += m_speed * Time.deltaTime;

        if (!m_enemyImpale && Physics.Raycast(transform.position, -Vector3.up, out hit, Mathf.Infinity, m_layer))
        {

            SetSlopeRotation(hit.normal);
        }

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
        if (other.tag == "Enemy" || other.tag == "Cristal")
        {
            GlobalSoundManager.PlayOneShot(9, transform.position);
            if (other.tag == "Enemy")
            {
                Enemies.NpcHealthComponent enemyTouch = other.GetComponent<Enemies.NpcHealthComponent>();
                if (enemyTouch.npcState == Enemies.NpcState.DEATH) return;

                if (!m_firstHit && m_currentDistance < m_minRangeToImpale)
                {
                    m_characterShoot.ActiveOnHit(other.transform.position, EntitiesTrigger.Enemies, other.gameObject);
                    enemyTouch.ReceiveDamage(m_damage * m_impalementDamageRatio, enemyTouch.transform.position - transform.position, m_power);

                    if (enemyTouch.npcState == Enemies.NpcState.DEATH) return;

                    m_firstHit = true;
                    m_enemyImpale = enemyTouch;
                    m_enemyImpale.npcState = Enemies.NpcState.PAUSE;
                }
                if (m_firstHit || m_currentDistance > m_minRangeToImpale)
                {
                    m_characterShoot.ActiveOnHit(other.transform.position, EntitiesTrigger.Enemies, other.gameObject);
                    enemyTouch.ReceiveDamage(m_damage, enemyTouch.transform.position - transform.position, m_power);
                }
            }
            if (other.tag == "Cristal")
            {
                other.GetComponent<CristalHealth>().ReceiveHit((int)m_damage);
            }
        }



    }


}
