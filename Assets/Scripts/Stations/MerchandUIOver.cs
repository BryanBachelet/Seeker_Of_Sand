using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using GuerhoubaGames.GameEnum;

public class MerchandUIOver : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public int index;
    public CharacterObjectType type;

    public Action<int, CharacterObjectType> OnEnter;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (OnEnter != null) OnEnter.Invoke(index, type);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (OnEnter != null) OnEnter.Invoke(index, type);
    }

    // Start is called before the first frame update
   
}
