using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CharacterUpgrade : MonoBehaviour
{
    public List<Upgrade> m_avatarUpgrade;
    public int upgradePoint = 1;
    public GameObject upgradeUiGO;
    public GameObject uiLoaderDisplay;
    public Text m_upgradePoint;

    private Loader_Behavior m_loaderBehavior;
    private UpgradeManager m_upgradeManager;
    private UpgradeUI m_upgradeUi;
    private CharacterProfile m_characterProfil;
    private Character.CharacterShoot m_characterShoot;

    private Upgrade[] m_upgradeToChoose = new Upgrade[3];


    public void UpgradeWindowInput(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            if (upgradePoint == 0) return;
            upgradeUiGO.SetActive(!upgradeUiGO.activeSelf);
            GlobalSoundManager.PlayOneShot(6, Vector3.zero);
            //uiLoaderDisplay.SetActive(false);
            //m_loaderBehavior.UpdateLoaderInUpgrade(false);
            if (upgradeUiGO.activeSelf == false) 
            {
                DestroyAllUpgrade();
                return;
            }
            GetNewUpgrade();
            m_upgradeUi.UpdateUpgradeDisplay(m_upgradeToChoose);
            Time.timeScale = 0.02f;
        }
    }
    #region Init Script
    public void Start()
    {
        InitComponents();
        m_upgradePoint.text = upgradePoint.ToString();
    }

    private void InitComponents()
    {
        m_upgradeManager = FindObjectOfType<UpgradeManager>();
        m_upgradeUi = upgradeUiGO.GetComponent<UpgradeUI>();
        m_characterProfil = GetComponent<CharacterProfile>();
        m_characterShoot = GetComponent<Character.CharacterShoot>();
        //m_loaderBehavior = uiLoaderDisplay.GetComponent<Loader_Behavior>();
    }
    #endregion

    public void GetNewUpgrade()
    {
        if (upgradePoint == 0) return;

        m_upgradeToChoose = m_upgradeManager.RandomUpgrade(3);
        for (int i = 0; i < 3; i++)
        {
            m_upgradeToChoose[i].Setup(m_characterShoot.capsuleIndex.Length);
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
    public void ChooseUpgrade(int indexChoice)
    {
        m_avatarUpgrade.Add(m_upgradeToChoose[indexChoice]);
        m_characterProfil.ApplyStat(CalculateStat(m_characterProfil.m_baseStat));
        upgradePoint--;
        m_upgradePoint.text = upgradePoint.ToString();
        DestroyAllUpgrade();
        if (upgradePoint == 0)
        {
            upgradeUiGO.SetActive(!upgradeUiGO.activeSelf);
            //uiLoaderDisplay.SetActive(true);
            //m_loaderBehavior.UpdateLoaderInUpgrade(true);
            Time.timeScale = 1;
            return;
        }
       
        GetNewUpgrade();
        m_upgradeUi.UpdateUpgradeDisplay(m_upgradeToChoose);
    }


    public void GainLevel()
    {
        upgradePoint++;
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
                m_avatarUpgrade[index].Apply(ref m_characterShoot.launcherStats) ;
                break;
            case UpgradeType.CAPSULE:
                m_avatarUpgrade[index].Apply(ref m_characterShoot.capsuleStatsAlone[m_avatarUpgrade[index].capsuleIndex]);
                break;
            default:
                break;
        }
    }
}
