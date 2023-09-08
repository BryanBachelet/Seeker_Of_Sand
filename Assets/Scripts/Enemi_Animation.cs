using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemi_Animation : MonoBehaviour
{
    public Animator m_animator;
    private Enemies.NpcHealthComponent m_npcHealth;
    private Transform m_target;

    public float debugdistance;
    // Start is called before the first frame update
    void Start()
    {
        m_npcHealth = this.GetComponent<Enemies.NpcHealthComponent>();
        m_target = m_npcHealth.targetData.target;
    }

    // Update is called once per frame
    void Update()
    {
        float distancePlayer = Vector3.Distance(transform.position, m_target.position);
        debugdistance = distancePlayer;
        if (distancePlayer < 100)
        {
            m_animator.SetBool("Close", true);
        }
        else
        {
            m_animator.SetBool("Close", false);
        }

        if (distancePlayer < 15)
        {
            m_animator.SetTrigger("ActiveAttack");

        }
        else
        {
            m_animator.ResetTrigger("ActiveAttack");
        }
    }
}
