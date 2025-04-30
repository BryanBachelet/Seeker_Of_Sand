using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct AgentStat
{

    public float healthMax;
    public float speed;
    public int armor;
    public float regeneration;
    public float damage;

    public static AgentStat operator +(AgentStat a ,AgentStat b)
    {
        AgentStat result = new AgentStat();
        result.armor = a.armor + b.armor;
        result.healthMax = a.healthMax + b.healthMax;
        result.speed = a.speed + b.speed;
        result.regeneration = a.regeneration + b.regeneration;
        result.damage = a.damage + b.damage;
        return result;
    }
    public static AgentStat operator -(AgentStat a, AgentStat b)
    {
        AgentStat result = new AgentStat();
        result.armor = a.armor - b.armor;
        result.healthMax = a.healthMax - b.healthMax;
        result.speed = a.speed - b.speed;
        result.regeneration = a.regeneration - b.regeneration;
        result.damage = a.damage - b.damage;
        return result;
    }

};

[System.Serializable]
[CreateAssetMenu(fileName = "CharacterStatPreset", menuName = "Stats/Character Stat Preset", order = 2)]
public class CharacterStatPreset : ScriptableObject
{
    public CharacterStat preset;
}
[System.Serializable]
public class GameCharacterStats
{
    public int statsValue;
    public float modificatorPercent;
    public int totalValue { get { return Mathf.RoundToInt(statsValue * ((100f + modificatorPercent) / 100)); } }
    public float percent { get {  return (100f + modificatorPercent)/100.0f; } }
    public static GameCharacterStats operator +(GameCharacterStats a, GameCharacterStats b)
    {
        GameCharacterStats result = new GameCharacterStats();
         result.statsValue = a.statsValue + b.statsValue;
         result.modificatorPercent = a.modificatorPercent + b.modificatorPercent;

        return result;
    }


    public static GameCharacterStats operator -(GameCharacterStats a, GameCharacterStats b)
    {
        GameCharacterStats result = new GameCharacterStats();
        result.statsValue = a.statsValue - b.statsValue;
        result.modificatorPercent = a.modificatorPercent - b.modificatorPercent;

        return result;
    }
}

[Serializable]
public struct CharacterStat  
{
    public GameCharacterStats healthMax;
    public GameCharacterStats runSpeed;
    public GameCharacterStats combatSpeed;
    public GameCharacterStats armor;
    public GameCharacterStats regeneration;
    public GameCharacterStats baseDamage;
    public GameCharacterStats airDamage;
    public GameCharacterStats waterDamage;
    public GameCharacterStats fireDamage;
    public GameCharacterStats earthDamage;
    public GameCharacterStats attrackness;
    public GameCharacterStats luck;



    public static CharacterStat operator +(CharacterStat a, CharacterStat b)
    {
        CharacterStat result = new CharacterStat();
        result.armor = a.armor + b.armor;
        result.healthMax = a.healthMax + b.healthMax;
        result.runSpeed = a.runSpeed + b.runSpeed;
        result.combatSpeed = a.combatSpeed + b.combatSpeed;
        result.regeneration = a.regeneration + b.regeneration;
        result.baseDamage = a.baseDamage + b.baseDamage;
        result.baseDamage = a.baseDamage + b.baseDamage;
        result.airDamage = a.airDamage + b.airDamage;
        result.fireDamage = a.fireDamage + b.fireDamage;
        result.waterDamage = a.waterDamage + b.waterDamage;
        result.earthDamage = a.earthDamage + b.earthDamage;
        result.attrackness = a.attrackness + b.attrackness;
        result.luck = a.luck + b.luck;
        return result;
    }


    public static CharacterStat operator -(CharacterStat a, CharacterStat b)
    {
        CharacterStat result = new CharacterStat();
        result.armor = a.armor - b.armor;
        result.healthMax = a.healthMax - b.healthMax;
        result.runSpeed = a.runSpeed - b.runSpeed;
        result.regeneration = a.regeneration - b.regeneration;
        result.baseDamage = a.baseDamage - b.baseDamage;
        result.baseDamage = a.baseDamage - b.baseDamage;
        result.airDamage = a.airDamage- b.airDamage;
        result.fireDamage = a.fireDamage - b.fireDamage;
        result.waterDamage = a.waterDamage - b.waterDamage;
        result.earthDamage = a.earthDamage - b.earthDamage;
        result.attrackness = a.attrackness - b.attrackness;
        result.luck = a.luck - b.luck;
        return result;
    }

};

public class AgentProfile : ScriptableObject
{
    public AgentStat agentStat;
}



public class HealthSystem
{
   private float m_currentHealth;
    private float m_healthPercent;
    private float m_maxHealth;
    public float health { get { return m_currentHealth; } private set { } }
    public float percentHealth { get { return m_healthPercent; } private set { } }
    public float maxHealth { get { return m_maxHealth; } private set { } }

    public delegate void HealthUpdateEvent(float currentHealth);
    public event HealthUpdateEvent healthUpdate;

    public void Setup(float maxHealth)
    {
        m_currentHealth = maxHealth;
        m_maxHealth = maxHealth;
        m_healthPercent = m_currentHealth / m_maxHealth;


    }


    public void SetMaxHealth(int maxHealth)
    {
        m_currentHealth = maxHealth;
        healthUpdate?.Invoke(m_currentHealth);
    }

    public void ChangeCurrentHealth(float value)
    {
        m_currentHealth += value;
        m_healthPercent = m_currentHealth / m_maxHealth;
        healthUpdate?.Invoke(m_currentHealth);
    }

}

public class ArmorSystem
{ 
    public float ApplyArmor(float damage, float armor)
    {
        damage -= armor;
        if (damage <= 0) damage = 1;
        return damage;
    }
}




