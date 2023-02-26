using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ProjectileExplosif : Projectile
{
    [SerializeField] private float m_timeBeforeExplosion = 1.0f;
    [SerializeField] private float m_explosionSize;
    [SerializeField] private float m_angleTrajectory = 45.0f;
    [SerializeField] private LayerMask m_explosionMask;
    [SerializeField] private Material m_explosionMatToUse;

    public AnimationCurve scaleByTime_Curve;
    public AnimationCurve coloriseByTime_Curve;
    public Color baseEmissiveColor;

    private bool m_isStick;
    private bool m_onEnemy;

    private Transform m_stickTransform;
    private Vector3 m_stickPosition;
    private Transform m_transform;
    private Material m_mat_explosion;

    private float m_gravityForce;
    private float m_distanceDest;

    private void Start()
    {
        m_transform = gameObject.GetComponent<Transform>();
        m_mat_explosion = new Material(m_explosionMatToUse);
        gameObject.GetComponent<MeshRenderer>().material = m_mat_explosion;

        Vector3 posHorizontal = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 destHorizontal = new Vector3(m_destination.x, 0, m_destination.z);
        m_distanceDest = Vector3.Distance(posHorizontal, destHorizontal);
        m_direction = destHorizontal - posHorizontal;
        m_direction.Normalize();
        m_speed = GetSpeed(m_distanceDest, m_lifeTime-0.1f, m_angleTrajectory);
        m_gravityForce = GetGravity(m_speed, m_lifeTime - 0.1f, m_angleTrajectory, transform.position.y - m_destination.y );
    }

    #region Physics Function
    public float GetSpeed(float distance, float timeMax, float angle)
    {
        angle = Mathf.Deg2Rad * angle;
        float speedZero = distance / (timeMax * Mathf.Cos(angle));
        return speedZero;
    }

    float GetGravity(float speed, float timeMax, float angle, float deltaHeight)
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

        float speedX = m_speed * Mathf.Cos(m_angleTrajectory * Mathf.Deg2Rad);
        float xPos = m_speed * Mathf.Cos(m_angleTrajectory * Mathf.Deg2Rad) * Time.deltaTime;

        float speedY = (-m_gravityForce * m_lifeTimer + m_speed * Mathf.Sin(m_angleTrajectory * Mathf.Deg2Rad));
        float yPos = (-m_gravityForce * m_lifeTimer + m_speed * Mathf.Sin(m_angleTrajectory * Mathf.Deg2Rad)) * Time.deltaTime;

        Vector3 pos = m_direction.normalized * xPos + new Vector3(0.0f, yPos, 0.0f);
        transform.position += pos;
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
        StartCoroutine(TimeToExplose());
        m_isStick = true;
    }

    private IEnumerator TimeToExplose()
    {
        yield return new WaitForSeconds(m_timeBeforeExplosion);
        Explosion();
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, m_destination);
    }
    private void Explosion()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, m_explosionSize, m_explosionMask);
        GlobalSoundManager.PlayOneShot(0, transform.position);
        for (int i = 0; i < enemies.Length; i++)
        {
            Enemies.Enemy enemyTouch = enemies[i].GetComponent<Enemies.Enemy>();
            if (enemyTouch.IsDestroing())
            {
                Destroy(this.gameObject);
                return;
            }
                
                enemyTouch.HitEnemy(m_damage,enemyTouch.transform.position - transform.position, m_power);
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
