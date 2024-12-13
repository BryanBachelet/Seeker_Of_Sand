using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.GameEnum;
using GuerhoubaGames.VFX;
using Enemies;

namespace SpellSystem
{

    public class AreaDotBehavior : MonoBehaviour
    {
        private ObjectState state;
        private AreaMeta m_areaMeta;
        private DOTMeta m_DotMeta;
        private SummonsMeta m_summonMeta;

        private float m_hitFrequencyTime = 0.0f;
        private float m_hitFrequencyTimer = 0.0f;
        private int m_hitMaxCount = 0;
        private int m_hitCount = 0;
        private float m_sizeArea = 1.0f;
        private int m_damage = 0;
        private GameElement m_element;

        public bool isLimitTarget;
        private float m_currentAreaLifetime;

        [Header("Feedback Paramets")]
        public int indexSFX;
        public float damageDelay;
        public VFX_Spawner m_vfxSpwaner;


        private float hitDelayTimer;
        private bool canHit;

        [Header("Debug Paraemters")]
        public bool isDebugActive;
        public Color color;
        [Range(0, 1)] public float transparency = 0.5f;
        private SpellProfil profil;

        #region Unity Functions
        // Start is called before the first frame update

        public void Awake()
        {
            m_areaMeta = GetComponent<AreaMeta>();
            m_DotMeta = GetComponent<DOTMeta>();
          
            m_areaMeta.OnSpawn += InitComponent;
            m_areaMeta.OnRelaunch += RelaunchComponent;
        }


        // Update is called once per frame
        void Update()
        {
            UpdateArea();
        }
        #endregion

        public void InitComponent()
        {
            GlobalSoundManager.PlayOneShot(indexSFX, transform.position);
            if (m_DotMeta.dotData.spellProfil.tagData.EqualsSpellNature(SpellNature.SUMMON))
                m_summonMeta = GetComponent<SummonsMeta>();

            profil = m_areaMeta.areaData.spellProfil;
            m_sizeArea = profil.GetFloatStat(StatType.Size);
            m_damage = profil.GetIntStat(StatType.Damage);
            m_element = profil.tagData.element;

            if (profil.tagData.EqualsSpellNature(SpellNature.DOT))
            {
                m_hitFrequencyTime = profil.GetFloatStat(StatType.HitFrequency);
                m_hitMaxCount = m_DotMeta.dotData.currentHitCount;
            }

            if (profil.tagData.EqualsSpellParticularity(SpellParticualarity.Explosion))
            {
                m_sizeArea = profil.GetIntStat(StatType.SizeExplosion);
                m_damage += profil.GetIntStat(StatType.DamageAdditionel);
            }

            if (m_DotMeta.dotData.spellProfil.tagData.EqualsSpellNature(SpellNature.SUMMON))
            {
                m_hitMaxCount = (int)(profil.GetFloatStat(StatType.LifeTimeSummon) / m_hitFrequencyTime);
                m_summonMeta.OnSpecialSkill += ApplyAreaDamage;
            }
        }

        public void RelaunchComponent()
        {
            m_hitCount = 0;  
        }

        public void UpdateArea()
        {

            AreaMouvement();
            if (hitDelayTimer >= damageDelay)
            {
                canHit = true;
            }
            else
            {
                m_currentAreaLifetime += Time.deltaTime;
                hitDelayTimer += Time.deltaTime;
                if (m_vfxSpwaner) m_vfxSpwaner.UpdateTimeValue();
            }

            if (!canHit) return;
            if (m_hitCount == m_hitMaxCount)
            {
                Destroy(this.gameObject);
            }

            if (m_hitFrequencyTimer > m_hitFrequencyTime)
            {
                ApplyAreaDamage();
                m_hitCount++;
                m_hitFrequencyTimer = 0.0f;
            }
            else
            {
                m_hitFrequencyTimer += Time.deltaTime;
                m_currentAreaLifetime += Time.deltaTime;
                if (m_vfxSpwaner) m_vfxSpwaner.UpdateTimeValue();
            }


        }

        public void AreaMouvement()
        {
            if (profil.tagData.spellMovementBehavior == SpellMovementBehavior.OnSelf)
            {
                transform.position = m_areaMeta.areaData.characterShoot.transform.position + new Vector3(0, 10, 0);
            }
        }

        public void ApplyAreaDamage()
        {
            Collider[] collider = Physics.OverlapSphere(transform.position, m_sizeArea, GameLayer.instance.enemisLayerMask);

            if (collider.Length == 0) return;

            if (!isLimitTarget)
            {
                for (int i = 0; i < collider.Length ; i++)
                {
                    NpcHealthComponent npcHealthComponent = collider[i].GetComponent<NpcHealthComponent>();
                    Vector3 direction = collider[i].transform.position - transform.position;
                    if (npcHealthComponent)
                    {
                        m_areaMeta.areaData.characterShoot.ActiveOnHit(collider[i].transform.position, EntitiesTrigger.Enemies, collider[i].gameObject, m_element);
                        DamageStatData damageStatData = new DamageStatData(m_damage, m_areaMeta.areaData.objectType);
                        npcHealthComponent.ReceiveDamage(profil.name, damageStatData, direction, 10, (int)m_element, (int)CharacterProfile.instance.stats.baseStat.damage);
                        if(m_DotMeta.OnDamage != null) m_DotMeta.OnDamage.Invoke(npcHealthComponent.transform.position);
                    }
                }
            }
            else

            {
                List<Collider> colliderDraw = new List<Collider>(collider);

              
                for (int i = 0;  i < profil.GetIntStat(StatType.AreaTargetSimulately); i++)
                {
                    int index = Random.Range(0, colliderDraw.Count);
                    NpcHealthComponent npcHealthComponent = colliderDraw[index].GetComponent<NpcHealthComponent>();
                    Vector3 direction = colliderDraw[index].transform.position - transform.position;
                    if (npcHealthComponent)
                    {
                        m_areaMeta.areaData.characterShoot.ActiveOnHit(collider[i].transform.position, EntitiesTrigger.Enemies, collider[i].gameObject, m_element);
                        DamageStatData damageStatData = new DamageStatData(m_damage, m_areaMeta.areaData.objectType);
                        npcHealthComponent.ReceiveDamage(profil.name, damageStatData, direction, 10, (int)m_element, (int)CharacterProfile.instance.stats.baseStat.damage);
                        m_DotMeta.OnDamage.Invoke(npcHealthComponent.transform.position);
                    }

                    colliderDraw.RemoveAt(index);
                    if (colliderDraw.Count == 0) break;
                }
            }
        }

        public float GetCurrentLifeTime() { return m_currentAreaLifetime; }

        public void OnDrawGizmos()
        {
            if (!isDebugActive) return;

            Gizmos.color = new Color(color.r, color.g, color.b, transparency);
            Gizmos.DrawSphere(transform.position, m_sizeArea);
        }

    }
}
