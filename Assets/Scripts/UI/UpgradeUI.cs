using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    public Text[] m_upgradeDescription = new Text[3];
    [SerializeField] private CharacterUpgrade m_upgradeCharacter ;


    public void OpenUpgrade(Upgrade[] upgrades)
    {
        for (int i = 0; i < upgrades.Length; i++)
        {
            m_upgradeDescription[i].text = upgrades[i].gain.description;
        }
    }

    public void ChooseUpgrade(int index)
    {
        m_upgradeCharacter.ChooseUpgrade(index);
    }
}
