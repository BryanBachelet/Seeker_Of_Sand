using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames;
using GuerhoubaGames.GameEnum;

namespace Character
{

    public class CharacterDamageComponent : MonoBehaviour
    {
        public DamageStats m_damageStats;

        public void AddDamage(int damage, GameElement element,DamageType damageType)
        {
            m_damageStats.AddDamage(damage, element, damageType);
        }
    }
}