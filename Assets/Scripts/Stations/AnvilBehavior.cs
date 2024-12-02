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
        m_anvilUIComponent.anvilBehavior = this;
    }

    public void SetFragmentUpgrade()
    {
        currentArtefactReinforce.UpgradeTierFragment();
    }

    public BuyResult BuyUpgradeFragment()
    {
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
}
