using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    public BossRoom bossRoom;
    private bool isFirstTime = false;
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!isFirstTime)
            {
                bossRoom.SpawnBossInstance();
                isFirstTime = true;
            }
        }
    }
}
