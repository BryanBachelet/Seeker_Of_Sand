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

     public Teleporter[] teleporterArray = new Teleporter[3];
    private int m_currentTeleporterCount = 0;

    private Enemies.EnemyManager m_enemyManager;
    private bool isRoomHasBeenValidate = true;
    private Chosereward choserewardRef;

    static RewardDistribution playerRewardDistribution;

    public void RetriveComponent()
    {
        isRoomHasBeenValidate = false;
        m_enemyManager = FindAnyObjectByType<Enemies.EnemyManager>();
        if (playerRewardDistribution == null)
        {
            playerRewardDistribution = GameObject.Find("Player").GetComponent<RewardDistribution>();
        }
    }
    public void ActivateRoom()
    {
     
      if(roomType == RoomType.Enemy)  m_enemyManager.OnDeathSimpleEvent += CountEnemy;
        currentCountOfEnemy = 0;

        SetupRoomType();
    }

    // Setup room teleporter 
    public void AddTeleporter(Teleporter newInstance)
    {
        newInstance.DesactivationTeleportor();

        newInstance.enemyManager = m_enemyManager;
        if (m_currentTeleporterCount > 2)
        {
            Debug.Log("Too much teleporter");
            return;
        }

       
        teleporterArray[m_currentTeleporterCount] = newInstance;
        m_currentTeleporterCount++;

    }


    public void SetupRoomType()
    {
        switch (roomType)
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
            //teleporterArray[i].GetComponentInChildren<TeleporterFeebackController>().activeChange = true;
        }
    }

    private void GiveRoomReward()
    {
        playerRewardDistribution.GiveReward(rewardType);
        //switch (rewardType)
        //{
        //    case RewardType.UPGRADE:
        //        m_enemyManager.m_playerTranform.GetComponent<Character.CharacterUpgrade>().GiveUpgradePoint();
        //        m_enemyManager.m_playerTranform.GetComponent<Character.CharacterUpgrade>().ShowUpgradeWindow();
        //        break;
        //    case RewardType.SPELL:
        //        m_enemyManager.m_playerTranform.GetComponent<Character.CharacterUpgrade>().ShowSpellChoiceInteface();
        //        break;
        //    case RewardType.ARTEFACT:
        //        if(choserewardRef == null) 
        //            choserewardRef = FindAnyObjectByType<Chosereward>();
        //        choserewardRef.GiveArtefact();
        //        break;
        //    case RewardType.HEAL:
        //        m_enemyManager.m_playerTranform.GetComponent<HealthPlayerComponent>().RestoreHealQuarter(1);
        //        break;
        //    default:
        //        break;
        //}
    }


    #endregion

    public void ExcludeReward()
    {

    }

    public void IntegreReward()
    {

    }
    public void SetRandomReward()
    {
       
    }

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
