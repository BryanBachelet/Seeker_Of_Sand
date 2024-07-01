using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractionInterface : MonoBehaviour
{
    public bool isOpen;
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
