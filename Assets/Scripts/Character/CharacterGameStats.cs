using GuerhoubaGames.GameEnum;
using SpellSystem;
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
        
        [SerializeField] private PlayerEffectStats<StatData> m_playerStats;

        #region Unity Functions
        public void Awake()
        {
            if (instance == null)
                instance = this;

            PlayerProfilData playerProfilDataClone = m_playerProfilData.Clone(); 
            m_playerStats = playerProfilDataClone.stats;
            m_playerStats.InitialisationPlayerStats();
        }
        #endregion


        public PlayerEffectStats<StatData> GetPlayerStats() { return m_playerStats; }


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