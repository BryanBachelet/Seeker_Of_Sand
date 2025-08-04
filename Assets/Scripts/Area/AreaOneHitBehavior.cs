using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.GameEnum;
using GuerhoubaGames.Resources;
using Enemies;
using System;
using GuerhoubaGames;
using TMPro;

namespace SpellSystem
{
    public class AreaOneHitBehavior : MonoBehaviour
    {
        private AreaMeta m_areaMeta;
        public AreaType areaType = AreaType.CIRCLE;
        [Header("Box Area Parameters")]
        public Vector3 baseSize;
        public bool Xsize;
        public bool Ysize;
        public bool Zsize;
        public float timeBeforeHit = 0.5f;
        private float m_timerBeforeHit;
        private float m_sizeArea = 0;
        private int m_damage = 0;
        private GameElement m_element;

        private bool isDestroing;

        [Header("Debug Paraemters")]
        public bool isDebugActive;
        public Color color;
        [Range(0, 1)] public float transparency = 0.5f;

        public bool asPreviewDecal;
        public Texture2D previewDecal;
        private SpellProfil profil;

        private DamageCalculComponent m_damageCalculComponent;
        public GameObject ObjectToSpawnAtDeath;

        public float sizeGain = 0.1f;


        public void Awake()
        {
            m_areaMeta = GetComponent<AreaMeta>();
            m_areaMeta.OnSpawn += InitAreaData;
            m_damageCalculComponent = GetComponent<DamageCalculComponent>();
        }

        public void InitAreaData()
        {
            profil = m_areaMeta.areaData.spellProfil;
            m_sizeArea = profil.GetFloatStat(StatType.Size);
            m_damage = profil.GetIntStat(StatType.Damage);
            m_element = profil.tagData.element;
           
;            if (profil.tagData.EqualsSpellParticularity(SpellParticualarity.Explosion))
            {
                m_sizeArea = profil.GetFloatStat(StatType.SizeExplosion);
                m_damage += profil.GetIntStat(StatType.DamageAdditionel);
            }

            transform.localScale += Vector3.one * sizeGain * (int)(m_sizeArea / 10);
        }

        void Update()
        {
            if (isDestroing) return;
            if (m_timerBeforeHit > timeBeforeHit)
            {
                ApplyAreaDamage();


                if (ObjectToSpawnAtDeath != null)
                {

                    GameObject instance = GamePullingSystem.SpawnObject(ObjectToSpawnAtDeath, transform.position, transform.rotation);
                    MultiHitAreaMeta dotMeta = instance.GetComponent<MultiHitAreaMeta>();
                    if (dotMeta)
                    {
                        dotMeta.dotData.characterShoot = m_areaMeta.areaData.characterShoot;
                        dotMeta.dotData.currentMaxHitCount = profil.GetIntStat(StatType.HitNumber);
                        dotMeta.dotData.spellProfil = profil;

                    }
                    AreaMeta areaMeta = instance.GetComponent<AreaMeta>();
                    if (areaMeta)
                    {
                        areaMeta.areaData = m_areaMeta.areaData;
                        areaMeta.OnSpawn?.Invoke();
                    }
                   
                }
                isDestroing = true;
                ActiveDeath();
            }
            else
            {
                m_timerBeforeHit += Time.deltaTime;
            }
        }

        public void ApplyAreaDamage()
        {
            Collider[] collider = new Collider[0];


            if (areaType == AreaType.CIRCLE) collider = Physics.OverlapSphere(transform.position, m_sizeArea, GameLayer.instance.enemisLayerMask + GameLayer.instance.interactibleLayerMask);
            if (areaType == AreaType.RECT)
            {
                Vector3 halfArea = new Vector3(
                   (baseSize.x * Convert.ToInt32(!Xsize)) + m_sizeArea * Convert.ToInt32(Xsize),
                   (baseSize.y * Convert.ToInt32(!Ysize)) + m_sizeArea * Convert.ToInt32(Ysize),
                   (baseSize.z * Convert.ToInt32(!Zsize)) + m_sizeArea * Convert.ToInt32(Zsize)
                   );

                Vector3 position = transform.position + transform.forward * halfArea.z;

                collider = Physics.OverlapBox(position, halfArea, transform.rotation, GameLayer.instance.enemisLayerMask + GameLayer.instance.interactibleLayerMask);
            }

            for (int i = 0; i < collider.Length; i++)
            {
                if (collider[i] == null) 
                    return;

                IDamageReceiver npcHealthComponent = collider[i].GetComponent<IDamageReceiver>();
                Vector3 direction = collider[i].transform.position - transform.position;
                m_damageCalculComponent.damageStats.AddDamage(m_damage, m_element, DamageType.TEMPORAIRE);
                DamageStatData[] damageStatDatas = m_damageCalculComponent.CalculDamage(m_element, m_areaMeta.areaData.objectType, collider[i].gameObject, m_areaMeta.areaData.spellProfil);

                for (int j = 0; j < damageStatDatas.Length; j++)
                {
                    if (collider[i] == null)
                        return;

                    AfflictionManager.AfflictionDrawData[] afflictionToApplyArray = AfflictionManager.DrawAfflictionApplication(m_areaMeta.areaData.spellProfil);
                    npcHealthComponent.GetAfflictionManager().AddAfflictions(afflictionToApplyArray);
                    npcHealthComponent.ReceiveDamage(profil.name, damageStatDatas[j], collider[i].transform.position - transform.position, 10, (int)damageStatDatas[j].element, (int)CharacterProfile.instance.stats.baseDamage.totalValue);
                }

            }
        }

        protected virtual void ResetArea()
        {
            m_timerBeforeHit = 0.0f;
            isDestroing = false;
            transform.localScale -= Vector3.one * sizeGain * (int)(m_sizeArea / 10);
        }


        protected virtual void ActiveDeath()
        {
            PullingMetaData pullingMetaData = GetComponent<PullingMetaData>();
            if (GamePullingSystem.instance != null && pullingMetaData != null)
            {
                ResetArea();
                GamePullingSystem.instance.ResetObject(this.gameObject, pullingMetaData.id);
            }
            else
            {
                Destroy(this.gameObject);
            }

        }

        public Texture2D ReturnDecalPreview()
        {
            return previewDecal;
        }
        public void OnDrawGizmos()
        {
            if (!isDebugActive) return;

            Gizmos.color = new Color(color.r, color.g, color.b, color.a);
            if (areaType == AreaType.CIRCLE)
            {
                Gizmos.DrawWireSphere(transform.position, m_sizeArea);
            }
            if (areaType == AreaType.RECT)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Vector3 halfArea = new Vector3(
                    (baseSize.x * Convert.ToInt32(!Xsize)) + m_sizeArea * Convert.ToInt32(Xsize),
                    (baseSize.y * Convert.ToInt32(!Ysize)) + m_sizeArea * Convert.ToInt32(Ysize),
                    (baseSize.z * Convert.ToInt32(!Zsize)) + m_sizeArea * Convert.ToInt32(Zsize)
                    );

                Vector3 position = Vector3.forward * (halfArea.z / 2.0f);
                Gizmos.DrawCube(position, halfArea);
            }
        }
    }
}
