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
        m_upgradeUi = upgradeUiGO.GetComponent<UpgradeUI>();
        m_characterProfil = GetComponent<CharacterProfile>();
        m_characterShoot = GetComponent<Character.CharacterShoot>();
        m_characterInventory = GetComponent<Character.CharacterSpellBook>();
        m_upgradeUiGODisplay = UiSpellGrimoire.bookDisplayRoot.GetComponent<UpgradeUIDecal>().upgradePanelGameObject;
        m_spellBookUIDisplay = UiSpellGrimoire.bookDisplayRoot.GetComponent<UpgradeUIDecal>().gameObject;
        m_UpgradeUiDecal = UiSpellGrimoire.bookDisplayRoot.GetComponent<UpgradeUIDecal>();
        m_experienceSystem = this.GetComponent<Experience_System>();

        m_upgradeUi.m_upgradeButtonFunction += ChooseUpgrade;
        for (int i = 0; i < m_upgradeUi.upgradeButtons.Length; i++)
        {
            int upgradeLink = m_upgradeUi.upgradeButtons[i].upgradeLink;
            int upgradeNumber = m_upgradeUi.upgradeButtons[i].numberOfUpgrade;
            m_upgradeUi.upgradeButtons[i].button.onClick.AddListener(() => m_upgradeUi.m_upgradeButtonFunction.Invoke(upgradeLink, upgradeNumber));
        }
        m_upgradePoint.text = upgradePoint.ToString();
        //m_loaderBehavior = uiLoaderDisplay.GetComponent<Loader_Behavior>();
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

    public void ReplaceNewUpgrade(int indexUpgrade)
    {
        int index = Random.Range(0, m_characterShoot.maxSpellIndex);
        int spellIndex = m_characterShoot.spellEquip[index];
        m_upgradeToChoose[indexUpgrade] = m_upgradeManager.GetRandomUpgradeToSpell(m_characterShoot.m_capsuleManager.GetCapsuleIndex(m_characterInventory.GetSpecificSpell(spellIndex)));
        m_upgradeToChoose[indexUpgrade].Setup(index, m_characterInventory.GetSpecificSpell(spellIndex).sprite);
        Instantiate(upgradeDisplayVFX, transform.position, transform.rotation);
    }

    public void GetNewUpgrades()
    {
        if (m_isFirstTime) m_isFirstTime = false;
        for (int i = 0; i < 3; i++)
        {
            int index = Random.Range(0, m_characterShoot.maxSpellIndex);
            int spellIndex = m_characterShoot.spellEquip[index];
            m_upgradeToChoose[i] = m_upgradeManager.GetRandomUpgradeToSpell(m_characterShoot.m_capsuleManager.GetCapsuleIndex(m_characterInventory.GetSpecificSpell(spellIndex)));
            m_upgradeToChoose[i].Setup(index, m_characterInventory.GetSpecificSpell(spellIndex).sprite);
        }
    }



    public void DestroyAllUpgrade()
    {
        for (int i = 0; i < m_upgradeToChoose.Length; i++)
        {
            m_upgradeToChoose[i].Destroy();
            m_upgradeToChoose[i] = null;
        }

    }
    public void ChooseUpgrade(int indexChoice, int numberUpgrade)
    {

        //   Debug.Log("Index Choice = " + indexChoice.ToString() + " number of upgrade " + numberUpgrade.ToString());
        if (numberUpgrade > upgradePoint) return;
        for (int i = 0; i < numberUpgrade; i++)
        {
            m_avatarUpgrade.Add(m_upgradeToChoose[indexChoice]);
            ApplyUpgrade(indexChoice);
            upgradePoint--;
            m_experienceSystem.m_LevelTaken++;
            m_UpgradeUiDecal.upgradAvailable.text = "" + upgradePoint;
        }

        ReplaceNewUpgrade(indexChoice);
        if (upgradePoint == 0)
        {
            StartCoroutine(CloseBookWithDelay(2));
            return;
        }


        m_upgradeUi.UpdateUpgradeDisplay(m_upgradeToChoose);
        m_upgradePoint.text = upgradePoint.ToString();
    }


    public void GainLevel()
    {
        upgradePoint++;
        m_UpgradeUiDecal.upgradAvailable.text = "" + upgradePoint;
        m_upgradePoint.text = upgradePoint.ToString();

    }

    private CharacterStat CalculateStat(CharacterStat stats)
    {
        CharacterStat newStats = stats;

        for (int i = 0; i < m_avatarUpgrade.Count; i++)
        {
            ApplyUpgrade(i, ref newStats);
        }

        return newStats;
    }

    private void ApplyUpgrade(int index, ref CharacterStat stat)
    {
        switch (m_avatarUpgrade[index].gain.type)
        {
            case UpgradeType.CHARACTER:
                m_avatarUpgrade[index].Apply(ref stat);
                break;
            case UpgradeType.LAUNCHER:
                m_avatarUpgrade[index].Apply(ref m_characterShoot.launcherStats);
                break;
            case UpgradeType.CAPSULE:
                m_characterShoot.capsuleStatsAlone[m_avatarUpgrade[index].capsuleIndex] = m_avatarUpgrade[index].Apply(m_characterShoot.capsuleStatsAlone.ToArray()[m_avatarUpgrade[index].capsuleIndex]);
                break;
            default:
                break;
        }
    }

    private void ApplyUpgrade(int indexChoose)
    {
        switch (m_upgradeToChoose[indexChoose].gain.type)
        {
            case UpgradeType.CHARACTER:
                m_upgradeToChoose[indexChoose].Apply(ref m_characterProfil.stats);
                break;
            case UpgradeType.LAUNCHER:
                m_upgradeToChoose[indexChoose].Apply(ref m_characterShoot.launcherStats);
                break;
            case UpgradeType.CAPSULE:
                m_characterShoot.capsuleStatsAlone[m_upgradeToChoose[indexChoose].capsuleIndex] = m_upgradeToChoose[indexChoose].Apply(m_characterShoot.capsuleStatsAlone.ToArray()[m_upgradeToChoose[indexChoose].capsuleIndex]);
                break;
        }
    }

    public void ApplyUpgrade(Upgrade upgradeChoose)
    {
        upgradePoint--;
        m_UpgradeUiDecal.upgradAvailable.text = "" + upgradePoint;
        m_upgradePoint.text = upgradePoint.ToString();

        if (isDebugActive) Debug.Log("Upgrade choose is " + upgradeChoose.gain.nameUpgrade);
        m_characterShoot.capsuleStatsAlone[upgradeChoose.capsuleIndex] = upgradeChoose.Apply(m_characterShoot.capsuleStatsAlone.ToArray()[upgradeChoose.capsuleIndex]);
        if (upgradePoint == 0 && isUpgradeWindowOpen)
            UnShowUpgradeWindow();
    }

    public IEnumerator CloseBookWithDelay(float time)
    {
        bookAnimator.SetBool("BookOpen", false);
        StartCoroutine(DisplayUpgradeWithDelay(false));
        yield return new WaitForSeconds(time);
        upgradeUiGO.SetActive(!upgradeUiGO.activeSelf);
        m_FixeElementUI.SetActive(true);

        Debug.Log("Book close !!!!!!");
        UiSpellGrimoire.bookDisplayRoot.SetActive(!upgradeUiGO.activeSelf);
        m_upgradeUiGODisplay.SetActive(!m_upgradeUiGODisplay.activeSelf);
        m_spellBookUIDisplay.SetActive(!m_spellBookUIDisplay.activeSelf);
        m_upgradePoint.text = upgradePoint.ToString();
        GlobalSoundManager.PlayOneShot(30, transform.position);
        GameState.ChangeState();
        if (had5level) { had5level = false; }
    }

    public IEnumerator DisplayUpgradeWithDelay(bool newState)
    {
        if (newState)
        {
            yield return new WaitForSeconds(3.5f);
            upgradeDisplayGO.SetActive(newState);
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
            upgradeDisplayGO.SetActive(newState);
        }

    }

}
