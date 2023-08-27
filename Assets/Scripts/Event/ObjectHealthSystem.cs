using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventObjectState
{
    Active,
    Deactive,
    Death,
}

public class ObjectHealthSystem : MonoBehaviour
{
   
    [Header("Health Parameters")]
    [SerializeField] private float m_maxHealth;
    [SerializeField] private float m_currentHealth;
    [SerializeField] private float m_invicibleDuration;
    [SerializeField] private bool m_isInvicible;

    private float m_invicibleTimer;

    public EventObjectState eventState = EventObjectState.Deactive;


    public void Update()
    {
        InvicibleCountdown();
        CheckLifeState();
    }

    public void TakeDamage(int damage)
    {
        if (m_isInvicible || eventState != EventObjectState.Active) return;

        m_currentHealth -= damage;
        m_isInvicible = true;
        m_invicibleTimer = 0.0f;
    }

    private void InvicibleCountdown()
    {
        if(m_invicibleTimer>m_invicibleDuration)
        {
            m_isInvicible = false;
        }
        else
        {
            m_invicibleTimer += Time.deltaTime;
        }
    }

    public void ChangeState(EventObjectState newState)
    {
        eventState = newState;
    }

    public void SetMaxHealth(int newMaxHealth)
    {
        m_maxHealth = newMaxHealth;
    }

    public void ResetCurrentHealth()
    {
        m_currentHealth = m_maxHealth;
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag != "Enemy") return;
        TakeDamage(1);
    }

    public void CheckLifeState()
    {
        if(m_currentHealth <0.0f)
        {
            eventState = EventObjectState.Death;
        }
    }

    public bool IsEventActive()
    {
        return eventState == EventObjectState.Active;
    }
}
