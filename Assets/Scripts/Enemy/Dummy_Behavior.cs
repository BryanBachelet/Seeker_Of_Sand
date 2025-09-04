using GuerhoubaGames;
using GuerhoubaGames.Resources;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy_Behavior : MonoBehaviour, IDamageReceiver
{
    private HealthSystem m_healthSystem;

    public HealthManager m_healthManager;
    public int health_ID = 0;
    public Vector2 offsetAdditionnel = new Vector2(0, 0); //Offset du damageFeedback
    public void Start()
    {
        m_healthManager = FindObjectOfType<HealthManager>();
        health_ID = m_healthManager.GenerateNewDamageFD();
    }
    public void ReceiveDamage(string nameDamage, DamageStatData damageStat, Vector3 direction, float power, int element, int additionnal)
    {

        GameStats.instance.AddDamageSource(nameDamage, damageStat);
        // VfX feedback
        Vector3 positionOnScreen = transform.position + new Vector3(0, 5, 0);
        m_healthManager.CallDamageEvent(positionOnScreen, damageStat.damage + additionnal, element, health_ID, offsetAdditionnel);


        //m_entityAnimator.SetTrigger("TakeDamage");
        GlobalSoundManager.PlayOneShot(12, transform.position);

       // if (m_healthSystem.health > 0) return;

    }

    public float GetLifeRatio()
    {
        return m_healthSystem.percentHealth;
    }

    public string GetName()
    {
        return "Dummy";
    }

    public AfflictionManager GetAfflictionManager()
    {
        throw new System.NotImplementedException();
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
    public bool IsDead()
    {
        return false;
    }

    public int GetLastingLife()
    {
        return (int)m_healthSystem.health;
    }

    public bool IsObjectifTarget()
    {
        return false;
    }
}
