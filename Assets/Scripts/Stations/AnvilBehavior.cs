using GuerhoubaGames.GameEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.UI;

public class AnvilBehavior : InteractionInterface
{

    private AnvilUIView m_anvilUIComponent;
    private UI_Inventory m_uiInventory;
    private CristalInventory m_cristalInventory;
    [HideInInspector] public ArtefactsInfos currentArtefactReinforce;


    public int costT1 = 100;
    public int costT2 = 150;

    public bool isBuylessActive;
    private UIDispatcher m_uiDispatcher;


    public void Start()
    {
        InitComponent();
    }

    public void InitComponent()
    {
        if (GameState.m_uiManager) m_uiDispatcher = GameState.m_uiManager.GetComponent<UIDispatcher>();

        m_anvilUIComponent = m_uiDispatcher.anvilUIView;
        m_uiInventory = m_uiDispatcher.uiInventory;
        m_cristalInventory = GameState.s_playerGo.GetComponent<CristalInventory>();
        m_anvilUIComponent.anvilBehavior = this;
    }

    public void SetFragmentUpgrade()
    {
        currentArtefactReinforce.UpgradeTierFragment();
        m_uiInventory.ActualizeInventory();
    }

    public BuyResult BuyUpgradeFragment()
    {
        if (isBuylessActive)
            return BuyResult.BUY;

        bool isT1 = currentArtefactReinforce.levelTierFragment == LevelTier.TIER_1 ? true : false;
        int value = isT1 ? costT1 : costT2;
        bool hasEnoughCristal = m_cristalInventory.HasEnoughCristal(value, currentArtefactReinforce.gameElement, currentArtefactReinforce.nameArtefact);
        if (!hasEnoughCristal) return BuyResult.NOT_ENOUGH_MONEY;

        m_cristalInventory.RemoveCristalCount((int)currentArtefactReinforce.gameElement, -value);
        // TODO : Update Anvil Upgrade price;


        return BuyResult.BUY;
    }

    public override void OnInteractionStart(GameObject player)
    {
        m_anvilUIComponent.OpenUiAnvil();
        m_uiInventory.ActivateInventoryInterface();
        GameState.ChangeState();
    }

    public override void OnInteractionEnd(GameObject player)
    {
        m_anvilUIComponent.CloseUIAnvil();
        m_uiInventory.DeactivateInventoryInterface();
        GameState.ChangeState();

    }

    public bool IsFrgmentCanBeUpgrade(ArtefactsInfos currentArtefactReinforce)
    {

        return currentArtefactReinforce.levelTierFragment != LevelTier.TIER_3;
    }
}
