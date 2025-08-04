using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GuerhoubaGames.UI;
using GuerhoubaGames.Resources;
using GuerhoubaGames.GameEnum;
using SeekerOfSand.Tools;

public class MarchandUiView : MonoBehaviour
{
    public bool isOpen = false;
    public GameObject shopContainer;

    [HideInInspector] public MarchandBehavior marchandBehavior;

    private MerchandItemData merchandItemData;

    public MerchandUIOver[] merchandButtonElementArray;
    public TMP_Text negatifFeedbackText;

    public Color noEnoughMoneyColor = Color.red;
    public Color canBuyColor = Color.white;

    [Header("Fragments Elements")]
    public Image[] fragmentSpriteArray;
    public Image[] cristalFragmentSpriteArray;
    public TMP_Text[] fragmentPriceTextArray;
    public TMP_Text[] fragmentNameArray;
    public Image[] fragmentBackground;
    public Image[] fragmentElementIcon;
    public Image[] fragmentRarity;

    [Header("Spells Elements")]
    public Image[] spellSpriteArray;
    public Image[] cristalSpellSpriteArray;
    public TMP_Text[] spellPriceTextArray;
    [Header("Descriptions Elements")]
    public TMP_Text nameDescriptionText;
    public TMP_Text descriptionElementText;
    public Image imageDescription;

    [Header("Gamepad Parameters")]
    public GameObject firstButtonToSelect;

    public Sprite[] backgroundElement = new Sprite[4];
    public Sprite[] rarityCadre = new Sprite[3];
    public Sprite[] iconElement = new Sprite[4];

    [SerializeField] private TMP_Text[] currentCristalTrade = new TMP_Text[4];
    public void Start()
    {
        InitEventComponent();
    }


    public void InitEventComponent()
    {
        for (int i = 0; i < merchandButtonElementArray.Length; i++)
        {
            merchandButtonElementArray[i].OnEnter += ActualizeDescriptionInterface;
        }
    }

    public void ActiveMarchandUI(MerchandItemData itemData)
    {
        if (isOpen) return;
        GlobalSoundManager.PlayOneShot(40, transform.position);
        isOpen = true;
        shopContainer.SetActive(isOpen);
        merchandItemData = itemData;


        if (GameState.instance.IsGamepad())
            UITools.instance.SetUIObjectSelect(firstButtonToSelect); 
        
            UpdateShopInterface();
    }

    public void DeactiveMarchandUI()
    {
        if (!isOpen) return;
        GlobalSoundManager.PlayOneShot(41, transform.position);
        isOpen = false;
        shopContainer.SetActive(isOpen);
    }

    public void UpdateShopInterface()
    {
        negatifFeedbackText.text = "";
        // Updating Spell Interface 
        for (int i = 0; i < merchandItemData.spellData.Length; i++)
        {
            ItemData itemData = merchandItemData.itemSpellData[i];

            ActualizeSpellInterface(i);
        }

        for (int i = 0; i < merchandItemData.fragmentData.Length; i++)
        {
            ActualizeFragmentInteface(i);
        }
    }

    public void InteractBuySpell(int index)
    {
        BuyResult resultAction = marchandBehavior.AcquiereNewSpell(index);

        switch (resultAction)
        {
            case BuyResult.BUY:
                ActualizeSpellInterface(index);
                GlobalSoundManager.PlayOneShot(43, transform.position);
                break;
            case BuyResult.NOT_ENOUGH_MONEY:
                negatifFeedbackText.text = "Not enough cristal!";
                GlobalSoundManager.PlayOneShot(42, transform.position);
                break;
            default:
                break;
        }
        ActualizeShop();
    }


    public void InteractBuyFragment(int index)
    {
        BuyResult resultAction = marchandBehavior.AcquiereNewFragment(index);

        switch (resultAction)
        {
            case BuyResult.BUY:
                ActualizeFragmentInteface(index);
                GlobalSoundManager.PlayOneShot(44, transform.position);
                break;
            case BuyResult.NOT_ENOUGH_MONEY:
                negatifFeedbackText.text = "Not enough cristal!";
                GlobalSoundManager.PlayOneShot(42, transform.position);
                break;
            default:
                break;
        }
        ActualizeShop();
    }

    public void ActualizeShop()
    {
        for (int i = 0; i < merchandItemData.itemSpellData.Length; i++)
        {
            ItemData itemData = merchandItemData.itemSpellData[i];
            if(itemData.isBuyable) spellPriceTextArray[i].color = canBuyColor;
            else spellPriceTextArray[i].color = noEnoughMoneyColor;
        }
        for (int i = 0; i < merchandItemData.itemFragmentData.Length; i++)
        {
            ItemData itemData = merchandItemData.itemFragmentData[i];
            if (itemData.isBuyable) fragmentPriceTextArray[i].color = canBuyColor;
            else fragmentPriceTextArray[i].color = noEnoughMoneyColor;
        }
    }

    public void ActualizeSpellInterface(int index)
    {
        ItemData itemData = merchandItemData.itemSpellData[index];
        if (itemData.hasBeenBuy)
        {
            spellSpriteArray[index].sprite = null;
            spellPriceTextArray[index].text = "Sold";
            spellSpriteArray[index].color = new Color(1, 1, 1, 0);
        }
        else
        {
            spellSpriteArray[index].sprite = merchandItemData.spellData[index].spell_Icon;
            spellPriceTextArray[index].text = itemData.price.ToString();
            spellSpriteArray[index].color = new Color(1, 1, 1, 1);

            if (itemData.isBuyable) fragmentSpriteArray[index].color = canBuyColor;
            else fragmentSpriteArray[index].color = noEnoughMoneyColor;

        }
        int indexElement = GeneralTools.GetElementalArrayIndex(itemData.element);
        cristalSpellSpriteArray[index].sprite = GameResources.instance.cristalIconArray[indexElement];
    }

    public void ActualizeFragmentInteface(int index)
    {
         ItemData itemData = merchandItemData.itemFragmentData[index];
        if (itemData.hasBeenBuy)
        {
            fragmentSpriteArray[index].sprite = null;
            fragmentPriceTextArray[index].text = "Sold";
            fragmentSpriteArray[index].color = new Color(1, 1, 1, 0);
        }
        else
        {
            fragmentSpriteArray[index].sprite = merchandItemData.fragmentData[index].icon;
            fragmentPriceTextArray[index].text = itemData.price.ToString();
            ChangeFragmentCadre(merchandItemData.fragmentData[index], index);

           if( itemData.isBuyable) fragmentSpriteArray[index].color =canBuyColor;
           else fragmentSpriteArray[index].color = noEnoughMoneyColor;
            fragmentSpriteArray[index].color = new Color(1, 1, 1, 1);
        }
        int indexElement = GeneralTools.GetElementalArrayIndex(itemData.element);
        cristalFragmentSpriteArray[index].sprite = GameResources.instance.cristalIconArray[indexElement];
    }

    public void ActualizeDescriptionInterface(int index, CharacterObjectType type)
    {
        string name, description;
        name = "";
        description = "";
        Sprite image = null;
        switch (type)
        {
            case CharacterObjectType.SPELL:
                if (merchandItemData.itemSpellData[index].hasBeenBuy) return;
                description = merchandItemData.spellData[index].description;
                name = merchandItemData.spellData[index].name;
                image = merchandItemData.spellData[index].spell_Icon;
                break;
            case CharacterObjectType.FRAGMENT:
                if (merchandItemData.itemFragmentData[index].hasBeenBuy) return;
                description = merchandItemData.fragmentData[index].baseDescription;
                name = merchandItemData.fragmentData[index].name;
                image = merchandItemData.fragmentData[index].icon; ;
                break;
            default:
                break;
        }

        nameDescriptionText.text = name;
        descriptionElementText.text = description;
        imageDescription.sprite = image;
    }


    public void DropElementMysteryBag(DragData dragData)
    {
        switch (dragData.currentType)
        {
            case CharacterObjectType.SPELL:
                marchandBehavior.AcquiereRandomNewSpell(dragData);
                break;
            case CharacterObjectType.FRAGMENT:
                marchandBehavior.AcquiereRandomNewFragment(dragData);
                break;
            default:
                break;
        }
    }

    public void ChangeFragmentCadre(ArtefactsInfos fragmentData, int indexFragmentInShop)
    {
        int elementIndex = GeneralTools.GetElementalArrayIndex(fragmentData.gameElement, true);

        fragmentBackground[indexFragmentInShop].sprite = backgroundElement[elementIndex];
        fragmentElementIcon[indexFragmentInShop].sprite = iconElement[elementIndex - 1];
        fragmentNameArray[indexFragmentInShop].text = fragmentData.nameArtefact;
        
    }

    public TMP_Text[] GetCristalCount_TmpText { get { return currentCristalTrade; } }
}
