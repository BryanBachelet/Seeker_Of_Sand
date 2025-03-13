using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractionInterface : MonoBehaviour
{
    public bool isOpen;
    public bool hasClosePhase;

    public void CallOpenInteraction(GameObject player)
    {
        if(!isOpen)
        {
            OnInteractionStart(player);
            isOpen = true;
        }
    }

    public void CallCloseInteraction(GameObject player)
    {
        if (isOpen)
        {
            OnInteractionEnd(player);
            isOpen = false;
        }
    }
    public void CallInteraction(GameObject player)
    {
        if (isOpen)
        {
            OnInteractionEnd(player);
            isOpen = false;
        }
        else
        {
            OnInteractionStart(player);
            isOpen = true;
        }
    }
    public abstract void OnInteractionStart(GameObject player);

    public abstract void OnInteractionEnd(GameObject player);
}
