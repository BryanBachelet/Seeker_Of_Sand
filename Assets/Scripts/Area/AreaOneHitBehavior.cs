using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.GameEnum;
using GuerhoubaGames.Resources;
using Enemies;

namespace SpellSystem
{
    public class AreaOneHitBehavior : MonoBehaviour
    {
        private AreaMeta m_areaData;
        public float timeBeforeHit = 0.5f;
        private float m_timerBeforeHit;
        private float  m_sizeArea = 0;
        private int m_damage = 0;
        private GameElement m_element;

        [Header("Debug Paraemters")]
        public bool isDebugActive;
        public Color color;
        [Range(0, 1)] public float transparency = 0.5f;
        
        void Start()
        {
            m_areaData = GetComponent<AreaMeta>();
            InitAreaData();
        }

        public void InitAreaData()
        {
            SpellProfil profil = m_areaData.areaData.spellProfil;
            m_sizeArea = profil.GetFloatStat(StatType.Size);
            m_damage = profil.GetIntStat(StatType.Damage);
            m_element = profil.tagData.element;
            
            if(profil.tagData.EqualsSpellParticularity(SpellParticualarity.Explosion))
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
                Destroy(this.gameObject);
            }
            else
            {
                m_timerBeforeHit += Time.deltaTime;
            }
        }

        public void ApplyAreaDamage()
        {
            Collider[] collider = Physics.OverlapSphere(transform.position, m_sizeArea,GameLayer.instance.enemisLayerMask);

            for (int i = 0; i < collider.Length; i++)
            {
                NpcHealthComponent npcHealthComponent = collider[i].GetComponent<NpcHealthComponent>();
                Vector3 direction = collider[i].transform.position - transform.position;
                npcHealthComponent.ReceiveDamage(m_damage, direction, 10, (int)m_element);
            }
        }

        public void OnDrawGizmos()
        {
            if (!isDebugActive) return;

            Gizmos.color = new Color(color.r, color.g, color.b,transparency);
            Gizmos.DrawSphere(transform.position, m_sizeArea);
        }

    }
}
