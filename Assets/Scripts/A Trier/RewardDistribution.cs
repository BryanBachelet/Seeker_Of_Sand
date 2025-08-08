using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.GameEnum;
using GuerhoubaGames.Resources;
using SeekerOfSand.Tools;
using Unity.VisualScripting;

public class RewardDistribution : MonoBehaviour
{
    public GameObject rewardHolderPrefan;
    public RewardType rewardType;

    [HideInInspector] public bool isRewardSend;
    private GameObject currentRewardHolder;

    public UnityEngine.VFX.VisualEffect vfxactiveLoot;

    public void RewardValidate()
    {
        isRewardSend = true;
    }

    public void Update()
    {
        //if (Input.GetKeyUp(KeyCode.U))
        //{

        //    GiveReward(RewardType.SPELL, transform.position + transform.forward * 50f, HealthReward.QUARTER, GameElement.FIRE);
        //}
    }

    public void GiveReward(RewardType rewardType, Transform positionReward,HealthReward healthReward, GameElement roomElementColor)
    {
        currentRewardHolder = Instantiate(rewardHolderPrefan, positionReward.position, this.transform.rotation);
        RewardTypologie rewardTypopologie = currentRewardHolder.GetComponentInChildren<RewardTypologie>();
        rewardTypopologie.rewardType = rewardType;
        rewardTypopologie.healthReward = healthReward;
        rewardTypopologie.rewardDistribution = this;
        rewardTypopologie.elementIndex = GeneralTools.GetElementalArrayIndex( roomElementColor,true);
        rewardTypopologie.element = roomElementColor;
        isRewardSend = false;
    }

    public void GiveReward(RewardType rewardType, Vector3 positionReward, HealthReward healthReward, GameElement roomElementColor)
    {
        currentRewardHolder = Instantiate(rewardHolderPrefan, positionReward, this.transform.rotation);
        RewardTypologie rewardTypopologie = currentRewardHolder.GetComponentInChildren<RewardTypologie>();
        rewardTypopologie.rewardType = rewardType;
        rewardTypopologie.healthReward = healthReward;
        rewardTypopologie.rewardDistribution = this;
        rewardTypopologie.elementIndex = GeneralTools.GetElementalArrayIndex(roomElementColor, true);
        rewardTypopologie.element = roomElementColor;
        isRewardSend = false;
    }


    public void ActiveLootFB()
    {
        vfxactiveLoot.Play();
    }



}
