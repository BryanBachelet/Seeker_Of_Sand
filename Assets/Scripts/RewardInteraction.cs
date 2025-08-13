using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardInteraction : InteractionInterface
{
    [SerializeField] private RewardTypologie m_rewardTypologie;
    [SerializeField] private GameObject m_meshCristal;
    private Transform playerRef;
    public Animator animatorBall;
    public bool autoValidation = true;

    public ExperienceMouvement xpMove;
    // Start is called before the first frame update
    void Start()
    {
        m_rewardTypologie = this.GetComponentInChildren<RewardTypologie>();
        playerRef = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position, playerRef.transform.position) < 50)
        {
            animatorBall.SetBool("PlayerNear", true);
        }
        else
        {
            animatorBall.SetBool("PlayerNear", false);
        }
    }

    public override void OnInteractionStart(GameObject player)
    {
        if (TerrainGenerator.s_currentRoomManager.isRoomHasBeenValidate || autoValidation)
        {
            StartCoroutine(distributeWithDelay());
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

    public IEnumerator distributeWithDelay()
    {
        animatorBall.SetTrigger("Looted");
        yield return new WaitForSeconds(1);
        xpMove.ActiveExperienceParticule(playerRef.transform);
    }
    public override void OnInteractionEnd(GameObject player)
    {

    }
}
