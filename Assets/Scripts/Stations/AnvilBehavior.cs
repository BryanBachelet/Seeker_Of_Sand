using GuerhoubaGames.GameEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnvilBehavior : InteractionInterface
{


    private CristalInventory m_cristalInventory;


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
