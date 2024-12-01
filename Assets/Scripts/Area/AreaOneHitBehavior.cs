using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.GameEnum;
using GuerhoubaGames.Resources;
using Enemies;
using System;

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

        [Header("Debug Paraemters")]
        public bool isDebugActive;
        public Color color;
        [Range(0, 1)] public float transparency = 0.5f;

        public bool asPreviewDecal;
        public Texture2D previewDecal;
        private SpellProfil profil;
       
       

        public void Awake()
        {
            m_areaMeta = GetComponent<AreaMeta>();
            m_areaMeta.OnSpawn += InitAreaData;
        }

        public void InitAreaData()
        {
             profil = m_areaMeta.areaData.spellProfil;
            m_sizeArea = profil.GetFloatStat(StatType.Size);
            m_damage = profil.GetIntStat(StatType.Damage);
            m_element = profil.tagData.element;

            if (profil.tagData.EqualsSpellParticularity(SpellParticualarity.Explosion))
            {
                m_sizeArea = profil.GetIntStat(StatType.SizeExplosion);
                m_damage += profil.GetIntStat(StatType.DamageAdditionel);
            }
        }

        void Update()
        {
            if (m_timerBeforeHit > timeBeforeHit)
            {
                ApplyAreaDamage();
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

            
            if (areaType == AreaType.CIRCLE) collider = Physics.OverlapSphere(transform.position, m_sizeArea, GameLayer.instance.enemisLayerMask);
            if (areaType == AreaType.RECT)
            {
                Vector3 halfArea = new Vector3(
                   (baseSize.x * Convert.ToInt32(!Xsize)) + m_sizeArea * Convert.ToInt32(Xsize),
                   (baseSize.y * Convert.ToInt32(!Ysize)) + m_sizeArea * Convert.ToInt32(Ysize),
                   (baseSize.z * Convert.ToInt32(!Zsize)) + m_sizeArea * Convert.ToInt32(Zsize)
                   );

                Vector3 position = transform.position + transform.forward * halfArea.z;

                collider = Physics.OverlapBox(position, halfArea, transform.rotation, GameLayer.instance.enemisLayerMask);
            }

            for (int i = 0; i < collider.Length; i++)
            {
                NpcHealthComponent npcHealthComponent = collider[i].GetComponent<NpcHealthComponent>();
                Vector3 direction = collider[i].transform.position - transform.position;

                m_areaMeta.areaData.characterShoot.ActiveOnHit(collider[i].transform.position, EntitiesTrigger.Enemies, collider[i].gameObject, m_element);
                DamageStatData damageStatData = new DamageStatData(m_damage, m_areaMeta.areaData.objectType);
                npcHealthComponent.ReceiveDamage(profil.name, damageStatData, direction, 10, (int)m_element, 0);
            }
        }

        protected virtual void ResetArea()
        {
            m_timerBeforeHit = 0.0f;
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

            Gizmos.color = new Color(color.r, color.g, color.b, transparency);
            if (areaType == AreaType.CIRCLE)
            {
                Gizmos.DrawSphere(transform.position, m_sizeArea);
            }
            if (areaType == AreaType.RECT)
            {
                Gizmos.matrix =  transform.localToWorldMatrix;
                Vector3 halfArea = new Vector3(
                    (baseSize.x * Convert.ToInt32(!Xsize)) + m_sizeArea * Convert.ToInt32(Xsize),
                    (baseSize.y* Convert.ToInt32(!Ysize)) +  m_sizeArea * Convert.ToInt32(Ysize),
                    (baseSize.z * Convert.ToInt32(!Zsize)) + m_sizeArea * Convert.ToInt32(Zsize)
                    );

                Vector3 position = Vector3.forward *( halfArea.z /2.0f);
                Gizmos.DrawCube(position, halfArea);
            }
        }
    }
}
