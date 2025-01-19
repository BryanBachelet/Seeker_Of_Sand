using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using GuerhoubaTools;
using GuerhoubaGames.GameEnum;
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
        private SpellManager m_spellManager;
        [HideInInspector] public Character.CharacterShoot m_characterShoot;
        [HideInInspector] public Character.CharacterSpellBook m_characterInventory;
        private Experience_System experience;

        private bool isUpgradeWindowOpen;
        private bool isSpellUpgradeWindowOpen;
        [SerializeField] private bool isDebugActive = false;

        private ObjectState state;
        private SeekerOfSand.UI.UI_PlayerInfos m_UiPlayerInfo;

        public GameElement lastRoomElement;

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

        public void ChooseUpgradeInput(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
            {
                if (isUpgradeWindowOpen)
                {
                    upgradeManager.ValidateUpgrade(int.Parse(ctx.control.name)-1);
                }

                if(isSpellUpgradeWindowOpen)
                {
                    upgradeManager.ValidateSpell(int.Parse(ctx.control.name) -1);
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
            m_spellManager = upgradeManager.GetComponent<SpellManager>();
            m_characterShoot = GetComponent<Character.CharacterShoot>();
            m_characterInventory = GetComponent<Character.CharacterSpellBook>();
            experience = GetComponent<Experience_System>();
            if (!m_UiPlayerInfo) m_UiPlayerInfo = GameObject.Find("UI_Manager").GetComponent<SeekerOfSand.UI.UI_PlayerInfos>();
            // m_UpgradeUiDecal = UiSpellGrimoire.bookDisplayRoot.GetComponent<UpgradeUIDecal>();
            //upgradePointTextDisplay.text = upgradePoint.ToString();
        }
        #endregion

        public void ShowSpellChoiceInteface()
        {
            if (upgradeManager)
            {
                upgradeManager.OpenSpellChoiceUI(lastRoomElement);
               

                GameState.ChangeState();
                isSpellUpgradeWindowOpen = true;
                ChangeBaseInterfaceDisplay(false);
            }

        }

        public void ApplySpellChoise(SpellSystem.SpellProfil capsule)
        {
            m_characterShoot.AddSpell(m_spellManager.GetCapsuleIndex(capsule));
            CloseSpellChoiceInterface();
        }

        public void CloseSpellChoiceInterface()
        {
            GlobalSoundManager.PlayOneShot(30, Vector3.zero);
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
            data.capsuleIndex = m_characterShoot.spellIndexGeneral.ToArray();
            data.upgradePoint = upgradePoint;
            data.roomElement = lastRoomElement;
            upgradeManager.OpenUpgradeUI(data);


            //GlobalSoundManager.PlayOneShot(6, Vector3.zero); // Play Sound
            GameState.ChangeState(); // Set Game in pause

        }

        public void CloseUpgradeWindow()
        {
            isUpgradeWindowOpen = false;
            ChangeBaseInterfaceDisplay(true);
            upgradeManager.CloseUpgradeUI();
            m_characterShoot.UpdateSpellRarityCadre(m_characterShoot.m_characterSpellBook.GetAllSpells());
            GameState.ChangeState();
        }

        public void ApplyUpgrade(UpgradeObject upgradeChoose)
        {
            upgradePoint--;


            SpellSystem.SpellProfil spellProfil = m_characterInventory.GetSpellOfRotation(upgradeChoose.indexSpellLink);
            LogSystem.LogMsg("Upgrade choose is " + upgradeChoose.name + " for the spell" + spellProfil.name, isDebugActive);
            if (isDebugActive)
            {

                spellProfil.DebugStat();
            }

            upgradeChoose.Apply(spellProfil);
            m_characterShoot.UpdatePullObject(spellProfil);
            avatarUpgradeList.Add(upgradeChoose);
            if (isDebugActive)
            {
                spellProfil.DebugStat();
            }
            if (m_UiPlayerInfo)
            {
               bool isLevelUp = spellProfil.AddSpellExpPoint(1);
               

                if (isLevelUp )
                {
                    spellProfil.GainLevel();
                    m_UiPlayerInfo.UpdateLevelSpell(upgradeChoose.indexSpellLink, spellProfil);
                }
                else
                {
                    if (upgradePoint == 0 && isUpgradeWindowOpen)
                    {

                        CloseUpgradeWindow();

                    }
                }
            }



        }

        public void OpenTierUpObject()
        {

        }

        public void CloseTierUpObject()
        {
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
            if (baseSpellIndex < spellUpgradeConfigs.Length && spellUpgradeConfigs[baseSpellIndex].levelToGainSpell <= experience.GetCurrentLevel())
            {
                ShowSpellChoiceInteface();
                ChangeBaseInterfaceDisplay(false);
                baseSpellIndex++;

            }

        }

        public void GiveUpgradePoint(int upgradeNumber)
        {
            upgradePoint += upgradeNumber;
            //upgradePointTextDisplay.text = upgradePoint.ToString();
        }

        public void ChangeBaseInterfaceDisplay(bool willBeAble)
        {
            baseGameInterfaceUI.SetActive(willBeAble);
        }

    }

}