using GuerhoubaGames;
using GuerhoubaGames.Resources;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy_Behavior : MonoBehaviour ,IDamageReceiver
{
    private HealthSystem m_healthSystem;

    public HealthManager m_healthManager;

    public void Start()
    {
        m_healthManager = FindObjectOfType<HealthManager>();
    }
    public void ReceiveDamage(string nameDamage, DamageStatData damageStat, Vector3 direction, float power, int element, int additionnal)
    {

        GameStats.instance.AddDamageSource(nameDamage, damageStat);
        // VfX feedback
        Vector3 positionOnScreen = transform.position + new Vector3(0, 5, 0);
        m_healthManager.CallDamageEvent(positionOnScreen, damageStat.damage + additionnal, element);


        //m_entityAnimator.SetTrigger("TakeDamage");
        GlobalSoundManager.PlayOneShot(12, transform.position);

       // if (m_healthSystem.health > 0) return;

    }
}
