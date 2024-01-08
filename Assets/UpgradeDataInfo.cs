using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UpgradeDataInfo : MonoBehaviour
{
    public UpgradeUIDecal upgradeManager;

    public GameObject upg_Bandeau;
    public MeshRenderer upg_BandeauContour;
    public GameObject upg_Icon;
    public Material m_mat_Icon;
    public TMP_Text upg_Titre;
    public GameObject upg_IconRemainPoint;
    public TMP_Text upg_xRemainPoint;
    public GameObject upg_Iconx1;
    public TMP_Text upg_x1;
    public GameObject upg_Iconx3;
    public TMP_Text upg_x3;
    public GameObject upg_Iconx5;
    public TMP_Text upg_x5;

    public string upg_Description;

    public UpgradeProfil upg_Profil;
    public Animator m_myanimator;
    // Start is called before the first frame update
    void Start()
    {
        m_myanimator = this.GetComponent<Animator>();
        m_mat_Icon = upg_Icon.GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyUpgProfil(UpgradeProfil newProfil)
    {
        upg_Profil = newProfil;
        m_mat_Icon.mainTexture = upg_Profil.icon_Associat.texture;
        upg_Titre.text = upg_Profil.nameUgrade;
        upg_Description = upg_Profil.description;
        //m_upgradeName[i].text = upgrades[i].gain.nameUgrade;
        //m_upgradeDescription[i].text = upgrades[i].gain.description;
        //m_upgradeIcon[i].sprite = upgrades[i].gain.icon_Associat;
        //Debug.Log(upgrades[i].gain.icon_Associat.texture.name);
        //m_upgradMat[i].mainTexture = upgrades[i].gain.icon_Associat.texture;
        //m_upgradeText[i] = upgrades[i].gain.icon_Associat.texture;

    }

    public void OnMouseOver()
    {
        Debug.Log("Upgrade over (" + this.gameObject.name + ")");
        if(upgradeManager.ReturnUpgradeData() != this)
        {
            upgradeManager.NewUpgradeOvered(this);
            m_myanimator.SetBool("Overred", true);
            Debug.Log("Last Upgrade over changed (" + this.gameObject.name + ")");
        }
        
    }

}
