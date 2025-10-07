using GuerhoubaGames.GameEnum;
using SpellSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using Tracker;
using UnityEngine;

namespace GuerhoubaGames.Character
{

    public class CharacterGameStats : MonoBehaviour
    {
        public static CharacterGameStats instance;

        [Header("References")]
        [SerializeField] private PlayerProfilData m_playerProfilData;
        
        [SerializeField] private PlayerEffectStats<StatDataArtefact> m_playerStats;

        public Action onChangeStat;

        #region Unity Functions
        public void Awake()
        {
            if (instance == null)
                instance = this;

            PlayerProfilData playerProfilDataClone = m_playerProfilData.Clone();
            m_playerStats.Setup(playerProfilDataClone.stats);
           // m_playerStats.InitialisationPlayerStats();
        }
        #endregion


        public PlayerEffectStats<StatDataArtefact> GetPlayerStats() { return m_playerStats; }

        public void ApplyStatChange()
        {
            onChangeStat?.Invoke();
        }


        #region Static Functions

        public static float GetFloatData(StatType statType)
        {
           return instance.m_playerStats.GetFloatStat(statType);
        }

        public static int GetIntData(StatType statType)
        {
            return instance.m_playerStats.GetIntStat(statType);
        }

        public static bool GetBoolData(StatType statType)
        {
            return instance.m_playerStats.GetBoolStat(statType);
        }

        #endregion




    }

}