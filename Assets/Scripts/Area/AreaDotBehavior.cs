using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.GameEnum;
using GuerhoubaGames.VFX;
using Enemies;
using GuerhoubaGames;

namespace SpellSystem
{

    public class AreaDotBehavior : MonoBehaviour
    {
        private ObjectState state;
        private AreaMeta m_areaMeta;
        private MultiHitAreaMeta m_DotMeta;
        private SummonsMeta m_summonMeta;

        private float m_hitFrequencyTime = 0.0f;
        private float m_hitFrequencyTimer = 0.0f;
        private int m_hitMaxCount = 0;
        public int hitCount = 0;
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

        private DamageCalculComponent m_damageCalculComponent;

        #region Unity Functions
        // Start is called before the first frame update

        public void Awake()
        {
            m_areaMeta = GetComponent<AreaMeta>();
            m_DotMeta = GetComponent<MultiHitAreaMeta>();

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
            profil = m_areaMeta.areaData.spellProfil;
            if (profil.tagData.EqualsSpellNature(SpellNature.SUMMON))
                m_summonMeta = GetComponent<SummonsMeta>();

            m_sizeArea = profil.GetFloatStat(StatType.Size);
            m_damage = profil.GetIntStat(StatType.Damage);
            m_element = profil.tagData.element;
            m_damageCalculComponent = GetComponent<DamageCalculComponent>();
            if (profil.tagData.EqualsSpellNature(SpellNature.MULTI_HIT_AREA))
            {
                m_hitFrequencyTime = profil.GetFloatStat(StatType.HitFrequency);
                m_hitMaxCount = m_DotMeta.dotData.currentMaxHitCount;
            }

            if (profil.tagData.EqualsSpellParticularity(SpellParticualarity.Explosion))
            {
                m_sizeArea = profil.GetFloatStat(StatType.SizeExplosion);
                m_damage += profil.GetIntStat(StatType.DamageAdditionel);
            }

            if (profil.tagData.EqualsSpellNature(SpellNature.SUMMON))
            {
                m_hitMaxCount = (int)(profil.GetFloatStat(StatType.LifeTimeSummon) / m_hitFrequencyTime);
                m_summonMeta.OnSpecialSkill += ApplyAreaDamage;
            }
        }

        public void RelaunchComponent()
        {
            hitCount = 0;
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
            if (hitCount == m_hitMaxCount)
            {
                Destroy(this.gameObject);
            }

            if (m_hitFrequencyTimer > m_hitFrequencyTime)
            {
                ApplyAreaDamage();
                hitCount++;
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

            if (profil.tagData.spellMovementBehavior == SpellMovementBehavior.Direction || profil.tagData.spellMovementBehavior == SpellMovementBehavior.FollowMouse)
            {
                RaycastHit hit = new RaycastHit();

                if (Physics.Raycast(transform.position, -Vector3.up, out hit, Mathf.Infinity, GameLayer.instance.groundLayerMask))
                {
                    float distance = Vector3.Distance(hit.point, transform.position);

                    float deltaDistance = 10 - distance;
                    // deltaDistance = Mathf.Clamp(deltaDistance, 0, 10.0f);


                    transform.position += Vector3.up * deltaDistance;

                }


                if (profil.tagData.spellMovementBehavior == SpellMovementBehavior.Direction)
                    transform.position += m_areaMeta.areaData.direction.normalized * profil.GetFloatStat(StatType.DirectionSpeed) * Time.deltaTime;
                if (profil.tagData.spellMovementBehavior == SpellMovementBehavior.FollowMouse)
                {
                    Character.CharacterAim characterAim = m_areaMeta.areaData.characterShoot.GetComponent<Character.CharacterAim>();
                    Vector3 direction = characterAim.lastRawPosition - transform.position;
                    transform.position += direction.normalized * profil.GetFloatStat(StatType.DirectionSpeed) * Time.deltaTime;
                }

            }
        }

        public void ApplyAreaDamage()
        {
            Collider[] collider = Physics.OverlapSphere(transform.position, m_sizeArea, GameLayer.instance.enemisLayerMask + GameLayer.instance.interactibleLayerMask);

            if (collider.Length == 0) return;

            if (!isLimitTarget)
            {
                for (int i = 0; i < collider.Length; i++)
                {
                   
                    IDamageReceiver npcHealthComponent = collider[i].GetComponent<IDamageReceiver>();
                    Vector3 direction = collider[i].transform.position - transform.position;
                    if (npcHealthComponent != null)
                    {
                        m_damageCalculComponent.damageStats.AddDamage(m_damage, m_element, DamageType.TEMPORAIRE);
                        DamageStatData[] damageStatDatas = m_damageCalculComponent.CalculDamage(m_element, m_areaMeta.areaData.objectType, collider[i].gameObject, m_areaMeta.areaData.spellProfil);

                        for (int j = 0; j < damageStatDatas.Length; j++)
                        {
                            AfflictionManager.AfflictionDrawData[] afflictionToApplyArray = AfflictionManager.DrawAfflictionApplication(m_areaMeta.areaData.spellProfil);
                            npcHealthComponent.GetAfflictionManager().AddAfflictions(afflictionToApplyArray);
                            npcHealthComponent.ReceiveDamage(profil.name, damageStatDatas[j], collider[i].transform.position - transform.position, 10, (int)damageStatDatas[j].element, (int)CharacterProfile.GetCharacterStat().baseDamage.totalValue);
                        }
                        if (m_DotMeta.OnDamage != null) m_DotMeta.OnDamage.Invoke(collider[i].transform.position);
                    }
                }
            }
            else

            {
                List<Collider> colliderDraw = new List<Collider>(collider);


                for (int i = 0; i < profil.GetIntStat(StatType.AreaTargetSimulately); i++)
                {
                    int index = Random.Range(0, colliderDraw.Count);
                    IDamageReceiver npcHealthComponent = colliderDraw[index].GetComponent<IDamageReceiver>();
                    Vector3 direction = colliderDraw[index].transform.position - transform.position;
                    if (npcHealthComponent != null)
                    {
                        m_damageCalculComponent.damageStats.AddDamage(m_damage, m_element, DamageType.TEMPORAIRE);
                        DamageStatData[] damageStatDatas = m_damageCalculComponent.CalculDamage(m_element, m_areaMeta.areaData.objectType, colliderDraw[index].gameObject, m_areaMeta.areaData.spellProfil);

                        for (int j = 0; j < damageStatDatas.Length; j++)
                        {
                            AfflictionManager.AfflictionDrawData[] afflictionToApplyArray = AfflictionManager.DrawAfflictionApplication(m_areaMeta.areaData.spellProfil);
                            npcHealthComponent.GetAfflictionManager().AddAfflictions(afflictionToApplyArray);
                            npcHealthComponent.ReceiveDamage(profil.name, damageStatDatas[j], collider[i].transform.position - transform.position, 10, (int)damageStatDatas[i].element, (int)CharacterProfile.GetCharacterStat().baseDamage.totalValue);
                        }
                        m_DotMeta.OnDamage.Invoke(colliderDraw[index].transform.position);
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
