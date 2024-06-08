using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class UIOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UpgradeUI upgradeUI_object;
    [SerializeField] private int m_indexUpgrade = 0;
    public bool isOver;
    public Action<int> OnEnter;


    private void Start()
    {
        //upgradeUI_object = transform.parent.transform.parent.GetComponent<UpgradeUI>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        if (OnEnter != null) OnEnter.Invoke(m_indexUpgrade);
        isOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOver = false;
    }
}
