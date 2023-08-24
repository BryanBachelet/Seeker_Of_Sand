using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUIDecal : MonoBehaviour
{
    public Text[] m_upgradeName = new Text[3];
    public Text[] m_upgradeDescription = new Text[3];
    public Image[] m_upgradeIcon = new Image[3];
    public Image[] m_upgradeTypeIcon = new Image[3];

    public GameObject upgradePanelGameObject;

    public GameObject selectionPanelGameObject;
    public GameObject spellEquipPanelGameObject;

    public GameObject upgradeScreenState;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateUpgradeDisplay(Upgrade[] upgrades)
    {
        for (int i = 0; i < upgrades.Length; i++)
        {
            m_upgradeName[i].text = upgrades[i].gain.nameUgrade;
            m_upgradeDescription[i].text = upgrades[i].gain.description;
            m_upgradeIcon[i].sprite = upgrades[i].gain.icon_Associat;
        }
    }

    public void ChangeStateDisplay(bool newState)
    {
        selectionPanelGameObject.SetActive(newState);
        spellEquipPanelGameObject.SetActive(newState);
    }

}
