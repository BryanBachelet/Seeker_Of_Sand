using GuerhoubaGames.Character;
using GuerhoubaGames.GameEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuerhoubaGames
{
    public class RewardInteraction : InteractionInterface
    {

        public RewardType rewardType;
        public GameElement rewardElement;

       
        public override void OnInteractionStart(GameObject player)
        {
            //if (TerrainGenerator.s_currentRoomManager.isRoomHasBeenValidate || autoValidation)
            //{
            //    StartCoroutine(distributeWithDelay());
            //    if (m_rewardTypologie)
            //    {
            //        m_rewardTypologie.ActivationDistribution();
            //        m_rewardTypologie.autoValidation = autoValidation;
            //        m_rewardTypologie.GetComponent<ExperienceMouvement>().ActiveExperienceParticule(playerRef);
            //        //m_meshCristal.SetActive(false);

            //    }
            //}


            //xpMvtScript.ActiveExperienceParticule(this.transform);
            //m_worldExp.Remove(xpMvtScript);
            //ActiveEvent();

            CharacterManager characterManager = player.GetComponent<CharacterManager>();
            characterManager.ApplyReward(rewardType, rewardElement);
            Destroy(this.gameObject);
            
        }

       


        public override void OnInteractionEnd(GameObject player)
        {
            return;
        }
    }
}
