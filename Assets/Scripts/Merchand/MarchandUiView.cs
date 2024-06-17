using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GuerhoubaGames.UI;
using GuerhoubaGames.Resources;
using GuerhoubaGames.GameEnum;

public class MarchandUiView : MonoBehaviour
{
    public bool isOpen = false;
    public GameObject shopContainer;

    [HideInInspector] public MarchandBehavior marchandBehavior;

    private MerchandItemData merchandItemData;

    public MerchandUIOver[] merchandButtonElementArray;
    public TMP_Text negatifFeedbackText;
    public Image[] spellSpriteArray;
    public Image[] cristalSpriteArray;
    public TMP_Text[] cristalPriceTextArray;
    [Header("Descriptions Elements")]
    public TMP_Text nameDescriptionText;
    public TMP_Text descriptionElementText;
    public Image imageDescription;

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
        isOpen = true;
        shopContainer.SetActive(isOpen);
        merchandItemData = itemData;
        UpdateShopInterface();
    }

    public void DeactiveMarchandUI()
    {
        if (!isOpen) return;
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
    }

    public void InteractBuySpell(int index)
    {
       MarchandBehavior.BuyResult resultAction = marchandBehavior.AcquiereNewSpell(index);

        switch (resultAction)
        {
            case MarchandBehavior.BuyResult.BUY:
                ActualizeSpellInterface(index);
                break;
            case MarchandBehavior.BuyResult.NOT_ENOUGH_MONEY:
                negatifFeedbackText.text = "Not enough cristal!";
                break;
            default:
                break;
        }

    
    }

    public void ActualizeSpellInterface(int index)
    {
        ItemData itemData = merchandItemData.itemSpellData[index];
        if (itemData.hasBeenBuy)
        {
            spellSpriteArray[index].sprite = null;
            cristalPriceTextArray[index].text = "Sold";
            spellSpriteArray[index].color = new Color(1, 1, 1, 0);
        }
        else
        {
            spellSpriteArray[index].sprite = merchandItemData.spellData[index].sprite;
            cristalPriceTextArray[index].text = itemData.price.ToString();
        }
        cristalSpriteArray[index].sprite = GameResources.instance.cristalIconArray[(int)itemData.element];
    }

    public void ActualizeDescriptionInterface(int index,CharacterObjectType type)
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
                image = merchandItemData.spellData[index].sprite;
                break;
            case CharacterObjectType.FRAGMENT:
                break;
            default:
                break;
        }

        nameDescriptionText.text = name;
        descriptionElementText.text = description;
        imageDescription.sprite = image;
    }


}
