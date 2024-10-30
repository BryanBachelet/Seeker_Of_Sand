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

    [Header("Trading Cristal")] //Eau --> Air --> Feu --> Terre --> Eau
    [SerializeField] private string m_cristalTradeString;
    [SerializeField] private int[] m_panierTrade = new int[4];
    [SerializeField] private TMP_Text[] tmpText_TradeCristal = new TMP_Text[4];
    [SerializeField] private TMP_Text[] tmpText_CurrentCristal = new TMP_Text[4];
    [SerializeField] private TMP_Text tmpText_TradeButton;
    public bool hasSomethingInPanier = false;

    private Character.CharacterSpellBook m_characterSpellBool;
    private Character.CharacterShoot m_characterShoot;
    private UI_Fragment_Tooltip m_fragmentToolTip;

    #region Resources variables
    public List<Image> cristalImage = new List<Image>();
    public List<TMP_Text> cristalCount = new List<TMP_Text>();

    #endregion

    #region Objects variables

    public List<Image> fragmentHold = new List<Image>();
    public List<Image> fragmentTypeBackground = new List<Image>();
    public List<Image> fragmentRarityTier = new List<Image>();
    public List<TMP_Text> fragmentName = new List<TMP_Text>();
    public List<Image> fragmentElement = new List<Image>();
    public List<Image> bandeauFragmentName = new List<Image>();
    public List<ArtefactsInfos> fragmentInfo = new List<ArtefactsInfos>();
    public List<TooltipTrigger> tooltipFragment = new List<TooltipTrigger>();
    public List<Image> spellNotUsedHold = new List<Image>();
    public List<CapsuleProfil> spellNotUsedProfil = new List<CapsuleProfil>();

    public Sprite[] backgroundElement = new Sprite[4];
    public Sprite[] rarityCadre = new Sprite[3];
    public Sprite[] iconElement = new Sprite[4];
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
            tmpText_CurrentCristal[i].text = "" + m_cristalInventory.cristalCount[i];
        }
        List<GameObject> tempFragment = m_fragmentToolTip.fragment_List;
        for (int i = 0; i < m_fragmentToolTip.currentFragmentNumber; i++)
        {
            SetupFragmentImage(m_fragmentToolTip, i);

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

    public void SetupFragmentImage(UI_Fragment_Tooltip fragmentInfo, int index)
    {
        fragmentHold[index].sprite = m_fragmentToolTip.imageFragmentTooltip[index].sprite;
        tooltipFragment[index].name = m_fragmentToolTip.tooltipTrigger[index].name;
        tooltipFragment[index].content = m_fragmentToolTip.tooltipTrigger[index].content;
        int GameElementIndex = (int)fragmentInfo.fragmentInfo[index].gameElement;
        fragmentTypeBackground[index].sprite = backgroundElement[GameElementIndex];
        fragmentElement[index].sprite = iconElement[GameElementIndex];
       //if (fragmentInfo.fragmentInfo[index].gameElement == GuerhoubaGames.GameEnum.GameElement.WATER)
       //{
       //    fragmentTypeBackground[index].sprite = backgroundElement[0];
       //    fragmentElement[index].sprite = iconElement[0];
       //}
       //else if (fragmentInfo.fragmentInfo[index].gameElement == GuerhoubaGames.GameEnum.GameElement.AIR)
       //{
       //    fragmentTypeBackground[index].sprite = backgroundElement[1];
       //    fragmentElement[index].sprite = iconElement[1];
       //}
       //else if (fragmentInfo.fragmentInfo[index].gameElement == GuerhoubaGames.GameEnum.GameElement.FIRE)
       //{
       //    fragmentTypeBackground[index].sprite = backgroundElement[2];
       //    fragmentElement[index].sprite = iconElement[2];
       //}
       //else if (fragmentInfo.fragmentInfo[index].gameElement == GuerhoubaGames.GameEnum.GameElement.EARTH)
       //{
       //    fragmentTypeBackground[index].sprite = backgroundElement[3];
       //    fragmentElement[index].sprite = iconElement[3];
       //}
        fragmentRarityTier[index].sprite = rarityCadre[0];
        fragmentName[index].text = fragmentInfo.fragmentInfo[index].nameArtefact;
        fragmentHold[index].gameObject.SetActive(true);
        fragmentTypeBackground[index].gameObject.SetActive(true);
        fragmentRarityTier[index].gameObject.SetActive(true);
        fragmentName[index].gameObject.SetActive(true);
        fragmentElement[index].gameObject.SetActive(true);
        bandeauFragmentName[index].gameObject.SetActive(true);
        //if(fragmentInfo.)
        //fragmentTypeBackground[index].sprite
    }


    public void CheckTradePossibility(int cristalElement)
    {
        if (cristalElement == 0)
        {
            if (m_cristalInventory.cristalCount[3] + m_panierTrade[3] < 2)
            {
                Debug.Log("Not enought cristal");
                return;
            }
            else
            {
                m_panierTrade[cristalElement] += 1;
                m_panierTrade[3] -= 2;
                hasSomethingInPanier = true;

            }

        }
        else
        {
            if (m_cristalInventory.cristalCount[cristalElement - 1] + m_panierTrade[cristalElement - 1] < 2)
            {
                Debug.Log("Not enought cristal");
                return;
            }
            else
            {
                m_panierTrade[cristalElement] += 1;
                m_panierTrade[cristalElement - 1] -= 2;
                hasSomethingInPanier = true;
            }
        }
        UpdateTradeDisplay();
        if (hasSomethingInPanier) { tmpText_TradeButton.color = Color.white; }
        else { tmpText_TradeButton.color = Color.gray; }
    }

    public void UpdateTradeResult()
    {
        if (!hasSomethingInPanier) return;
        hasSomethingInPanier = false;
        for (int i = 0; i < m_cristalInventory.cristalCount.Length; i++)
        {
            m_cristalInventory.cristalCount[i] += m_panierTrade[i];
            tmpText_TradeCristal[i].text = "";
            tmpText_TradeCristal[i].color = Color.white;
            m_panierTrade[i] = 0;
        }
        tmpText_TradeButton.color = Color.gray;
        ActualizeInventory();
    }

    public void UpdateTradeDisplay()
    {
        for(int i = 0; i < m_cristalInventory.cristalCount.Length; i++)
        {
            if (m_panierTrade[i] == 0)
            {
                tmpText_TradeCristal[i].text = "";
                tmpText_TradeCristal[i].color = Color.white;
            }
            else if (m_panierTrade[i] > 0)
            {
                tmpText_TradeCristal[i].text = m_panierTrade[i] + "";
                tmpText_TradeCristal[i].color = Color.green;
            }
            else if (m_panierTrade[i] < 0)
            {
                tmpText_TradeCristal[i].text = m_panierTrade[i] + "";
                tmpText_TradeCristal[i].color = Color.red;
            }
            //
        }
    }

    public void CancelTrade()
    {
        if (!hasSomethingInPanier) return;
        hasSomethingInPanier = false;
        for (int i = 0; i < m_panierTrade.Length; i++)
        {
            m_panierTrade[i] = 0;
            tmpText_TradeCristal[i].text = "";
            tmpText_TradeCristal[i].color = Color.white;
        }
        tmpText_TradeButton.color = Color.gray;
        UpdateTradeDisplay();
    }

}
