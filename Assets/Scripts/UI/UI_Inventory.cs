using GuerhoubaGames.Character;
using GuerhoubaGames.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UI_Inventory : MonoBehaviour
{
    public bool isOpen;
    public GameObject inventoryContainer;
    public UIDispatcher dispatcher;

    private MarchandUiView m_marchandUiView;
    private CristalInventory m_cristalInventory;

    [Header("Trading Cristal")] //Eau --> Air --> Feu --> Terre --> Eau
    [SerializeField] private string m_cristalTradeString;
    [SerializeField] private int[] m_panierTrade = new int[4];
    [SerializeField] private TMP_Text[] tmpText_TradeCristal = new TMP_Text[4];
    [SerializeField] private TMP_Text[] tmpText_CurrentCristal = new TMP_Text[4];
    [SerializeField] private int m_DispanierTrade;
    [SerializeField] private TMP_Text tmpText_DisTradeCristal;
    [SerializeField] private TMP_Text tmpText_DisCurrentCristal;
    [SerializeField] private TMP_Text tmpText_TradeButton;
    public bool hasSomethingInPanier = false;

    private CharacterSpellBook m_characterSpellBool;
    private CharacterShoot m_characterShoot;
    [HideInInspector] public CharacterArtefact m_characterArtefact;
    private UI_Fragment_Tooltip m_fragmentToolTip;

    #region Resources variables
    public List<Image> cristalImage = new List<Image>();
    public List<TMP_Text> cristalCount = new List<TMP_Text>();
    public TMP_Text disCristalCount;

    #endregion

    #region Objects variables

    public FragmentUIView[] fragmentUIViews = new FragmentUIView[0];
    public Artefact_UI_View[] artefactUIView = new Artefact_UI_View[0];
    #endregion

    #region Spell variables
    public List<Image> spellUse = new List<Image>();
    public List<Image> cadreSpellUse = new List<Image>();
    [SerializeField] private Sprite[] spell_rarityCadre = new Sprite[4];
    public List<CapsuleProfil> spellProfil = new List<CapsuleProfil>();
    #endregion

    #region Profil variables
    [SerializeField] private TMP_Text m_healthBonusText;
    [SerializeField] private TMP_Text m_speedBonusText;
    [SerializeField] private TMP_Text m_damageBonusText;
    [SerializeField] private TMP_Text m_armorBonusText;

    #endregion

    public GameTutorialView gameTutorialView;
    private bool m_isFirstTimeOpen = true;
    // Avoid the close and t
    private bool IsNextCallDelay;
    public void InitComponent()
    {
        if (m_marchandUiView != null) return;

        m_marchandUiView = GameState.m_uiManager.GetComponent<UIDispatcher>().marchandUiView;
        tmpText_CurrentCristal = m_marchandUiView.GetCristalCount_TmpText;

        m_cristalInventory = GameState.s_playerGo.GetComponent<CristalInventory>();
        m_characterSpellBool = GameState.s_playerGo.GetComponent<CharacterSpellBook>();
        m_characterShoot = GameState.s_playerGo.GetComponent<CharacterShoot>();
        m_characterArtefact = GameState.s_playerGo.GetComponent<CharacterArtefact>();
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

    public void SetFragmentConditionnalUse(int idFamilyUse)
    {
        for (int i = 0; i < m_characterArtefact.artefactsList.Count; i++)
        {
            if (m_characterArtefact.artefactsList[i].idFamily != idFamilyUse)
            {
                fragmentUIViews[i].ActiveModeRestreint(true);
                fragmentUIViews[i].GetComponent<DragObjectUI>().isLock = true;
                artefactUIView[i].ActiveModeRestreint(true);
                artefactUIView[i].GetComponent<DragObjectUI>().isLock = true;
            }
        }

    }

    public void RemoveFragmentConditionalUse()
    {
        for (int i = 0; i < m_characterArtefact.artefactsList.Count; i++)
        {
            fragmentUIViews[i].ActiveModeRestreint(false);
            fragmentUIViews[i].GetComponent<DragObjectUI>().isLock = false;
            artefactUIView[i].ActiveModeRestreint(true);
            artefactUIView[i].GetComponent<DragObjectUI>().isLock = true;
        }

    }


    public void DeactivateInventoryInterface(bool delayNextInput = false)
    {
        if (!isOpen) return;

        isOpen = false;
        inventoryContainer.SetActive(isOpen);
        m_characterShoot.hasShootBlock = false;
        IsNextCallDelay = delayNextInput;
    }


    public void ActualizeInventory()
    {
        for (int i = 0; i < cristalCount.Count; i++)
        {
            cristalCount[i].text = "" + m_cristalInventory.cristalCount[i];
            tmpText_CurrentCristal[i].text = "" + m_cristalInventory.cristalCount[i];
        }
        disCristalCount.text = "" + m_cristalInventory.dissonanceCout;
        tmpText_DisCurrentCristal.text = "" + m_cristalInventory.dissonanceCout;
        List<GameObject> tempFragment = m_fragmentToolTip.fragment_List;
        for (int i = 0; i < tempFragment.Count; i++)
        {
            if (i < m_fragmentToolTip.currentFragmentNumber)
                SetupFragmentImage(m_fragmentToolTip, i);
            if (i >= m_fragmentToolTip.currentFragmentNumber)
                ClearFragmentImage(m_fragmentToolTip, i);

        }


        Sprite[] spellSprite = m_characterShoot.GetSpellSprite();
        int[] spellLevel = m_characterShoot.GetSpellLevel();
        for (int i = 0; i < spellSprite.Length && i < 4; i++)
        {
            spellUse[i].sprite = spellSprite[i];
            cadreSpellUse[i].sprite = spell_rarityCadre[(int)(spellLevel[i])];
        }
        CharacterStat stat = CharacterProfile.instance.stats;
        m_healthBonusText.text = ": " + (stat.healthMax.totalValue / 15);
        m_speedBonusText.text = ": " + (stat.runSpeed.totalValue / 5);
        m_damageBonusText.text = ": " + m_characterShoot.GetComponent<CharacterDamageComponent>().m_damageStats.damageBonusGeneral;
        m_armorBonusText.text = ": " + stat.armor;

    }

    public void OpenInventory()
    {
        if (IsNextCallDelay)
        {
            IsNextCallDelay = false;
            return;
        }


        if (!inventoryContainer.activeSelf)
        {
            ActualizeInventory();
            ActivateInventoryInterface();
            if (m_isFirstTimeOpen && GameManager.instance.generalSaveData.IsFirstTime)
            {
                gameTutorialView.StartTutoriel();
                m_isFirstTimeOpen = false;
            }
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
        fragmentUIViews[index].gameObject.SetActive(true);
        fragmentUIViews[index].UpdateInteface(m_characterArtefact.artefactsList[index]);
        artefactUIView[index].gameObject.SetActive(true);
        artefactUIView[index].UpdateInteface(m_characterArtefact.artefactsList[index]);
        m_characterArtefact.uiFragmentTooltip.SelectElement(m_fragmentToolTip.fragment_List[index], m_characterArtefact.artefactsList[index]);


    }

    public void ClearFragmentImage(UI_Fragment_Tooltip fragmentInfo, int index)
    {
        fragmentUIViews[index].gameObject.SetActive(false);
        fragmentUIViews[index].ResetFragmentUIView();
        artefactUIView[index].gameObject.SetActive(false);
        artefactUIView[index].ResetFragmentUIView();
    }


    public void CheckTradePossibility(int cristalElement)
    {
        if (m_cristalInventory.dissonanceCout <= 1) return;
        else
        {
            m_panierTrade[cristalElement] += 1;
            m_DispanierTrade -= 2;
            hasSomethingInPanier = true;

        }
        #region oldTrade 
            /*
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
        */
        #endregion
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
        m_cristalInventory.dissonanceCout += m_DispanierTrade;
        tmpText_DisTradeCristal.text = "";
        tmpText_DisTradeCristal.color = Color.white;
        m_DispanierTrade = 0;
        tmpText_TradeButton.color = Color.gray;
        ActualizeInventory();
    }

    public void UpdateTradeDisplay()
    {
        for (int i = 0; i < m_cristalInventory.cristalCount.Length; i++)
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
        if(m_DispanierTrade == 0)
        {
            tmpText_DisTradeCristal.text = "";
            tmpText_DisTradeCristal.color = Color.white;
        }
        else if (m_DispanierTrade < 0)
        {
            tmpText_DisTradeCristal.text = m_DispanierTrade + "";
            tmpText_DisTradeCristal.color = Color.red;
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
        m_DispanierTrade = 0;
        tmpText_DisTradeCristal.text = "";
        tmpText_DisTradeCristal.color = Color.white;
        tmpText_TradeButton.color = Color.gray;
        UpdateTradeDisplay();
    }

}
