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
    public Text m_upgradePoint;

    private UpgradeManager m_upgradeManager;
    private UpgradeUI m_upgradeUi;
    private CharacterProfile m_characterProfil;

    private Upgrade[] m_upgradeToChoose = new Upgrade[3];


    public void UpgradeWindowInput(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            if (upgradePoint == 0) return;
            // Input Function 
            upgradeUiGO.SetActive(!upgradeUiGO.activeSelf);
            if (upgradeUiGO.activeSelf == false) return;
            GetNewUpgrade();
            m_upgradeUi.OpenUpgrade(m_upgradeToChoose);

        }
    }

    public void Start()
    {
        m_upgradeManager = FindObjectOfType<UpgradeManager>();
        m_upgradeUi = upgradeUiGO.GetComponent<UpgradeUI>();
        m_characterProfil = GetComponent<CharacterProfile>();
        m_upgradePoint.text = upgradePoint.ToString();
    }

    public void GetNewUpgrade()
    {
        if (upgradePoint == 0) return;

        m_upgradeToChoose = m_upgradeManager.RandomUpgrade(3);
       
    }

    public void ChooseUpgrade(int indexChoice)
    {
        m_avatarUpgrade.Add(m_upgradeToChoose[indexChoice]);
        m_characterProfil.ApplyStat(CalculateStat(m_characterProfil.m_baseStat));
        upgradePoint--;
        m_upgradePoint.text = upgradePoint.ToString();
        if (upgradePoint == 0)
        {
            upgradeUiGO.SetActive(!upgradeUiGO.activeSelf);
            return;
        }
        GetNewUpgrade();
        m_upgradeUi.OpenUpgrade(m_upgradeToChoose);
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
            m_avatarUpgrade[i].Apply(ref newStats);
        }

        return newStats;
    }
}
