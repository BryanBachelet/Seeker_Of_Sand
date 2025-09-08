using GuerhoubaGames.GameEnum;
using SpellSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuerhoubaGames.Artefact
{
    #region Artefact Eneum & Struct Data

    public enum ArtefactType
    {
        Spawner = 0,
        Effect = 1,
        Stats = 2,
    }

    [System.Serializable]
    public struct ArtefactVisualData
    {
        public Sprite icon;
    }

    #endregion


    [System.Serializable]
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/New Artefacts Info", order = 2)]
    public class ArtefactBaseInfos : ScriptableObject
    {
        [Header("Base Information")]
        public string name = "";
        [SerializeField] private int m_id = 0;
        [SerializeField] private int m_idFamily;
        [TextArea] public string description = "";

        [Tooltip("Description shown in game")]
        [TextArea] public string shownDescription = "";

        [HideInInspector] public ArtefactType type;

        [Space]
        public GameElement element;
        public ArtefactVisualData visualData;

        [Header("Upgrade Feature Variable")]
        [SerializeField] private bool m_isUpgradePossible;
        public LevelTier levelTier;

        [Header("Reinforcement Feature Variable")]
        [SerializeField] private bool m_isReinforcementPossible;

        [Header("Merge Feature Variable")]
        [SerializeField] private bool m_isMergePossible;
        private bool m_hasBeenMergeOnce = false;

        [Header("Conditional Variables")]
        public bool hasCondition;
        [ShowIf("hasCondition", true, true)] public ConditionsTrigger conditionsTrigger;
        [ShowIf("hasCondition", true, true)] public EntitiesTrigger entitiesTrigger;
        [ShowIf("hasCondition", true, true)] public EntitiesTargetSystem EntitiesTargetSystem;

        [Header("Conditional Variables")]
        public bool hasStat;
        [ShowIf("hasStat", true, true)] public PlayerEffectStats<StatDataArtefact> EffectStats;



        public ArtefactBaseInfos Clone()
        {
            ArtefactBaseInfos clone = Instantiate(this);
            return clone;
        }

        public bool IsSameFragment(ArtefactBaseInfos artefactBaseInfos)
        {
            if (artefactBaseInfos.m_id == m_id) return true;
            return false;

        }

        public virtual void AddAdditionalArtefact(ArtefactBaseInfos artefactBaseInfos)
        {

        }

        public virtual void MergeFragment(ArtefactBaseInfos artefactBaseInfos)
        {

        }

        public virtual void UpdateTierFragment()
        {

        }


        public void ResultString()
        {
            string result = string.Empty;

            if (description == null)
            {
                shownDescription = description;
                return;
            }
            string[] bracketArray = description.Split("{");
            if (bracketArray == null)
            {
                shownDescription = description;
                return;
            }

            int countBracket = bracketArray.Length - 1;
            int indexEndString = 0;

            if (countBracket <= 0)
            {
                shownDescription = description;
                return;
            }
            int indexStart = description.IndexOf("{");


            //for (int i = 0; i < countBracket; i++)
            //{


            //    int indexStartProperty = description.IndexOf("{", indexEndString);
            //    if (indexStartProperty == -1)
            //    {

            //        shownDescription = description;
            //        return;

            //    }
            //    int indexEndProperty = description.IndexOf("}", indexEndString + 1);
            //    if (indexEndProperty == -1)
            //    {
            //        Debug.LogError("Artefact string missing }");
            //        shownDescription = description.Substring(0, indexStartProperty);
            //        return;
            //    }

            //    if (indexEndProperty - indexStartProperty < 0)
            //    {
            //        Debug.LogError("Artefact string missing {");
            //        shownDescription = description;
            //        return;
            //    }

            //    result += description.Substring(indexEndString, indexStartProperty - indexEndString);
            //    indexEndString = indexEndProperty + 1;
            //    string substringCode = description.Substring(indexStartProperty + 1, indexEndProperty - indexStartProperty - 1);
            //    if (substringCode == "probability")
            //    {
            //        result += spawnRatePerTier[(int)levelTierFragment].ToString();

            //        continue;
            //    }

            //    if (substringCode == "damage")
            //    {
            //        result += (damageArtefact + damageGainPerCount * additionialItemCount).ToString();
            //        continue;
            //    }



            //}

            result += description.Substring(indexEndString);
            shownDescription = result;
            return;
        }

        public void OnValidate()
        {
            ResultString();
        }



    }
}