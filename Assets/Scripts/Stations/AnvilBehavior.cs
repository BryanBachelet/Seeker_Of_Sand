using GuerhoubaGames.GameEnum;
using UnityEngine;
using GuerhoubaGames.UI;
using SeekerOfSand.Tools;

public class AnvilBehavior : InteractionInterface
{

    private AnvilUIView m_anvilUIComponent;
    private UI_Inventory m_uiInventory;
    private CristalInventory m_cristalInventory;
    private CharacterArtefact m_characterArtefact;
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
        m_characterArtefact = GameState.s_playerGo.GetComponent<CharacterArtefact>();
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
        int fragmentElement = GeneralTools.GetElementalArrayIndex(currentArtefactReinforce.gameElement);
        m_cristalInventory.RemoveCristalCount(fragmentElement, -value);
        // TODO : Update Anvil Upgrade price;


        return BuyResult.BUY;
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

    public bool CanFragmentBeMerge()
    {
        int countFragment = GetMergeFragmentCount();
        if (countFragment < 2) return false;


        // Check if fragments dont have the same elements and are interresting to merge 
        ArtefactsInfos[] fragmentInfosToMerge = GetMergeArtefactArray();
        GameElement stateOfMerge = GameElement.NONE;
        for (int i = 0; i < fragmentInfosToMerge.Length; i++)
        {
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
        int countFragment = GetMergeFragmentCount();
        if (countFragment < 2) return;

        ArtefactsInfos[] artefactsInfosToMerge = GetMergeArtefactArray();


        for (int i = 1; i < artefactsInfosToMerge.Length; i++)
        {
            artefactsInfosToMerge[0].MergeFragment(artefactsInfosToMerge[i]);

            // TODO : Verify artefact quantity to remove the best quantity
            m_characterArtefact.RemoveArtefact(artefactsInfosToMerge[i]);
        }

        m_uiInventory.ActualizeInventory();

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
