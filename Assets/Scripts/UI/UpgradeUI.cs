using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    public Text[] m_upgradeDescription = new Text[3];
    public Image[] m_upgradeIcon = new Image[3];
    public Image[] m_upgradeTypeIcon = new Image[3];
    public Material[] m_TypeIcon = new Material[3];
    public GameObject[] SelectionIcon = new GameObject[3];
    private Animator[] selectionAnimator = new Animator[3];
    public Animator[] upgradeButton = new Animator[3];
    [SerializeField] private CharacterUpgrade m_upgradeCharacter ;
    public int lastUpgradeSelected = 0;

    private void Start()
    {
        for(int i = 0; i < SelectionIcon.Length; i++)
        {
            selectionAnimator[i] = SelectionIcon[i].GetComponent<Animator>();
        }
    }
    public void UpdateUpgradeDisplay(Upgrade[] upgrades)
    {
        for (int i = 0; i < upgrades.Length; i++)
        {
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

    public void ChooseUpgrade(int index)
    {
        m_upgradeCharacter.ChooseUpgrade(index);
    }

    public void UpdateCursorOver(int upgradeOvered)
    {
        if (lastUpgradeSelected == upgradeOvered) return;

        for (int i = 0; i < SelectionIcon.Length; i++)
        {
            if(upgradeOvered != i)
            {
                SelectionIcon[i].SetActive(false);
                selectionAnimator[i].SetBool("Selected", false);
                upgradeButton[i].SetBool("Selected", false);
                GlobalSoundManager.PlayOneShot(5, Camera.main.transform.position);
            }
            else
            {
                SelectionIcon[i].SetActive(true);
                selectionAnimator[i].SetBool("Selected", true);
                upgradeButton[i].SetBool("Selected", true);
                Debug.Log("Active numero : || " + i);
                lastUpgradeSelected = i;
            }
        }
    }
}
