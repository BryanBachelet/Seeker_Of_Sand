using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GuerhoubaGames.UI;
public class Merchant_Function : MonoBehaviour
{
    public bool activeMerchant = false;
    private bool openedUI;
    #region Inventory
    #region ressource
    public List<Image> cristalImage = new List<Image>();
    public List<TMP_Text> cristalCount = new List<TMP_Text>();
    #endregion
    #region Object

    public List<Image> fragmentHold = new List<Image>();
    public List<ArtefactsInfos> fragmentInfo = new List<ArtefactsInfos>();
    public List<TooltipTrigger> tooltipFragment = new List<TooltipTrigger>();
    public List<Image> spellNotUsedHold = new List<Image>();
    public List<CapsuleProfil> spellNotUsedProfil = new List<CapsuleProfil>();
    #endregion
    #region Spell
    public List<Image> spellUse = new List<Image>();
    public List<CapsuleProfil> spellProfil = new List<CapsuleProfil>();
    #endregion
    #endregion

    #region Shop
    #region SpellSell
    public List<Image> spellToBuyImage = new List<Image>();
    public List<CapsuleProfil> spellToBuyProfil = new List<CapsuleProfil>();
    public List<Image> cristalSpellBuyImage = new List<Image>();
    public List<TMP_Text> cristalSpellBuyCount = new List<TMP_Text>();
    #endregion
    #region FragmentSell
    public List<Image> fragmentToBuyImage = new List<Image>();
    public List<ArtefactsInfos> fragmentToBuyProfil = new List<ArtefactsInfos>();
    public List<Image> cristalFragmentBuyImage = new List<Image>();
    public List<TMP_Text> cristalFragmentBuyCount = new List<TMP_Text>();
    #endregion
    #region MysteryBag
    public List<Image> mysteryObject = new List<Image>();
    #endregion
    #endregion

    #region Component
    public GameObject player;
    public CristalInventory cristalInventory;
    public UI_Fragment_Tooltip fragmentToolTip;
    public GameObject fixeUI = null;
    public GameObject merchantUI = null;
    public GameObject inventoryUI = null;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        if(!cristalInventory) { player.GetComponent<CristalInventory>(); }
    }

    // Update is called once per frame
    void Update()
    {
        //if(activeMerchant)
        //{
        //    ActiveMerchnt();

        //}
        
    }

    public void ActiveMerchnt()
    {
        activeMerchant = false;
        if(inventoryUI.activeSelf) { openedUI = false; }
        else { openedUI = !openedUI; }

        if (openedUI)
        {
            fixeUI.SetActive(false);
            merchantUI.SetActive(true);
            if (!inventoryUI.activeSelf) { OpenInventory(); }
            OnDisplayMerchant();
        }
        else
        {
            fixeUI.SetActive(true);
            merchantUI.SetActive(false);
            OpenInventory();
            OnHideMerchant();
        }
    }
    public void OnDisplayMerchant()
    {

    }

    public void OnHideMerchant()
    {

    }

    public void OpenInventory()
    {
        return;
        if (!inventoryUI.activeSelf)
        {
            UpdateInventory();
            inventoryUI.SetActive(true);
        }
        else
        {
            if (merchantUI.activeSelf) return;
            else
            {
                inventoryUI.SetActive(false);
            }
        }


    }
    public void UpdateInventory()
    {
        for (int i = 0; i < cristalCount.Count; i ++)
        {
            cristalCount[i].text = "" +cristalInventory.cristalCount[i];
        }
        List<GameObject> tempFragment = fragmentToolTip.fragment_List;
        for (int i = 0; i < fragmentToolTip.currentFragmentNumber; i++)
        {
            fragmentHold[i].sprite = fragmentToolTip.imageFragmentTooltip[i].sprite;
            tooltipFragment[i].name = fragmentToolTip.tooltipTrigger[i].name;
            tooltipFragment[i].content = fragmentToolTip.tooltipTrigger[i].content;
            fragmentHold[i].gameObject.SetActive(true);
        }
    }
}
