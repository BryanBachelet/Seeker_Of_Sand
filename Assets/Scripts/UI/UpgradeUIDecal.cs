using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UpgradeUIDecal : MonoBehaviour
{
    public TMP_Text[] m_upgradeName = new TMP_Text[3];
    public Text[] m_upgradeDescription = new Text[3];
    public Image[] m_upgradeIcon = new Image[3];
    public MeshRenderer[] m_upgradeMesh = new MeshRenderer[3];
    public Material[] m_upgradMat;
    public Texture[] m_upgradeText;
    public Image[] m_upgradeTypeIcon = new Image[3];

    public GameObject upgradePanelGameObject;

    public GameObject selectionPanelGameObject;
    public GameObject spellEquipPanelGameObject;
    public GameObject spellselectDescriptionPanelGameObject;
    public GameObject upgradeSelectedDetails;

    private Image m_iconSpellSelected;
    private TMP_Text[] m_textDescription;

    public GameObject upgradeScreenState;

    public MeshRenderer capacityAffectedMesh;
    [HideInInspector] public Material capacityAffectedIcon;
    public TMP_Text capacityAffectedName;
    public TMP_Text upgradeSelectedName;
    public TMP_Text upgradeDescription;
    public TMP_Text initialValue;
    public TMP_Text newValue;

    public TMP_Text upgradAvailable;

    public UpgradeDataInfo[] upg_DataHolder = new UpgradeDataInfo[3];
    public UpgradeDataInfo lastUpgradeOverd;
    private Animator lastUpgradeAnimator;

    private MaterialPropertyBlock _propBlock;
    private bool m_isFirstOpening = true;
  

    public void InitUpgradeDecal()
    {
        _propBlock = new MaterialPropertyBlock();
        m_iconSpellSelected = spellselectDescriptionPanelGameObject.GetComponentInChildren<Image>();
        m_textDescription = spellselectDescriptionPanelGameObject.GetComponentsInChildren<TMP_Text>();
        for (int i = 0; i < m_upgradeMesh.Length; i++)
        {
            m_upgradMat[i] = m_upgradeMesh[i].material;
            m_upgradeText[i] = m_upgradMat[i].mainTexture;
        }
        capacityAffectedIcon = capacityAffectedMesh.material;
        m_isFirstOpening = false;
    }

    public void UpdateUpgradeDisplay(Upgrade[] upgrades)
    {
        if (m_isFirstOpening) InitUpgradeDecal();

        for (int i = 0; i < upgrades.Length; i++)
        {
            m_upgradeName[i].text = upgrades[i].gain.nameUgrade;
            m_upgradeDescription[i].text = upgrades[i].gain.description;
            m_upgradeIcon[i].sprite = upgrades[i].gain.icon_Associat;
            Debug.Log(upgrades[i].gain.icon_Associat.texture.name);
            m_upgradMat[i].mainTexture = upgrades[i].gain.icon_Associat.texture;
            m_upgradeText[i] = upgrades[i].gain.icon_Associat.texture;
            upg_DataHolder[i].ApplyUpgProfil(upgrades[i].gain);
        }

    }

    public void SpellFocusDisplay(SpellSystem.Capsule infoSpell)
    {
        m_textDescription[0].text = infoSpell.name;
        m_textDescription[1].text = infoSpell.description;
        m_iconSpellSelected.sprite = infoSpell.sprite;
    }
    public void ChangeStateDisplay(bool newState)
    {
        selectionPanelGameObject.SetActive(newState);
        spellEquipPanelGameObject.SetActive(newState);
        spellselectDescriptionPanelGameObject.SetActive(newState);
    }

    public void FocusUpgrade(Upgrade upgradeSelected)
    {
        if(!upgradeSelectedDetails.activeSelf) { upgradeSelectedDetails.SetActive(true); }

        capacityAffectedIcon.mainTexture = upgradeSelected.gain.icon_Associat.texture;
        upgradeSelectedName.text = upgradeSelected.gain.name;
        upgradeDescription.text = upgradeSelected.gain.description;
        
    }

    public UpgradeDataInfo ReturnUpgradeData()
    {

        return lastUpgradeOverd;
    }

    public void NewUpgradeOvered(UpgradeDataInfo NewUpgradeDataInfo)
    {
        if(lastUpgradeAnimator != null) 
        {
            lastUpgradeAnimator.SetBool("Overred", false);
        }
        lastUpgradeOverd = NewUpgradeDataInfo;
        lastUpgradeAnimator = NewUpgradeDataInfo.GetComponent<Animator>();
        upgradeSelectedName.text = lastUpgradeOverd.upg_Titre.text;
        upgradeDescription.text = lastUpgradeOverd.upg_Description;
        capacityAffectedIcon.mainTexture = lastUpgradeOverd.m_mat_Icon.mainTexture;
        for(int i = 0; i < upg_DataHolder.Length; i++)
        {
            upg_DataHolder[i].upg_BandeauContour.GetPropertyBlock(_propBlock, 0);
            if (upg_DataHolder[i] != lastUpgradeOverd)
            {
                _propBlock.SetColor("_EmissiveColor", Color.white * 0);
                //upg_DataHolder[i].upg_BandeauContour.material.SetFloat("_EmissiveIntensity", 0);
                //upg_DataHolder[i].upg_BandeauContour.sharedMaterial.EnableKeyword("_EMISSION");
            }
            else
            {
                _propBlock.SetColor("_EmissiveColor", Color.white * 5);
            }
            upg_DataHolder[i].upg_BandeauContour.SetPropertyBlock(_propBlock, 0);
        }

    }

    public void DisplayModification()
    {

    }

}
