using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeCharacterElemental : InteractionInterface
{
    [SerializeField] private ObjectReward m_rewardTypologie;
    [SerializeField] private GameObject m_meshCristal;
    private Transform playerRef;
    private CristalInventory m_cristalInventory;

    // Start is called before the first frame update
    void Start()
    {
        //m_rewardTypologie = this.GetComponentInChildren<RewardTypologie>();
        playerRef = GameObject.Find("Player").transform;
        m_cristalInventory = playerRef.GetComponent<CristalInventory>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void OnInteractionStart(GameObject player)
    {
        if(m_cristalInventory.HasEnoughDissonanceCristal(cost, this.gameObject.name))
        {
            if (m_rewardTypologie)
            {
                m_rewardTypologie.GetComponent<ExperienceMouvement>().ActiveExperienceParticule(playerRef);

            }
        }


    }

    public override void OnInteractionEnd(GameObject player)
    {

    }
}
