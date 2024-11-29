using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using GuerhoubaGames.GameEnum;
using GuerhoubaGames.UI;

public class DragReceptacleUI : MonoBehaviour, IDropHandler
{
    public CharacterObjectType objectType;
    public Action<int, CharacterObjectType> OnDropEvent;

    public void OnDrop(PointerEventData eventData)
    {
        OnDropEvent?.Invoke(DragManager.instance.dragData.indexObj, objectType);
    }

 
}
