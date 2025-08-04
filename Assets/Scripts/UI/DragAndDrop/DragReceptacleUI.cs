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



public class DragReceptacleUI : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    public CharacterObjectType objectType;
    public Action<ReceptableData> OnDropEvent;
    public Action<ReceptableData> OnCtrlClick;
    public int indexReceptacle = -1;
    private int currentIndexObject;

    public void OnDrop(PointerEventData eventData)
    {
        ReceptableData receptableData = new ReceptableData();
        receptableData.indexObject = DragManager.instance.dragData.indexObj;
        currentIndexObject = DragManager.instance.dragData.indexObj;
        receptableData.objectType = objectType;
        receptableData.indexReceptacle = indexReceptacle;
        OnDropEvent?.Invoke(receptableData);
    }


    public void OnPointerClick(PointerEventData eventData)
    {

        if (eventData.button == PointerEventData.InputButton.Left && Input.GetKey(KeyCode.LeftShift))
        {
            ReceptableData receptableData = new ReceptableData();
            receptableData.indexObject = currentIndexObject;
            receptableData.objectType = objectType;
            receptableData.indexReceptacle = indexReceptacle;
            OnCtrlClick?.Invoke(receptableData);
        }
    }
}
