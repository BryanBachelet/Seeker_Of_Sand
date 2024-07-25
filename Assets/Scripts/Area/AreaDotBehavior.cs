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
        private AreaMeta m_areaData;
        private DOTMeta m_DotMeta;

        private float m_hitFrequencyTime = 0.0f;
        private float m_hitFrequencyTimer = 0.0f;
        private int m_hitMaxCount = 0;
        private int m_hitCount = 0;
        private float m_sizeArea = 1.0f;
        private int m_damage = 0;
        private GameElement m_element;


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


        #region Unity Functions
        // Start is called before the first frame update
        void Start()
        {
            m_areaData = GetComponent<AreaMeta>();
            m_DotMeta = GetComponent<DOTMeta>();
            GlobalSoundManager.PlayOneShot(indexSFX, transform.position);
            InitComponent();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateArea();
        }
        #endregion
       
        public void InitComponent()
        {
            SpellProfil profil = m_areaData.areaData.spellProfil;
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
        }

        public void UpdateArea()
        {

            if(hitDelayTimer>damageDelay)
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
            if(m_hitCount == m_hitMaxCount)
            {
                Destroy(this.gameObject);
            }

            if(m_hitFrequencyTimer>m_hitFrequencyTime)
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
         
        public void ApplyAreaDamage()
        {
            Collider[] collider = Physics.OverlapSphere(transform.position, m_sizeArea, GameLayer.instance.enemisLayerMask);

            for (int i = 0; i < collider.Length; i++)
            {
                NpcHealthComponent npcHealthComponent = collider[i].GetComponent<NpcHealthComponent>();
                Vector3 direction = collider[i].transform.position - transform.position;
             if(npcHealthComponent)   npcHealthComponent.ReceiveDamage(m_damage, direction, 10, (int)m_element);
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
