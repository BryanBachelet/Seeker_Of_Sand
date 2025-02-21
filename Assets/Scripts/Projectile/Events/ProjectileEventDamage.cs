using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEventDamage : MonoBehaviour
{
    public int damage;
    private bool m_hasDamagePlayer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
           if (!m_hasDamagePlayer)
            {
                    m_hasDamagePlayer = true;
                HealthPlayerComponent healthPlayer = other.GetComponent<HealthPlayerComponent>();

                AttackDamageInfo attackDamageInfo = new AttackDamageInfo();
                attackDamageInfo.attackName = "attackName";
                attackDamageInfo.position = transform.position;
                attackDamageInfo.damage = damage;
                healthPlayer.ApplyDamage(attackDamageInfo);
            }
        }
    }
}
