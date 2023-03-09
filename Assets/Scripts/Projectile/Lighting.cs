using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lighting : ProjectileExplosif
{
    private bool m_isOnGround;
    private bool m_isActive;
    void Start()
    {
        InitTrajectory();
    }

    // Update is called once per frame
    void Update()
    {
        Duration();
        Move();
    }

    protected override void Move()
    {
        if (m_isOnGround) return;
        CurveTrajectory();
    }
    protected override void Duration()
    {
        if (m_lifeTimer > m_lifeTime)
        {
            transform.position = m_destination + Vector3.up * 0.5f;
            m_isOnGround = true;
            StartCoroutine(TimeToExplose(m_timeBeforeExplosion));
        }
        else
        {
            m_lifeTimer += Time.deltaTime;
        }
    }


}
