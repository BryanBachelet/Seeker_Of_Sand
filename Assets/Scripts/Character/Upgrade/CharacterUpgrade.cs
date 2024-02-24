using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CharacterUpgrade : MonoBehaviour
{
    public List<Upgrade> m_avatarUpgrade;
    public static int upgradePoint = 0;
    [SerializeField] private int startPoint = 0;
    public GameObject upgradeUiGO;

    public GameObject uiLoaderDisplay;
    public TMPro.TMP_Text m_upgradePoint;
    [SerializeField] public Text m_LevelDisplay;

    // UI display object
    private GameObject m_upgradeUiGODisplay;
    private GameObject m_spellBookUIDisplay;
    public GameObject m_FixeElementUI;


    private Loader_Behavior m_loaderBehavior;
    private UpgradeManager m_upgradeManager;
    public UpgradeUI m_upgradeUi;
    private CharacterProfile m_characterProfil;
    private Character.CharacterShoot m_characterShoot;
    private Character.CharacterSpellBook m_characterInventory;
    private Experience_System experience;

    private Upgrade[] m_upgradeToChoose = new Upgrade[3];

    private bool had5level = false;
    private UpgradeUIDecal m_UpgradeUiDecal;

    public Animator bookAnimator;
    public GameObject upgradeDisplayGO;
    private bool m_isFirstTime = true;

    public GameObject upgradeDisplayVFX;

    private Experience_System m_experienceSystem;

    private bool isUpgradeWindowOpen;
    [SerializeField] private bool isDebugActive = false;

    public void UpgradeWindowInput(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            if (upgradePoint == 0 || had5level)
                return;

            if (!isUpgradeWindowOpen)
            {
                ShowUpgradeWindow();
                return;
            }
            if (isUpgradeWindowOpen)
            {
                UnShowUpgradeWindow();
                return;
            }
        }
    }


    #region Init Script
    public void Start()
    {
        upgradePoint = startPoint;
        InitComponents();
    }

    public void InitComponents()
    {
        m_upgradeManager = FindObjectOfType<UpgradeManager>();
        m_characterProfil = GetComponent<CharacterProfile>();
        m_characterShoot = GetComponent<Character.CharacterShoot>();
        m_characterInventory = GetComponent<Character.CharacterSpellBook>();
        experience = GetComponent<Experience_System>();
        m_upgradeUiGODisplay = UiSpellGrimoire.bookDisplayRoot.GetComponent<UpgradeUIDecal>().upgradePanelGameObject;
        m_UpgradeUiDecal = UiSpellGrimoire.bookDisplayRoot.GetComponent<UpgradeUIDecal>();
        m_upgradePoint.text = upgradePoint.ToString();
    }
    #endregion


    public void ShowUpgradeWindow()
    {
        isUpgradeWindowOpen = true;

        // -> Deactivate player in game interface
        m_FixeElementUI.SetActive(false);

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

    public void UnShowUpgradeWindow()
    {
        isUpgradeWindowOpen = false;
        m_FixeElementUI.SetActive(true);
        m_upgradeManager.CloseUpgradeUI();

        GameState.ChangeState();
    }

    public void GainLevel()
    {
        upgradePoint++;
        m_UpgradeUiDecal.upgradAvailable.text = "" + upgradePoint;
        m_upgradePoint.text = upgradePoint.ToString();

    }


    public void ApplyUpgrade(Upgrade upgradeChoose)
    {
        upgradePoint--;
        m_UpgradeUiDecal.upgradAvailable.text = "" + upgradePoint;
        m_upgradePoint.text = upgradePoint.ToString();
        experience.m_LevelTaken++;
        if (isDebugActive)
        {
            Debug.Log("Upgrade choose is " + upgradeChoose.gain.nameUpgrade);
            m_characterShoot.capsuleStatsAlone[upgradeChoose.capsuleIndex].DebugStat();
        }
        m_characterShoot.capsuleStatsAlone[upgradeChoose.capsuleIndex] = upgradeChoose.Apply(m_characterShoot.capsuleStatsAlone.ToArray()[upgradeChoose.capsuleIndex]);

        if (isDebugActive)
        {
            m_characterShoot.capsuleStatsAlone[upgradeChoose.capsuleIndex].DebugStat();
        }
        if (upgradePoint == 0 && isUpgradeWindowOpen)
            UnShowUpgradeWindow();
    }


}
