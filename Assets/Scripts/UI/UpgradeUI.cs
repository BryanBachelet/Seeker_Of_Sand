using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;

[Serializable]
public struct UpgradeButton
{
    public int upgradeLink;
    public int numberOfUpgrade;
    public Button button;
}

public class UpgradeUI : MonoBehaviour
{

    public UpgradeButton[] upgradeButtons = new UpgradeButton[9];
    public UnityEngine.Events.UnityAction<int, int> m_upgradeButtonFunction;
    public Text[] m_upgradeName = new Text[3];
    public Text[] m_upgradeDescription = new Text[3];
    public Image[] m_upgradeIcon = new Image[3];
    public Image[] m_upgradeTypeIcon = new Image[3];
    public Material[] m_TypeIcon = new Material[3];
    public GameObject[] SelectionIcon = new GameObject[3];
    private Animator[] selectionAnimator = new Animator[3];
    public Animator[] upgradeButton = new Animator[3];
    [SerializeField] private CharacterUpgrade m_upgradeCharacter ;
    public int lastUpgradeSelected = 0;

    public UpgradeUIDecal m_uiDecalUpdaterDisplay;
    private void Start()
    {
        m_uiDecalUpdaterDisplay = UiSpellGrimoire.otherSceneGameObject[0].GetComponent<UpgradeUIDecal>();
        
    }
    public void UpdateUpgradeDisplay(Upgrade[] upgrades)
    {
        if(!UiSpellGrimoire.otherSceneGameObject[0].activeSelf) { UiSpellGrimoire.otherSceneGameObject[0].SetActive(true); }

        if (m_uiDecalUpdaterDisplay == null) { m_uiDecalUpdaterDisplay = UiSpellGrimoire.otherSceneGameObject[0].GetComponent<UpgradeUIDecal>(); }
        m_uiDecalUpdaterDisplay.UpdateUpgradeDisplay(upgrades);
        for (int i = 0; i < upgrades.Length; i++)
        {
            m_upgradeName[i].text = upgrades[i].gain.nameUgrade;
            m_upgradeDescription[i].text = upgrades[i].gain.description;
            m_upgradeIcon[i].sprite = upgrades[i].gain.icon_Associat;
            switch (upgrades[i].gain.type)
            {
                case UpgradeType.CHARACTER:
                    m_upgradeTypeIcon[i].material = m_TypeIcon[0];
                    break;
                case UpgradeType.LAUNCHER:
                    m_upgradeTypeIcon[i].material = m_TypeIcon[1];
                    break;
                case UpgradeType.CAPSULE:
                    m_upgradeTypeIcon[i].material = m_TypeIcon[2];
                    break;
                default:
                    break;
            }
        }
    }

    public void ChooseUpgrade(int index, int number)
    {
        m_upgradeCharacter.ChooseUpgrade(index,number);
    }

    public void UpdateCursorOver(int upgradeOvered)
    {
        if (lastUpgradeSelected == upgradeOvered) return;

        for (int i = 0; i < SelectionIcon.Length; i++)
        {
            if(upgradeOvered != i)
            {
                SelectionIcon[i].SetActive(false);
               // selectionAnimator[i].SetBool("Selected", false);
                //upgradeButton[i].SetBool("Selected", false);
                GlobalSoundManager.PlayOneShot(5, Camera.main.transform.position);
            }
            else
            {
                SelectionIcon[i].SetActive(true);
                //selectionAnimator[i].SetBool("Selected", true);
                //upgradeButton[i].SetBool("Selected", true);
                Debug.Log("Active numero : || " + i);
                lastUpgradeSelected = i;
            }
        }
    }
}
