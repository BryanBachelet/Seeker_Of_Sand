using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.GameEnum;
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
        public bool isExplotion;
        [Header("Feedback Paramets")]
        public int indexSFX;

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

            if (profil.tagData.spellNatureType == SpellNature.DOT)
            {
                m_hitFrequencyTime = profil.GetFloatStat(StatType.HitFrequency);
                m_hitMaxCount = m_DotMeta.dotData.currentHitCount;
            }

            if (isExplotion)
            {
                m_sizeArea = profil.GetIntStat(StatType.SizeExplosion);
                m_damage += profil.GetIntStat(StatType.DamageAdditionel);
            }
        }

        public void UpdateArea()
        {
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
            }

            
        }

        public void ApplyAreaDamage()
        {
            Collider[] collider = Physics.OverlapSphere(transform.position, m_sizeArea, GameLayer.instance.enemisLayerMask);

            for (int i = 0; i < collider.Length; i++)
            {
                NpcHealthComponent npcHealthComponent = collider[i].GetComponent<NpcHealthComponent>();
                Vector3 direction = collider[i].transform.position - transform.position;
                npcHealthComponent.ReceiveDamage(m_damage, direction, 10, (int)m_element);
            }
        }
    }
}
