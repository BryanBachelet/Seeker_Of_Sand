using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CharacterUpgrade : MonoBehaviour
{
    public List<Upgrade> m_avatarUpgrade;
    public int upgradePoint = 0;
    public GameObject upgradeUiGO;

    public GameObject uiLoaderDisplay;
    public TMPro.TMP_Text m_upgradePoint;
    [SerializeField] public Text m_LevelDisplay;
    [SerializeField] private int m_CurrentLevel = 1;

       // UI display object
    private GameObject m_upgradeUiGODisplay;
    private GameObject m_spellBookUIDisplay;


    private Loader_Behavior m_loaderBehavior;
    private UpgradeManager m_upgradeManager;
    public UpgradeUI m_upgradeUi;
    private CharacterProfile m_characterProfil;
    private Character.CharacterShoot m_characterShoot;

    private Upgrade[] m_upgradeToChoose = new Upgrade[3];

    private bool had5level = false;
    private UpgradeUIDecal m_UpgradeUiDecal;

    public void UpgradeWindowInput(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            if (upgradePoint == 0 || had5level) return;
            upgradeUiGO.SetActive(!upgradeUiGO.activeSelf);
            UiSpellGrimoire.bookDisplayRoot.SetActive(!upgradeUiGO.activeSelf);

            m_upgradeUiGODisplay.SetActive(!m_upgradeUiGODisplay.activeSelf);
            m_spellBookUIDisplay.SetActive(!m_spellBookUIDisplay.activeSelf);
            GlobalSoundManager.PlayOneShot(6, Vector3.zero);
            if (upgradeUiGO.activeSelf == false)
            {
                GameState.ChangeState();
                DestroyAllUpgrade();
                return;
            }
            GetNewUpgrade();
            m_upgradeUi.UpdateUpgradeDisplay(m_upgradeToChoose);
            GameState.ChangeState();
            // Time.timeScale = 0.02f;
        }
    }

    public void UpgradeWindowLevel5()
    {
        if (upgradeUiGO.activeSelf == true) return;

        had5level = true;
        upgradeUiGO.SetActive(!upgradeUiGO.activeSelf);
        UiSpellGrimoire.bookDisplayRoot.SetActive(!upgradeUiGO.activeSelf);
        m_upgradeUiGODisplay.SetActive(!m_upgradeUiGODisplay.activeSelf);
        m_spellBookUIDisplay.SetActive(!m_spellBookUIDisplay.activeSelf);
        GlobalSoundManager.PlayOneShot(6, Vector3.zero);
        if (upgradeUiGO.activeSelf == false)
        {
            GameState.ChangeState();
            DestroyAllUpgrade();
            return;
        }
        GetNewUpgrade();
        m_upgradeUi.UpdateUpgradeDisplay(m_upgradeToChoose);
        GameState.ChangeState();
        // Time.timeScale = 0.02f;
    }
    #region Init Script
    public void Start()
    {
        //InitComponents();

    }

    public void InitComponents()
    {
        m_upgradeManager = FindObjectOfType<UpgradeManager>();
        //m_upgradeUi = upgradeUiGO.GetComponent<UpgradeUI>();
        m_characterProfil = GetComponent<CharacterProfile>();
        m_characterShoot = GetComponent<Character.CharacterShoot>();
        m_upgradeUiGODisplay = UiSpellGrimoire.bookDisplayRoot.GetComponent<UpgradeUIDecal>().upgradePanelGameObject;
        m_spellBookUIDisplay = UiSpellGrimoire.bookDisplayRoot.GetComponent<UpgradeUIDecal>().gameObject;
        m_UpgradeUiDecal = UiSpellGrimoire.bookDisplayRoot.GetComponent<UpgradeUIDecal>();

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

    public void GetNewUpgrade()
    {
        if (upgradePoint == 0) return;

        m_upgradeToChoose = m_upgradeManager.RandomUpgrade(3);
        for (int i = 0; i < 3; i++)
        {
            int index = Random.Range(0, m_characterShoot.maxSpellIndex);
            int spellIndex = m_characterShoot.spellEquip[index];
            m_upgradeToChoose[i].Setup(index, m_characterShoot.bookOfSpell[spellIndex].sprite);
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

        //Debug.Log("Index Choice = " + indexChoice.ToString() + " number of upgrade " + numberUpgrade.ToString());
        if (numberUpgrade > upgradePoint) return;
        for (int i = 0; i < numberUpgrade; i++)
        {
            m_avatarUpgrade.Add(m_upgradeToChoose[indexChoice]);
            ApplyUpgrade(indexChoice);
            upgradePoint--;
            m_UpgradeUiDecal.upgradAvailable.text = ""+ upgradePoint;
        }

        DestroyAllUpgrade();
        if (upgradePoint == 0 )
        {
            upgradeUiGO.SetActive(!upgradeUiGO.activeSelf);
            UiSpellGrimoire.bookDisplayRoot.SetActive(!upgradeUiGO.activeSelf);
            m_upgradeUiGODisplay.SetActive(!m_upgradeUiGODisplay.activeSelf);
            m_spellBookUIDisplay.SetActive(!m_spellBookUIDisplay.activeSelf);
            m_upgradePoint.text = upgradePoint.ToString();
            GlobalSoundManager.PlayOneShot(30, transform.position);
            GameState.ChangeState();
            if(had5level) { had5level = false; }
            return;
        }

        GetNewUpgrade();
        m_upgradeUi.UpdateUpgradeDisplay(m_upgradeToChoose);
        m_upgradePoint.text = upgradePoint.ToString();
    }


    public void GainLevel()
    {
        upgradePoint++;
        m_UpgradeUiDecal.upgradAvailable.text = "" + upgradePoint;
        m_upgradePoint.text = upgradePoint.ToString();
        if (upgradePoint >= 5) 
        { 
            UpgradeWindowLevel5(); 
        }
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
                m_avatarUpgrade[index].Apply(ref m_characterShoot.capsuleStatsAlone[m_avatarUpgrade[index].capsuleIndex]);
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
                m_upgradeToChoose[indexChoose].Apply(ref m_characterShoot.capsuleStatsAlone[m_upgradeToChoose[indexChoose].capsuleIndex]);
                break;
        }
    }
}
