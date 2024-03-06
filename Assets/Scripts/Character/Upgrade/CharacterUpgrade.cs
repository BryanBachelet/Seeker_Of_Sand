using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using GuerhoubaTools;

namespace Character
{
    [System.Serializable]
    public struct SpellUpgradeConfig
    {
        public int levelToGainSpell;
        public bool hasBeenTaken;
    }

    public class CharacterUpgrade : MonoBehaviour
    {
        [Header("Upgrades Base Infos")]
        public List<Upgrade> avatarUpgradeList;
        public static int upgradePoint = 0;
        [SerializeField] private int presetUpgradePoint = 0;

        [Header("Spell Upgrades Parameters")]
        public SpellUpgradeConfig[] spellUpgradeConfigs;
        private int baseSpellIndex;

        // UI display object
        [Header("UI Objects")]
        public TMPro.TMP_Text upgradePointTextDisplay;
        public GameObject baseGameInterfaceUI;
        //   private UpgradeUIDecal m_UpgradeUiDecal;

        // Useful Components
        private UpgradeManager m_upgradeManager;
        private CapsuleManager m_capsuleManager;
        private Character.CharacterShoot m_characterShoot;
        private Character.CharacterSpellBook m_characterInventory;
        private Experience_System experience;

        private bool isUpgradeWindowOpen;
        private bool isSpellUpgradeWindowOpen;
        [SerializeField] private bool isDebugActive = false;

        private ObjectState state;

        #region Input Functions
        public void UpgradeWindowInput(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
            {
                if (upgradePoint == 0 || isSpellUpgradeWindowOpen)
                    return;

                if (!isUpgradeWindowOpen)
                {
                   ShowUpgradeWindow();
                    return;
                }
                if (isUpgradeWindowOpen)
                {  
                    CloseUpgradeWindow();
                    return;
                }
            }
        }
        #endregion

        #region Init Script
        public void Start()
        {
            state = new ObjectState();
            GameState.AddObject(state);
            upgradePoint = presetUpgradePoint;
            InitComponents();
        }

        public void InitComponents()
        {
            m_upgradeManager = FindObjectOfType<UpgradeManager>();
            m_capsuleManager = m_upgradeManager.GetComponent<CapsuleManager>();
            m_characterShoot = GetComponent<Character.CharacterShoot>();
            m_characterInventory = GetComponent<Character.CharacterSpellBook>();
            experience = GetComponent<Experience_System>();
            // m_UpgradeUiDecal = UiSpellGrimoire.bookDisplayRoot.GetComponent<UpgradeUIDecal>();
            upgradePointTextDisplay.text = upgradePoint.ToString();
        }
        #endregion

        public void ShowSpellChoiceInteface()
        {
            m_upgradeManager.OpenSpellChoiceUI();
            GameState.ChangeState();
            isSpellUpgradeWindowOpen = true;
          //  Debug.Break();
        }

        public void ApplySpellChoise(SpellSystem.Capsule capsule)
        {
            m_characterShoot.AddSpell(m_capsuleManager.GetCapsuleIndex(capsule));
            CloseSpellChoiceInterface();
        }

        public void CloseSpellChoiceInterface()
        {
            m_upgradeManager.CloseSpellChoiceUI();
            GameState.ChangeState();
            isSpellUpgradeWindowOpen = false;
        }

        #region Upgrade Functions
        public void ShowUpgradeWindow()
        {
            isUpgradeWindowOpen = true;

            // -> Deactivate player in game interface
            baseGameInterfaceUI.SetActive(false);

            UpgradeLevelingData data = new UpgradeLevelingData();
            data.spellState = m_characterShoot.capsuleStatsAlone.ToArray();
            data.spellCount = m_characterShoot.maxSpellIndex;
            data.iconSpell = m_characterShoot.GetSpellSprite();
            data.capsuleIndex = m_characterShoot.capsuleIndex.ToArray();
            data.upgradePoint = upgradePoint;
            m_upgradeManager.OpenUpgradeUI(data);


            GlobalSoundManager.PlayOneShot(6, Vector3.zero); // Play Sound
            GameState.ChangeState(); // Set Game in pause

        }

        public void CloseUpgradeWindow()
        {
            isUpgradeWindowOpen = false;
            baseGameInterfaceUI.SetActive(true);
            m_upgradeManager.CloseUpgradeUI();

            GameState.ChangeState();
        }

        public void ApplyUpgrade(Upgrade upgradeChoose)
        {
            upgradePoint--;
           // m_UpgradeUiDecal.upgradAvailable.text = "" + upgradePoint;
            upgradePointTextDisplay.text = upgradePoint.ToString();
            experience.m_LevelTaken++;
            LogSystem.LogMsg("Upgrade choose is " + upgradeChoose.gain.nameUpgrade,isDebugActive);
            if (isDebugActive)
            {
               
                m_characterShoot.capsuleStatsAlone[upgradeChoose.capsuleIndex].DebugStat();
            }
            m_characterShoot.capsuleStatsAlone[upgradeChoose.capsuleIndex] = upgradeChoose.Apply(m_characterShoot.capsuleStatsAlone.ToArray()[upgradeChoose.capsuleIndex]);
            avatarUpgradeList.Add(upgradeChoose);
            if (isDebugActive)
            {
                m_characterShoot.capsuleStatsAlone[upgradeChoose.capsuleIndex].DebugStat();
            }
            if (upgradePoint == 0 && isUpgradeWindowOpen)
                CloseUpgradeWindow();
        }

        #endregion

        public void GainLevel()
        {
            upgradePoint++;
            upgradePointTextDisplay.text = upgradePoint.ToString();
            if(baseSpellIndex < spellUpgradeConfigs.Length && spellUpgradeConfigs[baseSpellIndex].levelToGainSpell <= experience.GetCurrentLevel())
            {
                ShowSpellChoiceInteface();
                baseSpellIndex++;
               
            }

        }


      


    }

}