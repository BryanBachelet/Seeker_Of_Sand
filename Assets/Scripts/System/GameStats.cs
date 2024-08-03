using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.GameEnum;

[System.Serializable]
public struct DamageStatData
{
    public int damage;
    public CharacterObjectType characterObjectType;

    public DamageStatData(int pdamage, CharacterObjectType objectType)
    {
        this.damage = pdamage;
        this.characterObjectType = objectType;
    }
}

public class GameStats : MonoBehaviour
{
    public static GameStats instance;

    public Dictionary<string, DamageStatData> m_damageStat = new Dictionary<string, DamageStatData>();

    public void Awake()
    {
        instance = this;
    }
    
    public int GetDamage(string name)
    {
        if(m_damageStat.ContainsKey(name))
        {
            return m_damageStat[name].damage;
        }else
        {
            return 0;
        }
    }    
    public void AddDamageSource(string nameDamageSource, DamageStatData damageToAdd)
    {
        if (m_damageStat.ContainsKey(nameDamageSource))
        {
            DamageStatData data = m_damageStat[nameDamageSource];
            data.damage += damageToAdd.damage;
            m_damageStat[nameDamageSource] = data;
        }
        else
        {
            m_damageStat.Add(nameDamageSource, damageToAdd);
        }
    }
}
