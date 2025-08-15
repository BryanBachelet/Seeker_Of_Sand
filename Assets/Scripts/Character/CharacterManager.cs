using GuerhoubaGames.Character;
using GuerhoubaGames.GameEnum;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Rendering.FilterWindow;


namespace GuerhoubaGames.Character
{
    public class CharacterManager : MonoBehaviour
    {

        // Character components
         private CharacterUpgrade m_characterUpgradeComponent;

        private Chosereward choseReward;

        public void Start()
        {
            m_characterUpgradeComponent = GetComponent<CharacterUpgrade>();
        }

        public void ApplyReward(RewardType rewardType, GameElement element)
        {
            switch (rewardType)
            {
                case RewardType.UPGRADE:
                   
                    m_characterUpgradeComponent.ApplyUpgradeReward(element);
                    GlobalSoundManager.PlayOneShot(6, Vector3.zero);
                    break;
                case RewardType.SPELL:
                    m_characterUpgradeComponent.ShowSpellChoiceInteface(element);
                    GlobalSoundManager.PlayOneShot(6, Vector3.zero);
                    break;
                case RewardType.ARTEFACT:
                    if (choseReward == null)
                    {
                        choseReward = GameObject.Find("Artefact-Choose-Trio").GetComponent<Chosereward>();
                    }
                    choseReward.GiveArtefactChoice(element);
                    GlobalSoundManager.PlayOneShot(56, Vector3.zero);
                    break;
               
                default:
                    break;
            }
        }
    }
}
