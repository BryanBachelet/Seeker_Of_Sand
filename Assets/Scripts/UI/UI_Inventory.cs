using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GuerhoubaGames.UI;
public class UI_Inventory : MonoBehaviour
{
    public bool isOpen;
    public GameObject inventoryContainer;


    private MarchandUiView m_marchandUiView;
    private CristalInventory m_cristalInventory;
    private Character.CharacterSpellBook m_characterSpellBool;
    private Character.CharacterShoot m_characterShoot;
    private UI_Fragment_Tooltip m_fragmentToolTip;

    #region Resources variables
    public List<Image> cristalImage = new List<Image>();
    public List<TMP_Text> cristalCount = new List<TMP_Text>();

    #endregion

    #region Objects variables

    public List<Image> fragmentHold = new List<Image>();
    public List<ArtefactsInfos> fragmentInfo = new List<ArtefactsInfos>();
    public List<TooltipTrigger> tooltipFragment = new List<TooltipTrigger>();
    public List<Image> spellNotUsedHold = new List<Image>();
    public List<CapsuleProfil> spellNotUsedProfil = new List<CapsuleProfil>();
    #endregion

    #region Spell variables
    public List<Image> spellUse = new List<Image>();
    public List<CapsuleProfil> spellProfil = new List<CapsuleProfil>();
    #endregion

    public void InitComponent()
    {
        if (m_marchandUiView != null) return;

        m_marchandUiView = GameState.m_uiManager.GetComponent<UIDispatcher>().marchandUiView;
        m_cristalInventory = GameState.s_playerGo.GetComponent<CristalInventory>();
        m_characterSpellBool = GameState.s_playerGo.GetComponent<Character.CharacterSpellBook>();
        m_characterShoot = GameState.s_playerGo.GetComponent<Character.CharacterShoot>();
        m_fragmentToolTip = GameState.m_uiManager.GetComponent<UI_Fragment_Tooltip>();
    }

    public void ActivateInventoryInterface()
    {
        if (isOpen) return;
        isOpen = true;
        inventoryContainer.SetActive(isOpen);
        ActualizeInventory();
        m_characterShoot.hasShootBlock = true;
    }

    public void DeactivateInventoryInterface()
    {
        if (!isOpen) return;

        isOpen = false;
        inventoryContainer.SetActive(isOpen);
        m_characterShoot.hasShootBlock = false;
    }


    public void ActualizeInventory()
    {
        for (int i = 0; i < cristalCount.Count; i++)
        {
            cristalCount[i].text = "" + m_cristalInventory.cristalCount[i];
        }
        List<GameObject> tempFragment = m_fragmentToolTip.fragment_List;
        for (int i = 0; i < m_fragmentToolTip.currentFragmentNumber; i++)
        {
            fragmentHold[i].sprite = m_fragmentToolTip.imageFragmentTooltip[i].sprite;
            tooltipFragment[i].name = m_fragmentToolTip.tooltipTrigger[i].name;
            tooltipFragment[i].content = m_fragmentToolTip.tooltipTrigger[i].content;
            fragmentHold[i].gameObject.SetActive(true);
        }

        Sprite[] spellSprite = m_characterShoot.GetSpellSprite();
        for (int i = 0; i < spellSprite.Length && i < 4; i++)
        {
            spellUse[i].sprite = spellSprite[i];
        }
    }

    public void OpenInventory()
    {

        if (!inventoryContainer.activeSelf)
        {
            ActualizeInventory();
            ActivateInventoryInterface();
        }
        else
        {
            if (m_marchandUiView.isOpen) return;
            else
            {
                DeactivateInventoryInterface();
            }
        }


    }


}
