using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractionInterface : MonoBehaviour
{
    [Header("Interaction Variables")]
    public bool isOpen;
    public bool isInteractable = true;
    public bool hasClosePhase;
    public string verbeInteraction = "Interact";
    public bool hasAdditionalDescription;
    public string additionalDescription = "Activate the object";
    public int cost = 0;
    public int cristalID = 4; //0 --> Water, 1 --> Air, 2 --> Feu, 3 --> Terre, 4 --> Dissonance

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
