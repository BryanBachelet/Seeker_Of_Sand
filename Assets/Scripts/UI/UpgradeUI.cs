using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    public Text[] m_upgradeDescription = new Text[3];
    public Image[] m_upgradeIcon = new Image[3];
    [SerializeField] private CharacterUpgrade m_upgradeCharacter ;


    public void UpdateUpgradeDisplay(Upgrade[] upgrades)
    {
        for (int i = 0; i < upgrades.Length; i++)
        {
            m_upgradeDescription[i].text = upgrades[i].gain.description;
            m_upgradeIcon[i].sprite = upgrades[i].gain.icon_Associat;
        }
    }

    public void ChooseUpgrade(int index)
    {
        m_upgradeCharacter.ChooseUpgrade(index);
    }
}
