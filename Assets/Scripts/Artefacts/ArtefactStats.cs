using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Sprites;
using UnityEngine;

namespace GuerhoubaGames.Artefact
{


    [System.Serializable]
    [CreateAssetMenu(fileName = " Artefact Stats Data", menuName = "Artefacts/Artefact Stas", order = 2)]
    public class ArtefactStats : ArtefactBaseInfos
    {
        [Header("Stats Conditions")]
        [SerializeField] private bool isPermanent;
        [ShowIf("isPermanent", false, true)] public bool isTemporary;
        [ShowIf("isTemporary", true, true)] public float buffDuration = 0;
        private float m_buffTimer;
        private bool m_isBuffActive;

        public ArtefactStats()
        {
            type = ArtefactType.Stats;
            m_isReinforcementPossible = true;
            m_isUpgradePossible = true;
            m_isMergePossible = false;
        }

        public new ArtefactStats Clone()
        {
            return Instantiate(this);
        }

        public bool IsPermanent() { return isPermanent; }

        public override void AddAdditionalArtefact(ArtefactBaseInfos artefactBaseInfos)
        {
            base.AddAdditionalArtefact(artefactBaseInfos);

            EffectStats.ChangeStats(artefactBaseInfos.EffectStats,levelTier);
        }

        public override void IncreaseTierFragment()
        {
            base.IncreaseTierFragment();

        }

        public override void ActiveCondition(ConditionData conditionData)
        {
            base.ActiveCondition(conditionData);

            if (!isTemporary && isPermanent) return;
            m_isBuffActive = true;
            m_buffTimer = 0;
            bool isTemporaryValid = true;
            characterArtefact.SetupStatsArtefacts(this, isTemporaryValid);
        }

        #region Specific Functions

        public override void UpdateArtefactItem()
        {
            base.UpdateArtefactItem();

            if (!m_isBuffActive) return;

            if(m_buffTimer>buffDuration)
            {
                m_isBuffActive = false;
                characterArtefact.RemoveStatsArtefacts(this);
            }
            else
            {
                m_buffTimer += Time.deltaTime;
            }
        }

        #endregion

    }
}