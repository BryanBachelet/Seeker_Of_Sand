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
    // Start is called before the first frame update
    void Start()
    {
       
  
        m_iconSpellSelected = spellselectDescriptionPanelGameObject.GetComponentInChildren<Image>();
        m_textDescription = spellselectDescriptionPanelGameObject.GetComponentsInChildren<TMP_Text>();
        for(int i = 0; i < m_upgradeMesh.Length; i++)
        {
            m_upgradMat[i] = m_upgradeMesh[i].material;
            m_upgradeText[i] = m_upgradMat[i].mainTexture;
        }
        capacityAffectedIcon = capacityAffectedMesh.material;
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
            Debug.Log(upgrades[i].gain.icon_Associat.texture.name);
            m_upgradMat[i].mainTexture = upgrades[i].gain.icon_Associat.texture;
            m_upgradeText[i] = upgrades[i].gain.icon_Associat.texture;
        }
    }

    public void SpellFocusDisplay(CapsuleSystem.Capsule infoSpell)
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

}
