using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct AgentStat
{
    public int healthMax;
    public float speed;
    public float attackSpeed;

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
    int m_currentHealth;

    public int health { get { return m_currentHealth; } private set { } }

    public delegate void HealthUpdateEvent(int currentHealth);
    public HealthUpdateEvent healthUpdate;

    public void SetMaxHealth(int maxHealth)
    {
        m_currentHealth = maxHealth;
        healthUpdate.Invoke(m_currentHealth);
    }

    public void ChangeCurrentHealth(int value)
    {
        m_currentHealth += value;
        healthUpdate.Invoke(m_currentHealth);
    }


}




