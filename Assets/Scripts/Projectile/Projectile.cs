using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.GameEnum;
using GuerhoubaGames.Resources;
using UnityEngine.VFX;
using SeekerOfSand.Tools;
using Mono.Cecil;
using Enemies;
using SpellSystem;
using UnityEngine.Profiling;
using UnityEditor.Purchasing;

public struct ProjectileData
{
    public Vector3 direction;
    public float speed;
    public float life;
    public float travelTime;
    public int damage;
    public Vector3 destination;
    public GameObject area_Feedback;
    public int piercingMax;
    public int shootNumber;
    public int salveNumber;
    public float sizeFactor;
    public float size;
    public SpellSystem.SpellProfil spellProfil;
    public Character.CharacterShoot characterShoot;
    public CharacterObjectType objectType;
    public int element;
    public string nameFragment;
}



public class Projectile : MonoBehaviour
{
    protected const int m_timeBeforeDestruction = 3;
    protected const float m_timeStartSizeShrinking = 0.75f;
    private const int maxSlopeAngle = 90;
    protected Vector3 m_direction;
    [SerializeField] protected float m_speed;
    [SerializeField] protected float m_lifeTime;
    [SerializeField] protected LayerMask m_layer;
    [SerializeField] protected float m_power;
    [SerializeField] protected int m_damage = 1;
    [SerializeField] public int m_indexSFX;

    [HideInInspector] public CharacterProfile m_characterProfil;
    protected Vector3 m_destination;
    protected float m_lifeTimer;
    public int m_piercingMax;
    protected int piercingCount;

    protected float m_travelTime;
    protected float m_travelTimer;
    protected int m_shootNumber;
    protected int m_salveNumber;
    protected float m_size;
    protected float m_sizeMultiplicateurFactor;

    protected SpellSystem.SpellProfil spellProfil;
    protected Character.CharacterShoot m_characterShoot;

    private float spawnTime;
    private bool checkSpawnTime = false;
    [SerializeField] private float m_deltaTimeMove = 0.0f;
    protected bool willDestroy = false;
    protected Collider m_collider;
    protected CharacterObjectType objectType;
    protected Vector3 m_initialScale;

    protected bool isStartToMove = false;

    protected Vector3 normalHit;
    protected Vector3 hitPoint;

    protected string damageSourceName;
    protected int elementIndex;

    public bool isDebugInstance;

    public GameObject vFXObject;
    public GameObject vFXExplosion;

    void Update()
    {
        if (!checkSpawnTime) { spawnTime = Time.time; checkSpawnTime = true; GlobalSoundManager.PlayOneShot(m_indexSFX, transform.position); }
        else
        {
            float currentTime = Time.time;
            if (currentTime > spawnTime + m_deltaTimeMove)
            {
                if (willDestroy)
                {
                    if (m_collider) m_collider.enabled = false;
                }
                else
                {
                    isStartToMove = true;
                    Move();

                }
                Duration();
            }
        }
    }


    public virtual void SetProjectile(ProjectileData data, CharacterProfile charaProfil)
    {
        m_characterProfil = charaProfil;
        m_piercingMax = 0;
        m_direction = data.direction;
        spellProfil = data.spellProfil;
        m_speed = spellProfil.GetFloatStat(StatType.Range) / spellProfil.GetFloatStat(StatType.LifeTime);
        m_lifeTime = spellProfil.GetFloatStat(StatType.LifeTime);
        m_damage = spellProfil.GetIntStat(StatType.Damage);
        m_destination = data.destination;

        if (spellProfil.tagData.spellParticualarity == SpellParticualarity.Piercing)
            m_piercingMax = spellProfil.GetIntStat(StatType.Piercing);

        m_shootNumber = spellProfil.GetIntStat(StatType.ShootNumber);
        m_salveNumber = spellProfil.GetIntStat(StatType.Projectile);
        m_size = 1;

        if (spellProfil.tagData.spellProjectileTrajectory == SpellProjectileTrajectory.CURVE)
            m_travelTime = spellProfil.GetFloatStat(StatType.TrajectoryTimer);

        m_sizeMultiplicateurFactor = 1;
        m_characterShoot = data.characterShoot;
        m_initialScale = transform.localScale;

        m_characterShoot = data.characterShoot;
        m_collider = this.GetComponent<Collider>();
        objectType = data.objectType;

        if (vFXObject != null) vFXObject.transform.rotation *= Quaternion.Euler(spellProfil.angleRotation);
        damageSourceName = spellProfil.name;
        elementIndex = (int)spellProfil.tagData.element;
        if (objectType == CharacterObjectType.FRAGMENT)
        {
            damageSourceName = data.nameFragment;
        }

    }

    public virtual void SetFragmentDirectProjectile(ProjectileData data)
    {
        m_piercingMax = 0;
        m_direction = data.direction;
        m_lifeTime = data.life;
        m_damage = data.damage;
        m_characterShoot = data.characterShoot;
        m_speed = data.speed;
        objectType = data.objectType;
        elementIndex = data.element;


        m_initialScale = transform.localScale;

        damageSourceName = "NoName";

        if (objectType == CharacterObjectType.FRAGMENT)
        {
            damageSourceName = data.nameFragment;
        }

    }

    protected virtual void Move()
    {
        //Debug.Log("Test");
        if (Physics.Raycast(transform.position, m_direction.normalized, m_speed * Time.deltaTime, m_layer))
        {
            ActiveDeath();
        }
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(transform.position, -Vector3.up, out hit, Mathf.Infinity, m_layer))
        {
            normalHit = hit.normal;
            hitPoint = hit.point;

            SetSlopeRotation(hit.normal);


        }
        transform.position += transform.forward * m_speed * Time.deltaTime;
    }
    protected virtual void Duration()
    {
        if (m_lifeTimer > m_lifeTime + m_timeBeforeDestruction)
        {
            ActiveDeath();
            return;
        }
        if (m_lifeTimer > m_lifeTime && !willDestroy )
        {
            willDestroy = true;
            ApplyExplosion();
        }
        if (m_lifeTimer > m_lifeTime - m_timeStartSizeShrinking)
        {
            transform.localScale = Vector3.Lerp(m_initialScale, Vector3.zero, m_lifeTimer - m_lifeTime);
        }

        m_lifeTimer += Time.deltaTime;

    }



    protected virtual void ResetProjectile()
    {
        m_lifeTimer = 0.0f;
        willDestroy = false;
        if (m_collider) m_collider.enabled = true;
        checkSpawnTime = false;
        piercingCount = 0;
        m_travelTimer = 0.0f;
        transform.localScale = m_initialScale;
        isStartToMove = false;
        VisualEffect visual = GetComponent<VisualEffect>();

        if (vFXObject != null) vFXObject.transform.rotation = Quaternion.Inverse( Quaternion.Euler(spellProfil.angleRotation));
        if (visual != null) visual.Reinit();
    }

    public virtual void ActiveDeath()
    {
        if (isDebugInstance)
        {
            Debug.Log("Stop Execution");
        }
        PullingMetaData pullingMetaData = GetComponent<PullingMetaData>();
        if (GamePullingSystem.instance != null && pullingMetaData != null)
        {
            ResetProjectile();
            GamePullingSystem.instance.ResetObject(this.gameObject, pullingMetaData.id);
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        CollisionEvent(other);
    }
    public virtual void CollisionEvent(Collider other)
    {

        if (!this.enabled) return;

        if (other.gameObject.tag == "DecorDes")
        {
            other.GetComponent<DestructibleObject>().SetupDestruction(m_power, other.transform.position - transform.position);
            return;
        }
        if (other.gameObject.tag == "Enemy")
        {
            Enemies.NpcHealthComponent enemyTouch = other.GetComponent<Enemies.NpcHealthComponent>();


            m_characterShoot.ActiveOnHit(other.transform.position, EntitiesTrigger.Enemies, other.gameObject, (GameElement)elementIndex);
            if (enemyTouch.m_npcInfo.state == Enemies.NpcState.DEATH) return;

            DamageStatData damageStatData = new DamageStatData(m_damage, objectType);
            enemyTouch.ReceiveDamage(damageSourceName, damageStatData, other.transform.position - transform.position, m_power, elementIndex, (int)CharacterProfile.instance.stats.baseStat.damage);

            PiercingUpdate();
            if (piercingCount >= m_piercingMax)
            {

                //Destroy(this.gameObject);
                m_lifeTimer = m_lifeTime;
                m_collider.enabled = false;
                //willDestroy = true;
            }
        }
        else if (other.gameObject.tag == "Cristal")
        {
            other.GetComponent<CristalHealth>().ReceiveHit((int)m_damage);
            PiercingUpdate();
            if (piercingCount >= m_piercingMax)
            {

                //Destroy(this.gameObject);
                m_lifeTimer = m_lifeTime + m_timeBeforeDestruction;
                //willDestroy = true;

                ApplyExplosion();
            }
        }
        else if (other.gameObject.tag == "DPSCheck")
        {
            Punketone enemyTouch = other.GetComponent<Punketone>();

            m_characterShoot.ActiveOnHit(other.transform.position, EntitiesTrigger.Enemies, other.gameObject, (GameElement)elementIndex);

            enemyTouch.currentHP -= m_damage;

            PiercingUpdate();
            if (piercingCount >= m_piercingMax)
            {

                //Destroy(this.gameObject);
                //willDestroy = true;
                m_lifeTimer = m_lifeTime;
            }
        }
        else if (other.gameObject.tag == "Dummy")
        {
            dummy enemyTouch = other.GetComponent<dummy>();

            m_characterShoot.ActiveOnHit(other.transform.position, EntitiesTrigger.Enemies, other.gameObject, (GameElement)elementIndex);
            enemyTouch.ReceiveDamage(m_damage, other.transform.position - transform.position, m_power, -1);

            enemyTouch.currentHP -= m_damage;

            PiercingUpdate();
            if (piercingCount >= m_piercingMax)
            {

                //Destroy(this.gameObject);
                //willDestroy = true;
                m_lifeTimer = m_lifeTime;
            }
        }
        else return;



    }

    protected void ApplyExplosion()
    {
        // Active Explosion
        if (spellProfil.tagData.EqualsSpellParticularity(SpellParticualarity.Explosion))
        {
            float sizeArea = spellProfil.GetFloatStat(StatType.SizeExplosion);
            Collider[] collider = new Collider[0];
            collider = Physics.OverlapSphere(transform.position, sizeArea, GameLayer.instance.enemisLayerMask);
            GameObject instance  = GamePullingSystem.SpawnObject(vFXExplosion, transform.position, Quaternion.identity);
            instance.transform.localScale *= 2;
            for (int i = 0; i < collider.Length; i++)
            {
                NpcHealthComponent npcHealthComponent = collider[i].GetComponent<NpcHealthComponent>();
                Vector3 direction = collider[i].transform.position - transform.position;


                m_characterShoot.ActiveOnHit(collider[i].transform.position, EntitiesTrigger.Enemies, collider[i].gameObject, (GameElement)elementIndex);
                DamageStatData damageStatData = new DamageStatData(spellProfil.GetIntStat(StatType.DamageAdditionel), objectType);
                npcHealthComponent.ReceiveDamage(spellProfil.name, damageStatData, direction, 10, (int)sizeArea, (int)CharacterProfile.instance.stats.baseStat.damage);
            }
        }
    }

    protected virtual void PiercingUpdate()
    {
        if (spellProfil.tagData.EqualsSpellParticularity(SpellParticualarity.Piercing))
            piercingCount++;
    }
    protected virtual void UpdateTravelTime()
    {
    }

    public void SetSlopeRotation(Vector3 hitNormal)
    {
        Quaternion rotTest = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        Vector3 transformedRight = Quaternion.Euler(0, 0, -transform.eulerAngles.z) * transform.right;
        Vector3 axis = Vector3.Cross(transformedRight, hitNormal);

        float angle = Vector3.SignedAngle(rotTest * Vector3.forward, axis, transformedRight);

        if (Mathf.Abs(angle) > maxSlopeAngle)
            return;

        transform.rotation = Quaternion.Euler(angle, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }

}
