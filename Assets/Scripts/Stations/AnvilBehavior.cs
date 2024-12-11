using GuerhoubaGames.GameEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.UI;
using SeekerOfSand.Tools;
using UnityEditor.Timeline.Actions;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using UnityEditor.Rendering.HighDefinition;

public class AnvilBehavior : InteractionInterface
{

    private AnvilUIView m_anvilUIComponent;
    private UI_Inventory m_uiInventory;
    private CristalInventory m_cristalInventory;
    [HideInInspector] public ArtefactsInfos currentArtefactReinforce;

    [HideInInspector] public ArtefactsInfos[] currentFragmentMergeArray;
    [HideInInspector] private int countOfFragmentSetup;
    [HideInInspector] private ArtefactsInfos m_cloneMergeFragment;


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


    #region Upgrade Fragment Functions
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
        int mabiteElement = GeneralTools.GetElementalArrayIndex(currentArtefactReinforce.gameElement);
        m_cristalInventory.RemoveCristalCount(mabiteElement, -value);
        // TODO : Update Anvil Upgrade price;


        return BuyResult.BUY;
    }

    public bool IsFrgmentCanBeUpgrade(ArtefactsInfos currentArtefactReinforce)
    {

        return currentArtefactReinforce.levelTierFragment != LevelTier.TIER_3;
    }

    #endregion


    #region Merge Fragment Functions

    public void ApplyMergeFragment()
    {
        currentFragmentMergeArray[0].MergeFragment(currentFragmentMergeArray[1]);
        m_uiInventory.ActualizeInventory();
    }

    public int GetMergeFragmentCount()
    {
        int count = 4;
        for (int i = 0; i < 4; i++)
        {
            if (currentFragmentMergeArray[i] == null)
            {
                count--;
            }
        }

        return count;
    }
    public ArtefactsInfos[] GetMergeArtefactArray()
    {

        int countFragment = GetMergeFragmentCount();
        ArtefactsInfos[] artefactsInfosToMerge = new ArtefactsInfos[countFragment];
        int count = 0;
        for (int i = 0; i < 4; i++)
        {
            if (currentFragmentMergeArray[i] != null)
            {
                artefactsInfosToMerge[count] = currentFragmentMergeArray[i];
            }
        }

        return artefactsInfosToMerge;
    }

    public ArtefactsInfos MergeFragmentClone()
    {

        int countFragment = GetMergeFragmentCount();
        if (countFragment < 2) return null;


        ArtefactsInfos[] artefactsInfosToMerge = GetMergeArtefactArray();

        m_cloneMergeFragment = artefactsInfosToMerge[0].Clone();
        for (int i = 1; i < artefactsInfosToMerge.Length; i++)
        {
            m_cloneMergeFragment.MergeFragment(artefactsInfosToMerge[i]);
        }


        return m_cloneMergeFragment;

    }

    public void MergeFragment()
    {

    }

    #endregion

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
