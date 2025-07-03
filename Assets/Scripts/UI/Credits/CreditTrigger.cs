using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.AI;
using Enemies;
using UnityEngine.SceneManagement;


public class CreditTrigger : MonoBehaviour
{

    private NpcHealthComponent m_healthComponent;    

    // Start is called before the first frame update
    void Start()
    {
        m_healthComponent= GetComponent<NpcHealthComponent>();
        m_healthComponent.OnDeathEvent += OnDeathCredit;
    }


    void OnDeathCredit()
    {
        Camera.main.GetComponent<CameraFadeFunction>().LaunchFadeIn(true, 1);
        StartCoroutine(LaunchScene());
    }

    IEnumerator LaunchScene()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(6);
        yield return null;
    }
}
