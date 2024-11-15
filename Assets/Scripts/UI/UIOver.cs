using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class UIOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private int m_indexUpgrade = 0;
    public bool isOver;
    public Action<int> OnEnter;
    public GameObject gameObjectToSelect;



    public void Update()
    {
        if (!GameState.instance.IsGamepad()) return;

        bool isSelected = EventSystem.current.currentSelectedGameObject == gameObjectToSelect;

        if (isSelected)
        {
            if (OnEnter != null) OnEnter.Invoke(m_indexUpgrade);
            isOver = true;
        }
        else
        {
            isOver = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        if (OnEnter != null) OnEnter.Invoke(m_indexUpgrade);
        GlobalSoundManager.PlayOneShot(5, Vector3.zero);
        isOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOver = false;
    }
}
