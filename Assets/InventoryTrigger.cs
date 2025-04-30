using GuerhoubaGames.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
public class InventoryTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public Action OnEnter;
    public bool IsActive = true;

    public CristalUI CristalUI;
    private bool currentState = false;
  

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsActive) return;

        if (OnEnter != null) OnEnter.Invoke();
        CristalUI.OpenInventoryOver();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!IsActive) return;

        CristalUI.CloseInventoryOver();
    }

    public void ChangeInventoryState()
    {
        if(!currentState)
        {
            CristalUI.OpenInventory();
            currentState = true;
        }
        else
        {
            CristalUI.CloseInventory();
            currentState = false;
        }
    }
}
