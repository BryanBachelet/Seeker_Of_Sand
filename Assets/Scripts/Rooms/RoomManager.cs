using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum RoomType
{
    Free = 0,
    Event = 1,
    Enemy = 2
}

public class RoomManager : MonoBehaviour
{
    public RoomType type;

    public int enemyToKillCount = 0;
    public int currentCountOfEnemy;

    [HideInInspector] public Teleporter[] teleporterArray = new Teleporter[3];
    private int m_currentTeleporterCount = 0;

    private Enemies.EnemyManager m_enemyManager;

    public void Start()
    {
        m_enemyManager = FindAnyObjectByType<Enemies.EnemyManager>();
        m_enemyManager.OnDeathSimpleEvent += CountEnemy;
        currentCountOfEnemy = 0;
    }

    // Setup room teleporter 
    public void AddTeleporter(Teleporter newInstance)
    {
        if (m_currentTeleporterCount >= 2)
        {
            GuerhoubaTools.LogSystem.LogMsg("They are alreay the max count of teleporter", true);
            return;
        }

        newInstance.DesactivationTeleportor();

        newInstance.enemyManager = m_enemyManager;
        teleporterArray[m_currentTeleporterCount] = newInstance;
        m_currentTeleporterCount++;

    }


    public void SetupRoomType(RoomType newType)
    {
        type = newType;
        switch (newType)
        {
            case RoomType.Free:
                ActiveTeleporter();
                DeactivateAltar();
                break;
            case RoomType.Event:

                break;
            case RoomType.Enemy:
                DeactivateAltar();
                enemyToKillCount = Random.Range(20, 100);
                break;
            default:
                break;
        }
    }

    public void ActiveTeleporter()
    {
       
        for (int i = 0; i < m_currentTeleporterCount; i++)
        {
            teleporterArray[i].ActivationTeleportor();
        }

    }


    private void DeactivateAltar() // Temp function 
    {
        transform.parent.GetComponentInChildren<AltarBehaviorComponent>().gameObject.SetActive(false);
    }

    private void CountEnemy()
    {
        currentCountOfEnemy ++;
        if(currentCountOfEnemy>enemyToKillCount)
        {
            ActiveTeleporter();
        }
    }

}
