using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.GameEnum;
using GuerhoubaGames.Enemies;
using GuerhoubaGames.Character;
public class Mine : ProjectileExplosif
{

    [Header("Mine Parameter")]
    [SerializeField] private float m_setupDuration = 1.0f;
    private float m_setupTimer = 0.0f;
    [SerializeField] private float powerPlayerProjection = 25.0f;
    //[SerializeField] private int indexSFXExplosion;
    private Animator m_animator;

    private bool m_isActive;
    private bool m_isTrigger;

    public GameObject vfxExplosion;
    public UnityEngine.VFX.VisualEffect vfxDecal;

    public void Start()
    {
        SetupNewDestination();
        InitTrajectory();
        m_animator = this.GetComponent<Animator>();
        GlobalSoundManager.PlayOneShot(m_indexSFX, transform.position);
    }

    private void SetupNewDestination()
    {
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(m_destination + Vector3.up * 50.0f, -Vector3.up, out hit, Mathf.Infinity, m_layer))
        {
            m_destination = hit.point;
        }
    }

    public override void SetProjectile(ProjectileData data, CharacterProfile charaProfil)
    {
        base.SetProjectile(data, charaProfil);

        if (spellProfil.TagList.spellProjectileTrajectory == SpellProjectileTrajectory.CURVE)
        {
            m_travelTime = spellProfil.GetFloatStat(StatType.TrajectoryTimer);
            m_angleTrajectory = spellProfil.GetIntStat(StatType.AngleTrajectory);
        }

        if (spellProfil.TagList.EqualsSpellParticularity(SpellParticualarity.Delayed))
        {
            m_timeBeforeExplosion = spellProfil.GetFloatStat(StatType.TimeDelay);
        }

        if (spellProfil.TagList.EqualsSpellParticularity(SpellParticualarity.Explosion))
        {
            m_explosionSize = spellProfil.GetFloatStat(GuerhoubaGames.GameEnum.StatType.SizeExplosion);
            m_damage = spellProfil.GetIntStat(GuerhoubaGames.GameEnum.StatType.DamageAdditionel);
        }
    }

    public void Update()
    {
        UpdateTravelTime();
        //  Duration();
        Move();
        SetupMine();
    }



    protected override void Duration()
    {
        if (!m_isTravelFinish) return;

        if (m_lifeTimer > m_lifeTime)
        {
            if (!m_isTrigger)
            {
                Explosion();
                m_isTrigger = true;
            }
        }
        else
        {
            m_lifeTimer += Time.deltaTime;
        }
    }

    // Curve Mouvement
    protected override void Move()
    {
        if (m_isTravelFinish) return;
        CurveTrajectory();
    }

    // Timer of setup
    private void SetupMine()
    {
        if (m_isActive || !m_isTravelFinish) return;

        if (m_setupTimer > m_setupDuration)
        {
            m_setupTimer = 0;
            if (vfxDecal != null) { vfxDecal.SendEvent("ActiveArea"); }

            m_isActive = true;
        }
        else
        {
            m_setupTimer += Time.deltaTime;
        }
    }


    public override void CollisionEvent(Collider other)
    {

        if (!m_isActive || m_isTrigger || !m_isTravelFinish) return;

        if (other.tag == "Enemy" || other.tag == "Player" || other.tag == "Cristal")
        {
            m_isTrigger = true;
            StartCoroutine(TimeToExplose(m_timeBeforeExplosion));
        }

    }

    protected override void Explosion()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, m_explosionSize, m_explosionMask);
        m_animator.SetTrigger("Activation");
        GlobalSoundManager.PlayOneShot(indexSFXExplosion, transform.position);
        GameObject explosion = Instantiate(vfxExplosion, transform.position, transform.rotation);
        explosion.transform.localScale = transform.localScale;
        //GlobalSoundManager.PlayOneShot(0, transform.position);
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] == null) continue;

            if (enemies[i].tag == "Player")
            {
                enemies[i].GetComponent<CharacterMouvement>().Projection((Vector3.up + (enemies[i].transform.position - transform.position).normalized).normalized * powerPlayerProjection, ForceMode.VelocityChange);
                Debug.Log("Player explosion fonction");
                continue;
            }
            else if (enemies[i].tag == "Enemy")
            {
                NpcHealthComponent enemyTouch = enemies[i].GetComponent<NpcHealthComponent>();

                if (enemyTouch.m_npcInfo.state == NpcState.DEATH) continue;
                DamageStatData damageStatData = new DamageStatData(m_damage, objectType);
                enemyTouch.ReceiveDamage(spellProfil.name, damageStatData, (Vector3.up + (enemies[i].transform.position - transform.position).normalized).normalized, m_power, (int)m_characterShoot.lastElement, (int)CharacterProfile.GetCharacterStat().baseDamage.totalValue);
            }
            else if (enemies[i].tag == "Cristal")
            {
                enemies[i].GetComponent<CristalHealth>().ReceiveHit((int)m_damage);
            }

        }
        vfxDecal.gameObject.SetActive(false);
        StartCoroutine(DelayDestroy(2));
    }


}
