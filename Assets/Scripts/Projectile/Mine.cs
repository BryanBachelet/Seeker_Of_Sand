using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : ProjectileExplosif
{

    [Header("Mine Parameter")]
    [SerializeField] private float m_setupDuration = 1.0f;
    private float m_setupTimer = 0.0f;
    [SerializeField] private float powerPlayerProjection = 25.0f;

    private bool m_isOnGround;
    private bool m_isActive;
    private bool m_isTrigger;


    public void Start()
    {
        SetupNewDestination();
        InitTrajectory();
    }

    private void SetupNewDestination()
    {
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(m_destination + Vector3.up * 50.0f, -Vector3.up, out hit, Mathf.Infinity, m_layer))
        {
            m_destination = hit.point;
        }
    }

    public void Update()
    {
        Duration();
        Move();
        SetupMine();
    }

    // Curve mouvement Life Time
    protected override void Duration()
    {
        if (m_lifeTimer > m_lifeTime)
        {
            transform.position = m_destination + Vector3.up * 0.5f;
            m_isOnGround = true;
        }
        else
        {
            m_lifeTimer += Time.deltaTime;
        }
    }
   
    // Curve Mouvement
    protected override void Move()
    {
        if (m_isOnGround) return;
        CurveTrajectory();
    }

    // Timer of setup
    private void SetupMine()
    {
        if (m_isActive || !m_isOnGround) return;

        if (m_setupTimer > m_setupDuration)
        {
            m_setupTimer = 0;
            m_isActive = true;
        }
        else
        {
            m_setupTimer += Time.deltaTime;
        }
    }


    public override void CollisionEvent(Collider other)
    {

        if (!m_isActive || m_isTrigger || !m_isOnGround) return;

        if (other.tag == "Enemy" || other.tag == "Player")
        {
            m_isTrigger = true;
            StartCoroutine(TimeToExplose(m_timeBeforeExplosion));
        }

    }

    protected override void Explosion()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, m_explosionSize, m_explosionMask);
        GlobalSoundManager.PlayOneShot(0, transform.position);
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] == null) continue;
           
            if (enemies[i].tag == "Player")
            {
                enemies[i].GetComponent<Character.CharacterMouvement>().Projection((Vector3.up + (enemies[i].transform.position -transform.position ).normalized).normalized * powerPlayerProjection, ForceMode.Impulse);
                continue;
            }
            
            Enemies.Enemy enemyTouch = enemies[i].GetComponent<Enemies.Enemy>();
           
            if (enemyTouch == null) continue;

            if (enemyTouch.IsDestroing()) continue;
             enemyTouch.HitEnemy(m_damage, (Vector3.up + (enemies[i].transform.position - transform.position).normalized).normalized , m_power);

          

        }
        Destroy(this.gameObject);
    }



}
