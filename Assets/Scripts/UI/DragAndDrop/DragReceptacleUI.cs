using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using GuerhoubaGames.GameEnum;
using GuerhoubaGames.UI;

public struct ReceptableData
{
    public CharacterObjectType objectType;
    public int indexReceptacle;
    public int indexObject;
}


public class DragReceptacleUI : MonoBehaviour, IDropHandler
{
    public CharacterObjectType objectType;
    public Action<ReceptableData> OnDropEvent;
    public int indexReceptacle = -1;

    public void OnDrop(PointerEventData eventData)
    {
        ReceptableData receptableData = new ReceptableData();
        receptableData.indexObject = DragManager.instance.dragData.indexObj;
        receptableData.objectType = objectType;
        receptableData.indexReceptacle = indexReceptacle;
        OnDropEvent?.Invoke(receptableData);
    }

 
}
