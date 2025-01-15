using GuerhoubaGames.GameEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.UI;
using SeekerOfSand.Tools;
using UnityEngine.Rendering;
using System;

public struct AnvilInfos
{
    public ArtefactsInfos[] artefactsInfosToMerge;
    public int[] indexArtefact;
    public int[] indexReceptacle;

}

public class AnvilBehavior : InteractionInterface
{

    private AnvilUIView m_anvilUIComponent;
    private UI_Inventory m_uiInventory;
    private CristalInventory m_cristalInventory;
    private CharacterArtefact m_characterArtefact;
    [HideInInspector] public ArtefactsInfos currentArtefactReinforce;

    [HideInInspector] public ArtefactsInfos[] currentFragmentMergeArray;
    [HideInInspector] public int[] indexCurrentFragmentMergeArray;
    [HideInInspector] private int countOfFragmentSetup;
    [HideInInspector] private ArtefactsInfos m_cloneMergeFragment;

    public int costT1 = 100;
    public int costT2 = 150;
    public int costT3 = 200;

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
        m_characterArtefact = GameState.s_playerGo.GetComponent<CharacterArtefact>();
        m_anvilUIComponent.anvilBehavior = this;
        currentFragmentMergeArray = new ArtefactsInfos[4];
        indexCurrentFragmentMergeArray = new int[] { -1, -1, -1, -1 };
    }

    #region Upgrade Fragment Functions
    public void SetFragmentUpgrade()
    {
        currentArtefactReinforce.UpgradeTierFragment();
        m_characterArtefact.RemoveSpecificFragment(currentArtefactReinforce);
        m_characterArtefact.GenerateNewArtefactAround(currentArtefactReinforce);
        m_uiInventory.ActualizeInventory();
    }

    public BuyResult BuyUpgradeFragment()
    {
        if (isBuylessActive)
            return BuyResult.BUY;
        int indexPrice = (int)currentArtefactReinforce.levelTierFragment;
        int value = 0;
        switch (indexPrice)
        {
            case 0:
                value = costT1;
                break;
            case 1:
                value = costT2;
                break;
            case 2:
                value = costT3;
                break;
        }
        bool hasEnoughCristal = m_cristalInventory.HasEnoughCristal(value, currentArtefactReinforce.gameElement, currentArtefactReinforce.nameArtefact);
        if (!hasEnoughCristal) return BuyResult.NOT_ENOUGH_MONEY;
        int mabiteElement = GeneralTools.GetElementalArrayIndex(currentArtefactReinforce.gameElement);
        m_cristalInventory.RemoveCristalCount(mabiteElement, -value);
        // TODO : Update Anvil Upgrade price;


        return BuyResult.BUY;
    }

    public int BuyPrice()
    {
        int indexPrice = (int)currentArtefactReinforce.levelTierFragment;
        int value = 0;
        switch (indexPrice)
        {
            case 0:
                value = costT1;
                break;
            case 1:
                value = costT2;
                break;
            case 2:
                value = costT3;
                break;
        }
        return value;
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


    #endregion


    #region Merge Fragment Functions

    public BuyResult BuyMergeFragment()
    {
        if (isBuylessActive)
            return BuyResult.BUY;

        return BuyResult.BUY;
    }




    public void LockFragment(int indexFragment, int indexReceptacle)
    {
        if (GetMergeFragmentCount() == 0)
        {
            m_uiInventory.SetFragmentConditionnalUse(m_characterArtefact.artefactsList[indexFragment].idFamily);
        }

        currentFragmentMergeArray[indexReceptacle] = m_characterArtefact.artefactsList[indexFragment];
        indexCurrentFragmentMergeArray[indexReceptacle] = indexFragment;
        m_uiInventory.fragmentUIViews[indexFragment].ActiveModeRestreint(true);
        m_uiInventory.fragmentUIViews[indexFragment].GetComponent<DragObjectUI>().isLock = true;
    }

    public bool UnlockFragment(int indexFragment, int indexReceptacle)
    {
        bool isAlreadyEmpty = currentFragmentMergeArray[indexReceptacle] == null;

        if (isAlreadyEmpty) return false;

        currentFragmentMergeArray[indexReceptacle] = null;
        indexCurrentFragmentMergeArray[indexReceptacle] = -1;

        m_uiInventory.fragmentUIViews[indexFragment].ActiveModeRestreint(false);
        m_uiInventory.fragmentUIViews[indexFragment].GetComponent<DragObjectUI>().isLock = false;

        if (GetMergeFragmentCount() == 0)
        {
            m_uiInventory.RemoveFragmentConditionalUse();
        }

        return true;
             
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

    public AnvilInfos GetMergeArtefactArray()
    {

        AnvilInfos anvilInfos = new AnvilInfos();
        int countFragment = GetMergeFragmentCount();
        anvilInfos.artefactsInfosToMerge = new ArtefactsInfos[countFragment];
        anvilInfos.indexArtefact = new int[countFragment];
        anvilInfos.indexReceptacle = new int[countFragment];
        int count = 0;
        for (int i = 0; i < 4; i++)
        {
            if (currentFragmentMergeArray[i] != null)
            {
                anvilInfos.artefactsInfosToMerge[count] = currentFragmentMergeArray[i];
                anvilInfos.indexArtefact[count] = indexCurrentFragmentMergeArray[i];
                anvilInfos.indexReceptacle[count] = i;
                count++;
            }
        }

        return anvilInfos;
    }


    public bool CanFragmentBeMerge()
    {
        int countFragment = GetMergeFragmentCount();
        if (countFragment < 2) return false;


        // Check if fragments dont have the same elements and are interresting to merge 
        ArtefactsInfos[] fragmentInfosToMerge = GetMergeArtefactArray().artefactsInfosToMerge;
        GameElement stateOfMerge = GameElement.NONE;
        int idFamily = fragmentInfosToMerge[0].idFamily;
        for (int i = 0; i < fragmentInfosToMerge.Length; i++)
        {
            if (idFamily != fragmentInfosToMerge[i].idFamily)
            {
                return false;
            }

            if (GeneralTools.IsThisElementPresent(stateOfMerge, fragmentInfosToMerge[i].gameElement))
            {
                return false;
            }



            stateOfMerge = stateOfMerge | fragmentInfosToMerge[i].gameElement;
        }


        return true;

    }


    public ArtefactsInfos MergeFragmentClone()
    {

        if (!CanFragmentBeMerge()) return null;


        ArtefactsInfos[] artefactsInfosToMerge = GetMergeArtefactArray().artefactsInfosToMerge;

        m_cloneMergeFragment = artefactsInfosToMerge[0].Clone();
        for (int i = 1; i < artefactsInfosToMerge.Length; i++)
        {
            m_cloneMergeFragment.MergeFragment(artefactsInfosToMerge[i]);
        }

        string currentName = m_cloneMergeFragment.nameArtefact;
        string sufixName = currentName.Substring(currentName.IndexOf(" "), currentName.Length - currentName.IndexOf(" "));

        m_cloneMergeFragment.nameArtefact = FragmentUIRessources.instance.prefixElementNamesArray[(int)m_cloneMergeFragment.gameElement] + sufixName;
        return m_cloneMergeFragment;

    }

    public void MergeFragment()
    {
        int countFragment = GetMergeFragmentCount();
        if (countFragment < 2) return;

        AnvilInfos anvilInfo = GetMergeArtefactArray();

        for (int i = 0; i < anvilInfo.artefactsInfosToMerge.Length; i++)
        {
            if (i == 0) 
            {
                m_characterArtefact.RemoveSpecificFragment(anvilInfo.artefactsInfosToMerge[0]);
                continue;
            }  
            anvilInfo.artefactsInfosToMerge[0].MergeFragment(anvilInfo.artefactsInfosToMerge[i]);

            UnlockFragment(anvilInfo.indexArtefact[i], anvilInfo.indexReceptacle[i]);
            // TODO : Verify artefact quantity to remove the best quantity
            m_characterArtefact.RemoveArtefact(anvilInfo.artefactsInfosToMerge[i]);
        }


        string currentName = anvilInfo.artefactsInfosToMerge[0].nameArtefact;
        string sufixName = currentName.Substring(currentName.IndexOf(" "), currentName.Length - currentName.IndexOf(" "));

        anvilInfo.artefactsInfosToMerge[0].nameArtefact = FragmentUIRessources.instance.prefixElementNamesArray[(int)m_cloneMergeFragment.gameElement] + sufixName;
        m_uiInventory.m_characterArtefact.GenerateNewArtefactAround(m_cloneMergeFragment);
        m_uiInventory.ActualizeInventory();
        m_uiInventory.RemoveFragmentConditionalUse();

    }

    public void ClearAnvil()
    {
        AnvilInfos anvilInfo = GetMergeArtefactArray();
        for (int i = 0; i < anvilInfo.artefactsInfosToMerge.Length; i++)
        {

            UnlockFragment(anvilInfo.indexArtefact[i], anvilInfo.indexReceptacle[i]);
        }

        currentArtefactReinforce = null;
        currentFragmentMergeArray = new ArtefactsInfos[4];
        indexCurrentFragmentMergeArray = new int[] { -1, -1, -1, -1 };
        m_cloneMergeFragment = null;
    }

    #endregion

}
