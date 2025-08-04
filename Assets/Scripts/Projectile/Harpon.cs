using GuerhoubaGames;
using GuerhoubaGames.GameEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harpon : Projectile
{
    [Tooltip("Range minimun for activate the impalement effect")]
    [Range(0, 100)]
    [SerializeField] private float m_minRangeToImpale = 20.0f;
    [SerializeField] private float m_impalementDamageRatio = 1.5f;
    private Enemies.NpcHealthComponent[] m_enemyImpales;
    private int m_impaleCount;
    private float m_currentDistance;

    private int impaleCountMaximum;

    [Header("Tier One Behavior")]
    public float timeBeforeSpawnArea;
    private float timerBeforeSpawnArea;
    private  bool m_IsTraineeBehaviorActive;

    [Header("Tier Three Behavior")]
    public int impaleCountUpgrade;

    public void Start()
    {
        if(spellProfil.currentSpellTier == 3)
        {
            m_enemyImpales =  new Enemies.NpcHealthComponent[impaleCountUpgrade];
        }
        else
        {
            m_enemyImpales = new Enemies.NpcHealthComponent[1];
        }
        impaleCountMaximum = m_enemyImpales.Length;
        timerBeforeSpawnArea = timeBeforeSpawnArea;
        Vector3 scale = transform.localScale;
        // transform.rotation *= Quaternion.AngleAxis(90, Vector3.right);
        //transform.localScale = new Vector3(scale.x, scale.y, scale.z);

        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, Mathf.Infinity, m_layer))
        {
            if (Vector3.Distance(transform.position, hit.point) < 1.5f)
            {
                transform.position += (transform.position - hit.point).normalized * 1.5f;            
                return;
            }

            if (Vector3.Distance(transform.position, hit.point) > 2f)
            {
                transform.position += (hit.point - transform.position).normalized * 1.5f;
            }
        }
    }

    public new void  Update() 
    {
        base.Update();
        if (m_IsTraineeBehaviorActive)
        {
            if(timerBeforeSpawnArea > timeBeforeSpawnArea)
            {
                SpawnObject(transform.position);
                timerBeforeSpawnArea = 0;
            }else
            {
                timerBeforeSpawnArea += Time.deltaTime;
            }
        }
    }

    protected override void Move()
    {
        // Check for impales status
        for (int i = 0; i < m_impaleCount; i++)
        {
            if (m_enemyImpales[i].m_npcInfo.state == Enemies.NpcState.DEATH)
            {
                m_enemyImpales[i] = null;
                m_impaleCount--;
                i--;
            }
        }
     
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(transform.position, transform.forward, out hit, m_speed * Time.deltaTime, m_layer))
        {

            for (int i = 0; i < m_impaleCount; i++)
            {
                if ( m_enemyImpales[i].m_npcInfo.state != Enemies.NpcState.DEATH)
                {
                    DamageStatData damageStatData = new DamageStatData((int)(m_damage * m_impalementDamageRatio), objectType);
                    m_enemyImpales[i].ReceiveDamage(spellProfil.name, damageStatData, m_enemyImpales[i].transform.position - transform.position, m_power, (int)m_characterShoot.lastElement, (int)CharacterProfile.GetCharacterStat().baseDamage.totalValue);
                    m_enemyImpales[i].m_npcInfo.state = Enemies.NpcState.PAUSE;
                   
                }
            }
            ActiveDeath();
        }

        if (m_impaleCount ==0  && Physics.Raycast(transform.position, -Vector3.up, out hit, Mathf.Infinity, m_layer))
        {

            SetSlopeRotation(hit.normal);
        }

        transform.position += transform.forward * m_speed * Time.deltaTime;
        m_currentDistance += m_speed * Time.deltaTime;

        for (int i = 0; i < m_impaleCount; i++)
        {
            if (m_enemyImpales[i].m_npcInfo.type != Enemies.EnemyType.TWILIGHT_SISTER)
                m_enemyImpales[i].transform.position = transform.position;
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        CollisionEvent(other);
    }


    protected override void ResetProjectile()
    {
        base.ResetProjectile();
        m_enemyImpales = null;
        m_impaleCount = 0;
        m_currentDistance = 0.0f;
        m_IsTraineeBehaviorActive = false;

        if (spellProfil.currentSpellTier == 3)
        {
            m_enemyImpales = new Enemies.NpcHealthComponent[impaleCountUpgrade];
        }
        else
        {
            m_enemyImpales = new Enemies.NpcHealthComponent[1];
        }
    }

    public override void CollisionEvent(Collider other)
    {
        EnemyCollision(other);
    }

    private void EnemyCollision(Collider other)
    {
        if (other.tag == "Enemy" || other.tag == "Dummy" || other.tag == "Cristal" || other.tag == "Object")
        {
            GlobalSoundManager.PlayOneShot(9, transform.position);
            if (other.tag == "Enemy" )
            {
                Enemies.NpcHealthComponent enemyTouch = other.GetComponent<Enemies.NpcHealthComponent>();
                if(enemyTouch.m_npcInfo.type == Enemies.EnemyType.TWILIGHT_SISTER)
                {
                    m_characterShoot.ActiveOnHit(other.transform.position, EntitiesTrigger.Enemies, other.gameObject, spellProfil.tagData.element);
                    DamageStatData damageStatData = new DamageStatData((int)(m_damage * m_impalementDamageRatio), objectType);
                    enemyTouch.ReceiveDamage(spellProfil.name, damageStatData, enemyTouch.transform.position - transform.position, m_power, (int)spellProfil.tagData.element, (int)CharacterProfile.GetCharacterStat().baseDamage.totalValue);
                    return;
                }

                if (enemyTouch.m_npcInfo.state == Enemies.NpcState.DEATH) return;

                if (m_impaleCount < impaleCountMaximum && m_currentDistance < m_minRangeToImpale)
                {
                    m_characterShoot.ActiveOnHit(other.transform.position, EntitiesTrigger.Enemies, other.gameObject, spellProfil.tagData.element);
                    m_damageCalculComponent.damageStats.AddDamage((int)(m_damage * m_impalementDamageRatio), (GameElement)elementIndex, DamageType.TEMPORAIRE);
                    DamageStatData[] damageStatDatas = m_damageCalculComponent.CalculDamage((GameElement)elementIndex, objectType, enemyTouch.gameObject, spellProfil);

                    for (int i = 0; i < damageStatDatas.Length; i++)
                    {
                        enemyTouch.ReceiveDamage(damageSourceName, damageStatDatas[i], other.transform.position - transform.position, m_power, (int)damageStatDatas[i].element, (int)CharacterProfile.GetCharacterStat().baseDamage.totalValue);
                    }

                    if (enemyTouch.m_npcInfo.state == Enemies.NpcState.DEATH) return;

                    m_enemyImpales[m_impaleCount] = enemyTouch;
                    if (spellProfil.currentSpellTier >= 1)
                    {
                        m_IsTraineeBehaviorActive = true;
                    }
                    m_enemyImpales[m_impaleCount].m_npcInfo.state = Enemies.NpcState.PAUSE;
                    m_impaleCount++;
                }

                // Other Element
                if (m_impaleCount >= impaleCountMaximum || m_currentDistance > m_minRangeToImpale)
                {
                    m_characterShoot.ActiveOnHit(other.transform.position, EntitiesTrigger.Enemies, other.gameObject, spellProfil.tagData.element);
                    m_damageCalculComponent.damageStats.AddDamage((int)(m_damage ), (GameElement)elementIndex, DamageType.TEMPORAIRE);
                    DamageStatData[] damageStatDatas = m_damageCalculComponent.CalculDamage((GameElement)elementIndex, objectType, enemyTouch.gameObject, spellProfil);

                    for (int i = 0; i < damageStatDatas.Length; i++)
                    {
                        enemyTouch.ReceiveDamage(damageSourceName, damageStatDatas[i], other.transform.position - transform.position, m_power, (int)damageStatDatas[i].element, (int)CharacterProfile.GetCharacterStat().baseDamage.totalValue);
                    }
                }
            }
            if(other.tag == "Dummy")
            {

                if (spellProfil.currentSpellTier >= 1)
                {
                    m_IsTraineeBehaviorActive = true;
                }

                Dummy_Behavior enemyTouch = other.GetComponent<Dummy_Behavior>();


                m_damageCalculComponent.damageStats.AddDamage(m_damage, (GameElement)elementIndex, DamageType.TEMPORAIRE);
                DamageStatData[] damageStatDatas = m_damageCalculComponent.CalculDamage((GameElement)elementIndex, objectType, enemyTouch.gameObject, spellProfil);

                for (int i = 0; i < damageStatDatas.Length; i++)
                {
                    enemyTouch.ReceiveDamage(damageSourceName, damageStatDatas[i], other.transform.position - transform.position, m_power, (int)damageStatDatas[i].element, (int)CharacterProfile.GetCharacterStat().baseDamage.totalValue);
                }
            }

            if(other.tag == "Object")
            {

                ObjectHealthSystem enemyTouch = other.GetComponent<ObjectHealthSystem>();
                m_damageCalculComponent.damageStats.AddDamage(m_damage, (GameElement)elementIndex, DamageType.TEMPORAIRE);
                DamageStatData[] damageStatDatas = m_damageCalculComponent.CalculDamage((GameElement)elementIndex, objectType, enemyTouch.gameObject, spellProfil);

                if (enemyTouch.eventState != EventObjectState.Active) return;

                for (int i = 0; i < damageStatDatas.Length; i++)
                {
                    enemyTouch.ReceiveDamage(damageSourceName, damageStatDatas[i], other.transform.position - transform.position, m_power, (int)damageStatDatas[i].element, (int)CharacterProfile.GetCharacterStat().baseDamage.totalValue);
                }

                PiercingUpdate();
                if (piercingCount >= m_piercingMax)
                {

                    //Destroy(this.gameObject);
                    m_lifeTimer = m_lifeTime;
                    m_collider.enabled = false;
                    //willDestroy = true;
                }
            }

                if (other.tag == "Cristal")
            {
                other.GetComponent<CristalHealth>().ReceiveHit((int)m_damage);
            }
        }



    }


}
