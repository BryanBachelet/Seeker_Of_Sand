using GuerhoubaGames.Character;
using GuerhoubaGames.GameEnum;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuerhoubaGames
{
    public class RewardInteraction : InteractionInterface
    {

        public RewardType rewardType;
        public GameElement rewardElement;
        public Action OnRewardTaken;

       
        public override void OnInteractionStart(GameObject player)
        {

            CharacterManager characterManager = player.GetComponent<CharacterManager>();
            characterManager.ApplyReward(rewardType, rewardElement);
            OnRewardTaken?.Invoke();
            Destroy(this.gameObject);
            
        }

       


        public override void OnInteractionEnd(GameObject player)
        {
            return;
        }
    }
}
