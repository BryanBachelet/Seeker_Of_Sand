using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames;
using GuerhoubaGames.GameEnum;

namespace GuerhoubaGames.Character
{

    public class CharacterDamageComponent : MonoBehaviour, CharacterComponent
    {
        public DamageStats m_damageStats;

        public void AddDamage(int damage, GameElement element, DamageType damageType)
        {
            m_damageStats.AddDamage(damage, element, damageType);
        }
        public void AddGameStatDamage(GameCharacterStats gameCharacterStatsDamage, GameElement element, DamageType damageType)
        {
            m_damageStats.AddDamage(gameCharacterStatsDamage.statsValue, element, damageType);
            m_damageStats.AddModificator(gameCharacterStatsDamage.modificatorPercent, element, damageType);
        }
        public void InitComponentStat(CharacterStat stat)
        {

        }

        public void ResetDamage()
        {
            m_damageStats.ResetTempDamage();
        }

        public void UpdateComponentStat(CharacterStat stat)
        {
            m_damageStats.ResetBonusDamage();
            AddGameStatDamage(stat.fireDamage, GameElement.FIRE, DamageType.BONUS);
            AddGameStatDamage(stat.waterDamage, GameElement.FIRE, DamageType.BONUS);
            AddGameStatDamage(stat.airDamage, GameElement.FIRE, DamageType.BONUS);
            AddGameStatDamage(stat.earthDamage, GameElement.FIRE, DamageType.BONUS);

        }
    }
}