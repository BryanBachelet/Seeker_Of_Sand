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

};

[Serializable]
public struct CharacterStat
{
    public AgentStat baseStat;
    public float attrackness;
    public float luck;
};

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Agent Profile", order = 1)]
public class AgentProfile : ScriptableObject
{
    public AgentStat agentStat;
}



public class HealthSystem
{
    float m_currentHealth;

    public float health { get { return m_currentHealth; } private set { } }

    public delegate void HealthUpdateEvent(float currentHealth);
    public event HealthUpdateEvent healthUpdate;

    public void Setup(float maxHealth)
    {
        m_currentHealth = maxHealth;
    }

    public void SetMaxHealth(int maxHealth)
    {
        m_currentHealth = maxHealth;
        healthUpdate?.Invoke(m_currentHealth);
    }

    public void ChangeCurrentHealth(float value)
    {
        m_currentHealth += value;
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




