using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardInteraction : InteractionInterface
{
    [SerializeField] private RewardTypologie m_rewardTypologie;
    [SerializeField] private GameObject m_meshCristal;
    private Transform playerRef;

    public bool autoValidation = true;
    // Start is called before the first frame update
    void Start()
    {
        m_rewardTypologie = this.GetComponentInChildren<RewardTypologie>();
        playerRef = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnInteractionStart(GameObject player)
    {
        if (TerrainGenerator.staticRoomManager.isRoomHasBeenValidate || autoValidation)
        {
            if (m_rewardTypologie)
            {
                m_rewardTypologie.ActivationDistribution();
                m_rewardTypologie.autoValidation = autoValidation;
                m_rewardTypologie.GetComponent<ExperienceMouvement>().ActiveExperienceParticule(playerRef);
                //m_meshCristal.SetActive(false);
                
            }
        }


        //xpMvtScript.ActiveExperienceParticule(this.transform);
        //m_worldExp.Remove(xpMvtScript);
        //ActiveEvent();

    }

    public override void OnInteractionEnd(GameObject player)
    {

    }
}
