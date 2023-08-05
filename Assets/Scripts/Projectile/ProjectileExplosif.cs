using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileExplosif : Projectile
{
    [SerializeField] protected float m_timeBeforeExplosion = 1.0f;
    [SerializeField] protected float m_explosionSize;
    [SerializeField] private float m_angleTrajectory = 45.0f;
    [SerializeField] protected LayerMask m_explosionMask;
    [SerializeField] private Material m_explosionMatToUse;

    public AnimationCurve scaleByTime_Curve;
    public AnimationCurve coloriseByTime_Curve;
    public Color baseEmissiveColor;

    [SerializeField] private bool m_ActivationVFX;
    [SerializeField] private GameObject m_VFXObject;
    private bool m_isStick;
    private bool m_onEnemy;

    private Transform m_stickTransform;
    private Vector3 m_stickPosition;
    private Transform m_transform;
    private Material m_mat_explosion;

    private float m_gravityForce;
    private float m_distanceDest;
    private Vector3 m_directionHeight;
    private Vector3 prevPosition;

    private void Start()
    {
        m_transform = gameObject.GetComponent<Transform>();
        m_mat_explosion = new Material(m_explosionMatToUse);
        gameObject.GetComponent<MeshRenderer>().material = m_mat_explosion;

        InitTrajectory();
    }

    protected virtual void InitTrajectory()
    {
        Vector3 direction = (m_destination - transform.position);
        m_distanceDest = direction.magnitude;
        Vector3 rightDirection = Quaternion.AngleAxis(90, Vector3.up) * direction.normalized;
        m_directionHeight = Quaternion.AngleAxis(-90, rightDirection.normalized) * direction.normalized;



        m_direction = direction.normalized;
        m_direction.Normalize();
        m_speed = GetSpeed(m_distanceDest, m_lifeTime - 0.1f, m_angleTrajectory);
        m_gravityForce = GetGravity(m_speed, m_lifeTime - 0.1f, m_angleTrajectory, 0);
    }

    #region Physics Function
    protected virtual float GetSpeed(float distance, float timeMax, float angle)
    {
        angle = Mathf.Deg2Rad * angle;
        float speedZero = distance / (timeMax * Mathf.Cos(angle));
        return speedZero;
    }

    protected virtual float GetGravity(float speed, float timeMax, float angle, float deltaHeight)
    {
        angle = Mathf.Deg2Rad * angle;
        float gravitySpeed = 2 * (speed * Mathf.Sin(angle) * timeMax + deltaHeight);
        gravitySpeed = gravitySpeed / (timeMax * timeMax);

        return gravitySpeed;

    }
    #endregion

    void Update()
    {
        Move();
        StickBehavior();
        Duration();
    }


    protected override void Move()
    {
        if (m_isStick) return;

        CurveTrajectory();
    }

    protected virtual void CurveTrajectory()
    {
        float timer = (m_lifeTimer * m_lifeTimer) / 2.0f;
        float xPos = m_speed * Mathf.Cos(m_angleTrajectory * Mathf.Deg2Rad) * m_lifeTimer;
        float yPos = -m_gravityForce * timer + m_speed * Mathf.Sin(m_angleTrajectory * Mathf.Deg2Rad) * m_lifeTimer;

        Vector3 pos = m_direction.normalized * xPos + m_directionHeight.normalized * yPos;
        transform.position += (pos - prevPosition);
        prevPosition = pos;
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

    public override void CollisionEvent(Collider other)
    {
        if (m_isStick && m_onEnemy || other.tag == "Player") return;
        if (other.tag == "Enemy")
        {
            m_onEnemy = true;
            if (m_ActivationVFX) { Instantiate(m_VFXObject, transform.position, transform.rotation, transform); }
            m_stickTransform = other.transform;
            m_stickPosition = other.transform.position - transform.position;
        }
        if (m_isStick) return;
        StartCoroutine(TimeToExplose(m_timeBeforeExplosion));
        m_isStick = true;


    }

    protected virtual IEnumerator TimeToExplose(float timer)
    {
        yield return new WaitForSeconds(timer);
        Explosion();
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, m_destination);
    }
    protected virtual void Explosion()
    {
        if (m_stickTransform != null)
        {
            Enemies.NpcHealthComponent stickyEnemy = m_stickTransform.GetComponent<Enemies.NpcHealthComponent>();
            Collider[] enemies = Physics.OverlapSphere(transform.position, m_explosionSize, m_explosionMask);

            for (int i = 0; i < enemies.Length; i++)
            {
                Enemies.NpcHealthComponent enemyTouch = enemies[i].GetComponent<Enemies.NpcHealthComponent>();
                if (enemyTouch == null) continue;

                if (enemyTouch.npcState == Enemies.NpcState.DEATH)
                {
                    Destroy(this.gameObject);
                    return;
                }
                if (enemyTouch != stickyEnemy)
                    enemyTouch.ReceiveDamage(m_damage, enemyTouch.transform.position - transform.position, m_power);
            }


            stickyEnemy.ReceiveDamage(m_damage, stickyEnemy.transform.position - transform.position, m_power);
        }
        StartCoroutine(DelayDestroy(2));
    }

    private void ScaleByTime()
    {
        m_lifeTimer += Time.deltaTime;
        float scaleByTime = scaleByTime_Curve.Evaluate(m_lifeTimer / m_timeBeforeExplosion) / 2;
        m_transform.localScale = new Vector3(scaleByTime, scaleByTime, scaleByTime);
        m_mat_explosion.SetColor("_EmissiveColor", baseEmissiveColor * coloriseByTime_Curve.Evaluate(m_lifeTimer / m_timeBeforeExplosion));
    }

    public IEnumerator DelayDestroy(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(this.gameObject);
    }

}
