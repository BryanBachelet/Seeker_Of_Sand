using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.GameEnum;
using GuerhoubaGames.Resources;

public class RewardDistribution : MonoBehaviour
{
    public GameObject rewardHolderPrefan;
    public RewardType rewardType;

    [HideInInspector] public bool isRewardSend;
    private GameObject currentRewardHolder;

    public void RewardValidate()
    {
        isRewardSend = true;
    }

    public void GiveReward(RewardType rewardType, Transform positionReward,HealthReward healthReward, GameElement roomElementColor)
    {
        currentRewardHolder = Instantiate(rewardHolderPrefan, positionReward.position, this.transform.rotation);
        RewardTypologie rewardTypopologie = currentRewardHolder.GetComponentInChildren<RewardTypologie>();
        rewardTypopologie.rewardType = rewardType;
        rewardTypopologie.healthReward = healthReward;
        rewardTypopologie.rewardDistribution = this;
        rewardTypopologie.element = (int)roomElementColor;
        isRewardSend = false;
    }



}
