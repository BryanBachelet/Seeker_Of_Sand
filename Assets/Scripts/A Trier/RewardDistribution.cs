using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.GameEnum;

public class RewardDistribution : MonoBehaviour
{

    public GameObject rewardHolderPrefan;
    public RewardType rewardType;

    private RewardTypologie m_rewardTypologie;
    [HideInInspector] public bool isRewardSend;

    public void RewardValidate()
    {
        isRewardSend = true;
    }

    public void GiveReward(RewardType rewardType, Transform positionReward,HealthReward healthReward)
    {
        GameObject NewReward = Instantiate(rewardHolderPrefan, positionReward.position, this.transform.rotation);
        ExperienceMouvement newXp  = NewReward.GetComponentInChildren<ExperienceMouvement>();
        //newXp.m_playerPosition = this.transform;
        //newXp.ActiveExperienceParticule(this.transform);
        RewardTypologie rewardTypopologie = NewReward.GetComponentInChildren<RewardTypologie>();
        rewardTypopologie.rewardType = rewardType;
        rewardTypopologie.healthReward = healthReward;
        rewardTypopologie.rewardDistribution = this;
        isRewardSend = false;
    }
}
