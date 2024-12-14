using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropInventory : MonoBehaviour
{
    public HintDropAcquisition hintDropAcquisitionObject;
    public HintDropAcquisition hintDropAcquisitionObjectUiOver;
    public HintDropAcquisition.DropInfo lastDropInfo;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddNewItem(int capsuleID)
    {
        lastDropInfo.m_dropType = "[Spell]";
        lastDropInfo.m_dropImage = SpellManager.instance.spellProfils[capsuleID].spell_Icon;
        lastDropInfo.dropDescription = SpellManager.instance.spellProfils[capsuleID].description;
        lastDropInfo.dropName = SpellManager.instance.spellProfils[capsuleID].name;
        hintDropAcquisitionObject.AddMajorDrop(lastDropInfo);
        hintDropAcquisitionObjectUiOver.AddMajorDrop(lastDropInfo);
    }

    public void AddNewArtefact(ArtefactsInfos artefact)
    {
        lastDropInfo.m_dropType = "[Artefact]";
        lastDropInfo.m_dropImage = artefact.icon;
        lastDropInfo.dropDescription = artefact.descriptionResult;
        lastDropInfo.dropName = artefact.nameArtefact;
        hintDropAcquisitionObject.AddMajorDrop(lastDropInfo);
        hintDropAcquisitionObjectUiOver.AddMajorDrop(lastDropInfo);
    }

    public void AddNewUpgrade(UpgradeObject upgradeData, Sprite spriteSpell)
    {
        lastDropInfo.m_dropType = "[Upgrade]";
        lastDropInfo.m_dropImage = spriteSpell;
        lastDropInfo.dropDescription = upgradeData.description;
        lastDropInfo.dropName = upgradeData.name;
        hintDropAcquisitionObject.AddNewMinorDrop(lastDropInfo);
    }
}
