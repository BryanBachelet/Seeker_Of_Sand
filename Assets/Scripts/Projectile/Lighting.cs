using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lighting : ProjectileExplosif
{
    private bool m_isOnGround;
    private bool m_isActive;
    private float m_baseTime;
    private float m_timerBeforeExplosion;
    public float m_attackTime = .5f;

    private const float m_maxTimeDamageTick = 1f;
    private const float m_minTimeDamageTick = 0.2f;

    void Start()
    {
        InitTrajectory();
        InitUpgradeSpell();
    }
    public void InitUpgradeSpell()
    {
        m_timeBeforeExplosion += Mathf.Clamp(m_lifeTime - 1, 0, 10);
        m_explosionSize += m_size * m_sizeMultiplicateurFactor;
        m_attackTime -= m_shootNumber * 0.053f;
        m_attackTime = Mathf.Clamp(m_attackTime, m_minTimeDamageTick, m_maxTimeDamageTick);
    }


    // Update is called once per frame
    void Update()
    {
        m_isOnGround = m_isTravelFinish;
        UpdateTravelTime();
        Move();
        SpawnAttack();
        GroundDuration();
    }

    public void SpawnAttack()
    {
        if (!m_isOnGround) return;

        if (m_timerBeforeExplosion > m_baseTime + m_attackTime)
        {
            Instantiate(m_VFXObject, transform.position, transform.rotation);
            Collider[] enemies = Physics.OverlapSphere(transform.position, m_explosionSize, m_explosionMask);
            GlobalSoundManager.PlayOneShot(indexSFXExplosion, transform.position);
            for (int i = 0; i < enemies.Length; i++)
            {
                for (int j = 0; j < m_salveNumber; j++)
                {
                    Enemies.NpcHealthComponent enemyTouch = enemies[i].GetComponent<Enemies.NpcHealthComponent>();
                    if (enemyTouch == null) continue;

                    if (enemyTouch.m_npcInfo.state == Enemies.NpcState.DEATH)
                    {
                        Destroy(this.gameObject);
                        return;
                    }
                    DamageStatData damageStatData = new DamageStatData(m_damage, objectType);
                    enemyTouch.ReceiveDamage(spellProfil.name, damageStatData, enemyTouch.transform.position - transform.position, m_power,-1);
                }
            }
            m_baseTime = m_timerBeforeExplosion;
        }
    }

    private void GroundDuration()
    {
        if (!m_isOnGround) return;

        if (m_timerBeforeExplosion > m_timeBeforeExplosion)
        {
            Destroy(this.gameObject);
        }
        else
        {
            m_timerBeforeExplosion += Time.deltaTime;
        }
    }

    protected override void Move()
    {
        if (m_isOnGround) return;
        CurveTrajectory();
    }

}
