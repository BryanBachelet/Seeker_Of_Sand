using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.GameEnum;

public class ProjectileExplosif : Projectile
{
    [SerializeField] protected float m_timeBeforeExplosion = 1.0f;
    [SerializeField] protected float m_explosionSize;
    [SerializeField] protected float m_angleTrajectory = 45.0f;
    [SerializeField] protected LayerMask m_explosionMask;
    [SerializeField] private Material m_explosionMatToUse;

    public AnimationCurve scaleByTime_Curve;
    public AnimationCurve coloriseByTime_Curve;
    public Color baseEmissiveColor;

    [SerializeField] private bool m_ActivationVFX;
    [SerializeField] protected GameObject m_VFXObject;
    [SerializeField] public int indexSFXExplosion;
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
    protected bool m_isTravelFinish;
    private UnityEngine.VFX.VisualEffect vfxBase;

    private void Start()
    {
        vfxBase = GetComponentInChildren<UnityEngine.VFX.VisualEffect>();

        m_transform = gameObject.GetComponent<Transform>();
        m_mat_explosion = new Material(m_explosionMatToUse);
        gameObject.GetComponent<MeshRenderer>().material = m_mat_explosion;
        GlobalSoundManager.PlayOneShot(m_indexSFX, transform.position);
        InitTrajectory();
    }

    public override void SetProjectile(ProjectileData data, CharacterProfile charaProfil)
    {
        m_characterProfil = charaProfil;
        base.SetProjectile(data, m_characterProfil);

        if (spellProfil.tagData.spellProjectileTrajectory == SpellProjectileTrajectory.CURVE)
        {
            m_travelTime = spellProfil.GetFloatStat(StatType.TrajectoryTimer);
            m_angleTrajectory = spellProfil.GetIntStat(StatType.AngleTrajectory);
        }

        if (spellProfil.tagData.EqualsSpellParticularity(SpellParticualarity.Delayed))
        {
            m_timeBeforeExplosion = spellProfil.GetFloatStat(StatType.TimeDelay);
        }

        if (spellProfil.tagData.EqualsSpellParticularity(SpellParticualarity.Explosion))
        {
            m_explosionSize = spellProfil.GetFloatStat(GuerhoubaGames.GameEnum.StatType.SizeExplosion);
            m_damage = spellProfil.GetIntStat(GuerhoubaGames.GameEnum.StatType.DamageAdditionel);
        }
    }



    protected virtual void InitTrajectory()
    {
        Vector3 direction = (m_destination - transform.position);
        m_distanceDest = direction.magnitude;
        Vector3 rightDirection = Quaternion.AngleAxis(90, Vector3.up) * direction.normalized;
        m_directionHeight = Quaternion.AngleAxis(-90, rightDirection.normalized) * direction.normalized;

        float angleTest = Vector3.SignedAngle(m_directionHeight, Vector3.up, direction);
        if (angleTest != 0)
        {
            m_directionHeight = Quaternion.AngleAxis(angleTest, direction) * m_directionHeight;
        }

        m_direction = direction.normalized;
        m_direction.Normalize();
        m_speed = GetSpeed(m_distanceDest, m_travelTime, m_angleTrajectory);
        m_gravityForce = GetGravity(m_speed, m_travelTime, m_angleTrajectory, 0);
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
        UpdateTravelTime();
    }


    protected override void Move()
    {
        if (m_isTravelFinish || m_isStick) return;

        CurveTrajectory();
    }

    protected virtual void CurveTrajectory()
    {
        float timer = (m_travelTimer * m_travelTimer) / 2.0f;
        float xPos = m_speed * Mathf.Cos(m_angleTrajectory * Mathf.Deg2Rad) * m_travelTimer;
        float yPos = -m_gravityForce * timer + m_speed * Mathf.Sin(m_angleTrajectory * Mathf.Deg2Rad) * m_travelTimer;

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
    }
    protected override void Duration()
    {
        if (m_isStick) return;
    
    }

    protected override void UpdateTravelTime()
    {
        if (m_isTravelFinish) return;

        if (m_travelTimer > m_travelTime)
        {
            transform.position = m_destination + Vector3.up * 0.5f;
            m_isTravelFinish = true;
        }
        else
        {
            m_travelTimer += Time.deltaTime;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        CollisionEvent(other);
    }

    public override void CollisionEvent(Collider other)
    {
        if (m_isStick && m_onEnemy || other.tag == "Player" || other.tag == "Projectile") return;

        if (other.tag == "Enemy")
        {
            Debug.Log(other.name + "has been stick");
            m_onEnemy = true;
            if (m_ActivationVFX) { Instantiate(m_VFXObject, transform.position, transform.rotation, transform); }
            m_stickTransform = other.transform;
            m_stickPosition = (other.transform.position - transform.position).normalized ;
        }
        if (m_isStick) return;
        StartCoroutine(TimeToExplose(m_timeBeforeExplosion));
        m_isStick = true;
        Debug.Log(other.name + " [Has been hit by exlosion]");


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
            GlobalSoundManager.PlayOneShot(indexSFXExplosion, transform.position);
             DamageStatData damageStatData = new DamageStatData(m_damage, objectType);
            for (int i = 0; i < enemies.Length; i++)
            {
                Enemies.NpcHealthComponent enemyTouch = enemies[i].GetComponent<Enemies.NpcHealthComponent>();
                if (enemyTouch == null) continue;

                if (enemyTouch.m_npcInfo.state == Enemies.NpcState.DEATH)
                {
                    Destroy(this.gameObject);
                    return;
                }
                if (enemyTouch != stickyEnemy)
                {
                    
                    enemyTouch.ReceiveDamage(spellProfil.name, damageStatData, enemyTouch.transform.position - transform.position, m_power, -1, (int)CharacterProfile.instance.stats.baseStat.damage);
                }
            }
               

            m_characterShoot.ActiveOnHit(stickyEnemy.transform.position, EntitiesTrigger.Enemies, stickyEnemy.gameObject, (GameElement)elementIndex);

            stickyEnemy.ReceiveDamage(spellProfil.name, damageStatData, stickyEnemy.transform.position - transform.position, m_power, -1, (int)CharacterProfile.instance.stats.baseStat.damage);
            m_stickTransform = null;
        }
        vfxBase.gameObject.SetActive(false);
        StartCoroutine(DelayDestroy(2));
    }

    public IEnumerator DelayDestroy(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(this.gameObject);
    }

}
