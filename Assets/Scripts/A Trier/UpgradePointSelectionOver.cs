using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradePointSelectionOver : MonoBehaviour
{
    public int upgradePointCost = 1;
    private MeshRenderer m_myMaterial;
    public UpgradeDataInfo m_myUpgradeDataInfo;

    private MaterialPropertyBlock _propBlock;

    public TMP_Text upgradePointAssociated;
    // Start is called before the first frame update
    void Start()
    {
        _propBlock = new MaterialPropertyBlock();
        m_myMaterial = this.GetComponent<MeshRenderer>();
        //m_myUpgradeDataInfo = this.GetComponentInParent<UpgradeDataInfo>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Character.CharacterUpgrade.upgradePoint < upgradePointCost)
        {
            upgradePointAssociated.color = Color.red;
        }
        else
        {
            upgradePointAssociated.color = Color.black;
        }
    }

    private void OnMouseOver()
    {
        if (m_myUpgradeDataInfo.upgradeManager.ReturnUpgradeData() != m_myUpgradeDataInfo)
        {
            m_myUpgradeDataInfo.upgradeManager.NewUpgradeOvered(m_myUpgradeDataInfo);
            Debug.Log("Last Upgrade over changed (" + this.gameObject.name + ")");
        }
        m_myMaterial.GetPropertyBlock(_propBlock, 0);
        _propBlock.SetColor("_EmissiveColor", Color.white * 1);
        m_myMaterial.SetPropertyBlock(_propBlock, 0);
    }

    private void OnMouseExit()
    {
        //m_myMaterial.GetPropertyBlock(_propBlock, 0);
        _propBlock.SetColor("_EmissiveColor", Color.grey * 0.01f);
        m_myMaterial.SetPropertyBlock(_propBlock, 0);
    }
}
