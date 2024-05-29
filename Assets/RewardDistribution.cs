using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardDistribution : MonoBehaviour
{
    public GameObject rewardHolderPrefan;
    public RewardType rewardType;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GiveReward(RewardType rewardType)
    {
        GameObject NewReward = Instantiate(rewardHolderPrefan, this.transform.position + new Vector3(0,100,0), this.transform.rotation);
        ExperienceMouvement newXp  = NewReward.GetComponent<ExperienceMouvement>();
        newXp.m_playerPosition = this.transform;
        newXp.ActiveExperienceParticule(this.transform);
        RewardTypologie rewardTypopologie = NewReward.GetComponent<RewardTypologie>();
        rewardTypopologie.rewardType = rewardType;

    }
}
