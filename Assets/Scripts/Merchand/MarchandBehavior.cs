using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.GameEnum;

public struct ItemData
{
    public CharacterObjectType type;
    public GameElement element;
    public int price;
    public int index;
    public bool hasBeenBuy;
}

public struct MerchandItemData
{
    public ItemData[] itemSpellData;
    public SpellSystem.Capsule[] spellData;
    public ItemData[] fragmentData;
}

public class MarchandBehavior : InteractionInterface
{
    public enum BuyResult
    {
        BUY = 0 ,
        NOT_ENOUGH_MONEY = 1
    }


    private MarchandUiView m_merchandView;
    private UI_Inventory m_uiInventory;
    private UIDispatcher m_uiDispatcher;
    private CapsuleManager m_capsuleManager;
    private GameObject m_playerGo;
    private CristalInventory m_cristalInventory;

    [HideInInspector] public ItemData[] spellItemData = new ItemData[2];
    [HideInInspector] public ItemData[] fragmentItemData = new ItemData[2];

    private MerchandItemData merchandItemData;

    public void InitComponents()
    {
        if (GameState.m_uiManager) m_uiDispatcher = GameState.m_uiManager.GetComponent<UIDispatcher>();

        if (m_uiDispatcher == null) return;
        m_merchandView = m_uiDispatcher.marchandUiView;

        m_merchandView.marchandBehavior = this;
        m_cristalInventory = GameState.s_playerGo.GetComponent<CristalInventory>();

        m_uiInventory = m_uiDispatcher.uiInventory;
        m_capsuleManager = GameState.m_enemyManager.GetComponent<CapsuleManager>();
        SetSpellItem();
    }

    #region Interaction Functions
    public override void OnInteractionEnd(GameObject player)
    {
        m_merchandView.DeactiveMarchandUI();
        m_uiInventory.DeactivateInventoryInterface();
        m_uiDispatcher.fixeGameplayUI.SetActive(true);
        GameState.ChangeState();

    }

    public override void OnInteractionStart(GameObject player)
    {
        m_merchandView.ActiveMarchandUI(merchandItemData);
        m_uiDispatcher.fixeGameplayUI.SetActive(false);
        m_uiInventory.ActivateInventoryInterface();
        GameState.ChangeState();
    }
    #endregion


    public void SetSpellItem()
    {
        merchandItemData.spellData = new SpellSystem.Capsule[2];
        for (int i = 0; i < 2; i++)
        {
            int spellIndex = Random.Range(0, m_capsuleManager.capsules.Length);
            merchandItemData.spellData[i] = m_capsuleManager.capsules[spellIndex];
            spellItemData[i].index = spellIndex;
            spellItemData[i].price = 50;
            spellItemData[i].type = CharacterObjectType.SPELL;
            spellItemData[i].element = m_capsuleManager.attackInfo[spellIndex].element;
            spellItemData[i].hasBeenBuy = false;
        }

        merchandItemData.itemSpellData = spellItemData;
    }

    public BuyResult AcquiereNewSpell(int index)
    {
        // Check the price and pay price

        bool canPay = m_cristalInventory.HasEnoughCristal(merchandItemData.itemSpellData[index].price, merchandItemData.itemSpellData[index].element);

        if (!canPay || merchandItemData.itemSpellData[index].hasBeenBuy) return BuyResult.NOT_ENOUGH_MONEY;
        m_cristalInventory.RemoveCristalCount((int)merchandItemData.itemSpellData[index].element, -merchandItemData.itemSpellData[index].price);

        merchandItemData.itemSpellData[index].hasBeenBuy = true;
        // Add Spell to the player inventory
        Character.CharacterShoot characterShoot = GameState.s_playerGo.GetComponent<Character.CharacterShoot>();
        int spellIndex = merchandItemData.itemSpellData[index].index;
        characterShoot.AddSpell(spellIndex);
        // Update Interface from inventory and Store
        m_uiInventory.ActualizeInventory();

        return BuyResult.BUY;
    }
}
