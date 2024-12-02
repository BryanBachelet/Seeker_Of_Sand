using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : Projectile
{


    private Rigidbody m_rigidbody;
    private bool m_hasRoll;
    private bool m_hasStart =false;
    // Start is called before the first frame update
    void Start()
    {
        m_rigidbody = GetComponentInChildren<Rigidbody>();
        StartCoroutine(SecondStart());
    }

    private IEnumerator SecondStart()
    {
        yield return new WaitForSeconds(1.0f);
        GlobalSoundManager.PlayOneShot(m_indexSFX, transform.position);
        m_rigidbody.velocity = m_direction.normalized * m_speed;
        m_hasStart = true;
        yield return null;
    }


    public void FixedUpdate()
    {
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!m_hasStart) return;
        ControlSpeed();
        Duration();
    }

    private void ControlSpeed()
    {
        if (m_rigidbody.velocity.magnitude > 8f)
        {
            m_hasRoll = true;
            return;
        }
        if (m_hasRoll && m_rigidbody.velocity.magnitude < 5f)
        {
            Destroy(this.gameObject);
        }
    }

    protected override void Duration()
    {
        if (m_hasRoll) return;

        if (m_lifeTimer > m_lifeTime)
        {

            Destroy(this.gameObject);
        }
        else
        {
            m_lifeTimer += Time.deltaTime;
        }
    }


    public override void CollisionEvent(Collider other)
    {
        if (other.gameObject.tag != "Enemy") return;
        //GlobalSoundManager.PlayOneShot(10, transform.position);
        Enemies.NpcHealthComponent enemyTouch = other.GetComponent<Enemies.NpcHealthComponent>();
        m_characterShoot.ActiveOnHit(other.transform.position, EntitiesTrigger.Enemies, other.gameObject, spellProfil.tagData.element);
        if (enemyTouch.m_npcInfo.state == Enemies.NpcState.DEATH) return;

        DamageStatData damageStatData = new DamageStatData((int)(m_rigidbody.velocity.magnitude * m_damage), objectType);
        enemyTouch.ReceiveDamage(spellProfil.name, damageStatData, (other.transform.position - transform.position).normalized, m_power, (int)m_characterShoot.lastElement, (int)CharacterProfile.instance.stats.baseStat.damage);
    }

}
