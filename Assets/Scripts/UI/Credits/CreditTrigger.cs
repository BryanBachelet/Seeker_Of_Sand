using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.AI;
using Enemies;
using UnityEngine.SceneManagement;


public class CreditTrigger : MonoBehaviour
{
    private   NpcHealthComponent m_healthComponent;    
    // Start is called before the first frame update
    void Start()
    {
        m_healthComponent= GetComponent<NpcHealthComponent>();
        m_healthComponent.OnDeathEvent += OnDeathCredit;
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    void OnDeathCredit()
    {
        SceneManager.LoadScene(6);
    }
}
