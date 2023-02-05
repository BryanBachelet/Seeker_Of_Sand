using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ProjectileExplosif : Projectile
{
    [SerializeField] private float m_timeBeforeExplosion = 1.0f;
    [SerializeField] private float m_explosionSize;
    [SerializeField] private LayerMask m_explosionMask;
    [SerializeField] private Material m_explosionMatToUse;

    public AnimationCurve scaleByTime_Curve;
    public AnimationCurve coloriseByTime_Curve;
    public Color baseEmissiveColor;

    private bool m_isStick;
    private bool m_onEnemy;

    private Transform m_stickTransform;
    private Vector3 m_stickPosition;
    private float m_lifeTimer;
    private Transform m_transform;
    private Material m_mat_explosion;

    private void Start()
    {
        m_transform = gameObject.GetComponent<Transform>();
        m_mat_explosion = new Material(m_explosionMatToUse);
        gameObject.GetComponent<MeshRenderer>().material = m_mat_explosion;

    }
    void Update()
    {
        Move();
        StickBehavior();
        Duration();
    }


    protected override void Move()
    {
        if (m_isStick) return;

        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(transform.position, m_direction.normalized,out hit, m_speed * Time.deltaTime, m_layer))
        {
            m_stickPosition = hit.point;
            m_isStick = true;
            StartCoroutine(TimeToExplose());
        }

        transform.position += m_direction.normalized * m_speed * Time.deltaTime;
    }

    private void StickBehavior()
    {
        if (!m_isStick) return;

        if (m_onEnemy)
        {
            if (m_stickTransform == null)
            {
                m_isStick = false;
            }
            else
            {
                transform.position = m_stickTransform.position + m_stickPosition;
            }

        }
        ScaleByTime();
    }
    protected override void Duration()
    {
        if (m_isStick) return;

        base.Duration();
    }

    private void OnTriggerEnter(Collider other)
    {
        CollisionEvent(other);
    }

    protected override void CollisionEvent(Collider other)
    {
        if (m_isStick && m_onEnemy || other.tag == "Player") return;
        if (other.tag == "Enemy")
        {
            m_onEnemy = true;
            m_stickTransform = other.transform;
            m_stickPosition = other.transform.position - transform.position;
        }
        if (m_isStick) return;
        m_isStick = true;
        StartCoroutine(TimeToExplose());
    }

    private IEnumerator TimeToExplose()
    {
        yield return new WaitForSeconds(m_timeBeforeExplosion);
        Explosion();
    }

    private void Explosion()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, m_explosionSize, m_explosionMask);
        GlobalSoundManager.PlayOneShot(0, transform.position);
        for (int i = 0; i < enemies.Length; i++)
        {
            Enemies.Enemy enemyTouch = enemies[i].GetComponent<Enemies.Enemy>();
            if (enemyTouch.IsDestroing()) return;

            enemyTouch.GetDestroy(enemyTouch.transform.position - transform.position, m_power);
        }
        Destroy(this.gameObject);
    }

    private void ScaleByTime()
    {
        m_lifeTimer += Time.deltaTime;
        float scaleByTime = scaleByTime_Curve.Evaluate(m_lifeTimer / m_timeBeforeExplosion) / 2;
        m_transform.localScale = new Vector3(scaleByTime, scaleByTime, scaleByTime);
        m_mat_explosion.SetColor("_EmissiveColor", baseEmissiveColor * coloriseByTime_Curve.Evaluate(m_lifeTimer / m_timeBeforeExplosion));
    }


}
