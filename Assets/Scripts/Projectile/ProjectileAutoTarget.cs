using GuerhoubaGames.Enemies;
using GuerhoubaGames.GameEnum;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAutoTarget : Projectile
{
    public bool isTemporary;
    private bool m_hasNoTarget;
    private NpcHealthComponent m_npcHealthComponent;
    private Vector3 m_currentVelocity;
    private Vector3 m_prevVelocity;
    public AnimationCurve directionCurve;
    private Vector3 m_startDirection;
    public float timeDirection = 1;
    private float m_timerDirection;
    [HideInInspector] public float autoGuideSpeed = 30;
   
 
    public void Start()
    {
        SearchTarget();
        m_startDirection = transform.forward;
    }

    private void SearchTarget()
    {
        float range = spellProfil.GetFloatStat(StatType.Range);

        Collider[] enemis = Physics.OverlapSphere(transform.position, range, GameLayer.instance.enemisLayerMask);
        Collider nextTarget = null;
        float currentDistance = 1000f;
        for (int i = 0; i < enemis.Length; i++)
        {
            float enemiDistance = Vector3.Distance(transform.position, enemis[i].transform.position);
            if (enemiDistance < currentDistance)
            {
                currentDistance = enemiDistance;
                nextTarget = enemis[i];
            }
        }


        if (nextTarget == null)
        {
            m_hasNoTarget = true;
            return;
        }
        m_npcHealthComponent = nextTarget.GetComponent<NpcHealthComponent>();
        if (m_npcHealthComponent == null)
        {
            m_hasNoTarget = true;
            return;
        }
        Vector3 direction = m_npcHealthComponent.transform.position - transform.position;
        m_prevVelocity = transform.forward;
        m_currentVelocity = m_prevVelocity;

        m_startDirection = transform.forward;
        m_timerDirection = 0.0f;
 
    }

    protected override void Move()
    {
        if (m_hasNoTarget)
        {
            base.Move();
            return;
        }
        if (Physics.Raycast(transform.position, transform.forward.normalized, autoGuideSpeed * Time.deltaTime, m_layer))
        {
            ActiveDeath();
        }


        if (m_npcHealthComponent == null || m_npcHealthComponent.m_npcInfo.state == NpcState.DEATH)
            SearchTarget();

        Vector3 direction = m_npcHealthComponent.transform.position - transform.position;

        Vector3 velocityNormal = Vector3.RotateTowards(transform.forward, direction.normalized, 360*2  * directionCurve.Evaluate(m_timerDirection / timeDirection) * Time.deltaTime  * Mathf.Deg2Rad, 1f);
        transform.rotation = Quaternion.LookRotation(velocityNormal.normalized);
        Debug.Log("Velocity Target : " + velocityNormal.normalized.ToString());
        transform.position += transform.forward * autoGuideSpeed  * Time.deltaTime;

        m_timerDirection += Time.deltaTime;
    }

    protected override void Duration()
    {
        if(m_lifeTimer > m_lifeTime + m_timeBeforeDestruction)
        {
            ActiveDeath();
            return;
        }
        if (m_lifeTimer > m_lifeTime)
        {
            willDestroy = true;
        }
        if (m_lifeTimer > m_lifeTime - m_timeStartSizeShrinking)
        {
            transform.localScale = Vector3.Lerp(m_initialScale, Vector3.zero, m_lifeTimer - m_lifeTime);
        }
        if(m_lifeTimer >= m_lifeTime)
        {
            m_lifeTimer += Time.deltaTime;
        }
    }

    public override void ActiveDeath()
    {
        if (isTemporary)
        {
            Projectile[] proj = GetComponents<Projectile>();
            for (int i = 0; i < proj.Length; i++)
            {
                if (this != proj[i])
                {
                    proj[i].enabled = true;
                    proj[i].ActiveDeath();

                    break;
                }
            }

            Destroy(this);
            return;
        }


        base.ActiveDeath();

    }

    public override void CollisionEvent(Collider other)
    {
        base.CollisionEvent(other);
    }
}
