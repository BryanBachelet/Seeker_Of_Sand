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

[Serializable]
public struct CharacterStat
{
    public AgentStat baseStat;
    public float attrackness;
    public float luck;

    public static CharacterStat operator +(CharacterStat a, CharacterStat b)
    {
        CharacterStat result = new CharacterStat();
        result.baseStat = a.baseStat + b.baseStat;
        result.attrackness = a.attrackness + b.attrackness;
        result.luck = a.luck + b.luck;
        return result;
    }
    public static CharacterStat operator -(CharacterStat a, CharacterStat b)
    {
        CharacterStat result = new CharacterStat();
        result.baseStat = a.baseStat - b.baseStat;
        result.attrackness = a.attrackness - b.attrackness;
        result.luck = a.luck - b.luck;
        return result;
    }
};

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Agent Profile", order = 1)]
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




