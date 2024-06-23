using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.GameEnum;

public class MerchandRoom : MonoBehaviour
{
    public RoomManager roomManager;
    public MarchandBehavior marchandBehavior;
    void Awake()
    {
        roomManager.onActivateRoom += ActivateMarchandRoom;
        roomManager.onDeactivateRoom += DeactivateMarchandRoom;
    }


    public void ActivateMarchandRoom(RoomType roomtype,RewardType rewardType)
    {
        if (roomtype == RoomType.Merchant)
        {
            marchandBehavior.gameObject.SetActive(true);
            GlobalSoundManager.SwitchAmbiantToMarchand(true);
            marchandBehavior.InitComponents();
        }

    }

    public void DeactivateMarchandRoom(RoomType roomType,RewardType rewardType)
    {
        marchandBehavior.gameObject.SetActive(false);
        GlobalSoundManager.SwitchAmbiantToMarchand(false);
    }

}
