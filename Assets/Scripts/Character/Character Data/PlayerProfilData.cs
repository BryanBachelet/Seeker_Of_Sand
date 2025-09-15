using SpellSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuerhoubaGames.Character
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "PlayerProfilData", menuName = "Player/Player Profil Data", order = 1)]
    public class PlayerProfilData : ScriptableObject
    {
       public PlayerEffectStats<StatData> stats;

        public PlayerProfilData Clone()
        {
             PlayerProfilData playerProfilData = Instantiate(this);
            playerProfilData.stats = stats.Clone();
            return playerProfilData;
        }
    }
}