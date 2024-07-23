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
        public List<UpgradeObject> avatarUpgradeList;
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
       [HideInInspector] public UpgradeManager upgradeManager;
        private CapsuleManager m_capsuleManager;
        [HideInInspector] public Character.CharacterShoot m_characterShoot;
        private Character.CharacterSpellBook m_characterInventory;
        private Experience_System experience;

        private bool isUpgradeWindowOpen;
        private bool isSpellUpgradeWindowOpen;
        [SerializeField] private bool isDebugActive = false;

        private ObjectState state;
        private SeekerOfSand.UI.UI_PlayerInfos m_UiPlayerInfo;

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
            upgradeManager = FindObjectOfType<UpgradeManager>();
            m_capsuleManager = upgradeManager.GetComponent<CapsuleManager>();
            m_characterShoot = GetComponent<Character.CharacterShoot>();
            m_characterInventory = GetComponent<Character.CharacterSpellBook>();
            experience = GetComponent<Experience_System>();
            if (!m_UiPlayerInfo) m_UiPlayerInfo = GameObject.Find("UI_Manager").GetComponent<SeekerOfSand.UI.UI_PlayerInfos>();
            // m_UpgradeUiDecal = UiSpellGrimoire.bookDisplayRoot.GetComponent<UpgradeUIDecal>();
            upgradePointTextDisplay.text = upgradePoint.ToString();
        }
        #endregion

        public void ShowSpellChoiceInteface()
        {
            if(upgradeManager)
            {
                upgradeManager.OpenSpellChoiceUI();

                GameState.ChangeState();
                isSpellUpgradeWindowOpen = true;
                ChangeBaseInterfaceDisplay(false);
            }
           
        }

        public void ApplySpellChoise(SpellSystem.SpellProfil capsule)
        {
            m_characterShoot.AddSpell(m_capsuleManager.GetCapsuleIndex(capsule));
            CloseSpellChoiceInterface();
        }

        public void CloseSpellChoiceInterface()
        {
            upgradeManager.CloseSpellChoiceUI();
            GameState.ChangeState();
            isSpellUpgradeWindowOpen = false;
            ChangeBaseInterfaceDisplay(true);
        }

        #region Upgrade Functions
        public void ShowUpgradeWindow()
        {
            if (!m_characterShoot) return;
            isUpgradeWindowOpen = true;

            // -> Deactivate player in game interface
            ChangeBaseInterfaceDisplay(false);

            UpgradeLevelingData data = new UpgradeLevelingData();
            data.spellState = m_characterShoot.spellProfils.ToArray();
            data.spellCount = m_characterShoot.maxSpellIndex;
            data.iconSpell = m_characterShoot.GetSpellSprite();
            data.capsuleIndex = m_characterShoot.spellIndex.ToArray();
            data.upgradePoint = upgradePoint;
            upgradeManager.OpenUpgradeUI(data);


            GlobalSoundManager.PlayOneShot(6, Vector3.zero); // Play Sound
            GameState.ChangeState(); // Set Game in pause

        }

        public void CloseUpgradeWindow()
        {
            isUpgradeWindowOpen = false;
            ChangeBaseInterfaceDisplay(true);
            upgradeManager.CloseUpgradeUI();

            GameState.ChangeState();
        }

        public void ApplyUpgrade(UpgradeObject upgradeChoose)
        {
            upgradePoint--;
           // m_UpgradeUiDecal.upgradAvailable.text = "" + upgradePoint;
            upgradePointTextDisplay.text = upgradePoint.ToString();
            experience.m_LevelTaken++;
            experience.m_UiPlayerInfo.AddLevelTaken();
            LogSystem.LogMsg("Upgrade choose is " + upgradeChoose.name,isDebugActive);
            if (isDebugActive)
            {
               
                m_characterShoot.spellProfils[upgradeChoose.indexSpellLink].DebugStat();
            }

            //TODO : Change Apply from upgrade
            upgradeChoose.Apply(m_characterShoot.spellProfils.ToArray()[upgradeChoose.indexSpellLink]);
            if (m_UiPlayerInfo)
            {
                m_characterShoot.spellProfils[upgradeChoose.indexSpellLink].level++;
                
                m_UiPlayerInfo.UpdateLevelSpell(upgradeChoose.indexSpellLink, m_characterShoot.spellProfils[upgradeChoose.indexSpellLink].level);
            }
            avatarUpgradeList.Add(upgradeChoose);
            if (isDebugActive)
            {
                m_characterShoot.spellProfils[upgradeChoose.indexSpellLink].DebugStat();
            }
            if (upgradePoint == 0 && isUpgradeWindowOpen)
            {

                CloseUpgradeWindow();
            }

        }

  

        #endregion

        public void GainLevel()
        {
            return;
            upgradePoint++;
            upgradePointTextDisplay.text = upgradePoint.ToString();
            if(baseSpellIndex < spellUpgradeConfigs.Length && spellUpgradeConfigs[baseSpellIndex].levelToGainSpell <= experience.GetCurrentLevel())
            {
                ShowSpellChoiceInteface();
                ChangeBaseInterfaceDisplay(false);
                baseSpellIndex++;
               
            }

        }

        public void GiveUpgradePoint(int upgradeNumber)
        {
            upgradePoint+= upgradeNumber;
            upgradePointTextDisplay.text = upgradePoint.ToString();
        }
      
        public void ChangeBaseInterfaceDisplay(bool willBeAble)
        {
            baseGameInterfaceUI.SetActive(willBeAble);
        }

    }

}