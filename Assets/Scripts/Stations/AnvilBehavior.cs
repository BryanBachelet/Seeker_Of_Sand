using GuerhoubaGames.GameEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.UI;

public class AnvilBehavior : InteractionInterface
{

    private AnvilUIView m_anvilUIComponent;
    private CristalInventory m_cristalInventory;



    public void Start()
    {
        
    }

    /// <summary>
    ///  Apply Fragment Drag and drop in UI;
    /// </summary>
    public void SetFragmentUpgrade()
    {

    }

    public BuyResult BuyUpgradeFragment()
    {
        return BuyResult.BUY;
    }



    public override void OnInteractionStart(GameObject player)
    {
        throw new System.NotImplementedException();
    }

    public override void OnInteractionEnd(GameObject player)
    {
        throw new System.NotImplementedException();
    }
}
