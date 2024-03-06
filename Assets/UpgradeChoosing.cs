using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class UpgradeChoosing : MonoBehaviour
{
    public int indexSpellBar = 0;
    [Header("UI Object")]
    public Image[] spellInBar = new Image[4];
    public Image spellUpgradeFocus;
    public TMPro.TMP_Text upgradePointText;
    public TMPro.TMP_Text[] upgradeSelectable = new TMPro.TMP_Text[3];
    public EventSystem eventSystem;
    private UpgradeLevelingData m_upgradeLevelingData;
    [HideInInspector] public UpgradeManager m_upgradeManager;


    public void SetNewUpgradeData(UpgradeLevelingData data)
    {
        m_upgradeLevelingData = data;
        for (int i = 0; i < m_upgradeLevelingData.spellCount; i++)
        {
            spellInBar[i].sprite = m_upgradeLevelingData.iconSpell[i];
        }

        upgradePointText.text = m_upgradeLevelingData.upgradePoint.ToString();
        spellUpgradeFocus.sprite = m_upgradeLevelingData.iconSpell[m_upgradeLevelingData.indexSpellFocus];
        for (int i = 0; i < 3; i++)
        {
            upgradeSelectable[i].text = ((m_upgradeLevelingData.upgradeChoose[i])).gain.nameUpgrade;
        }
    }

    public void UpdateUpgradesAvailable(Upgrade[] upgradeGenerate)
    {
        m_upgradeLevelingData.upgradeChoose = upgradeGenerate;
    }

    public void ChooseUpgrade(int index)
    {
        m_upgradeManager.SendUpgrade(m_upgradeLevelingData.upgradeChoose[index]);
        m_upgradeManager.m_dropInventory.AddNewUpgrade(m_upgradeLevelingData.upgradeChoose[index], spellUpgradeFocus.sprite);
    }

}