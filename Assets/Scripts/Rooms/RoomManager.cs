using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum RoomType 
{
    Free = 0,
    Event = 1,
    Enemy = 2
}

public enum RewardType
{
    UPGRADE = 0,
    SPELL = 1,
    ARTEFACT = 2,
    HEAL = 3,
}

public class RoomManager : MonoBehaviour
{
    public RoomType roomType;
    public RewardType rewardType;

    public int enemyToKillCount = 0;
    public int currentCountOfEnemy;

    [HideInInspector] public Teleporter[] teleporterArray = new Teleporter[3];
    private int m_currentTeleporterCount = 0;

    private Enemies.EnemyManager m_enemyManager;
    private bool isRoomHasBeenValidate = true;



    public void ActivateRoom()
    {
        isRoomHasBeenValidate = false;
        m_enemyManager = FindAnyObjectByType<Enemies.EnemyManager>();
      if(roomType == RoomType.Enemy)  m_enemyManager.OnDeathSimpleEvent += CountEnemy;
        currentCountOfEnemy = 0;
    }

    // Setup room teleporter 
    public void AddTeleporter(Teleporter newInstance)
    {
        newInstance.DesactivationTeleportor();

        newInstance.enemyManager = m_enemyManager;
        if (m_currentTeleporterCount >= 2)
        {
            GuerhoubaTools.LogSystem.LogMsg("They are alreay the max count of teleporter", true);
            return;
        }

       
        teleporterArray[m_currentTeleporterCount] = newInstance;
        m_currentTeleporterCount++;

    }


    public void SetupRoomType(RoomType newType)
    {
        roomType = newType;
        switch (newType)
        {
            case RoomType.Free:
                ValidateRoom();
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

    public void ValidateRoom()
    {
        if (isRoomHasBeenValidate) return;
        ActivateTeleporters();
        GiveRoomReward();
        isRoomHasBeenValidate = true;
    }

    #region Room Validation Functions
    private void ActivateTeleporters()
    {
        for (int i = 0; i < m_currentTeleporterCount; i++)
        {
            teleporterArray[i].ActivationTeleportor();
        }
    }

    private void GiveRoomReward()
    {
        switch (rewardType)
        {
            case RewardType.UPGRADE:
                m_enemyManager.m_playerTranform.GetComponent<Character.CharacterUpgrade>().GiveUpgradePoint();
                m_enemyManager.m_playerTranform.GetComponent<Character.CharacterUpgrade>().ShowUpgradeWindow();
                break;
            case RewardType.SPELL:
                m_enemyManager.m_playerTranform.GetComponent<Character.CharacterUpgrade>().ShowSpellChoiceInteface();
                break;
            case RewardType.ARTEFACT:
                break;
            case RewardType.HEAL:
                break;
            default:
                break;
        }
    }


    #endregion


    private void DeactivateAltar() // Temp function 
    {
        AltarBehaviorComponent obj = transform.parent.GetComponentInChildren<AltarBehaviorComponent>();
        if(obj != null)
        {
            obj.gameObject.SetActive(false);
        }
    }

    private void CountEnemy()
    {
        currentCountOfEnemy ++;
        if(currentCountOfEnemy>enemyToKillCount && roomType == RoomType.Enemy)
        {
            ValidateRoom();
        }
    }

}
