using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOver : MonoBehaviour
{
    public UpgradeUI upgradeUI_object;
    [SerializeField] private int m_numeroUpgrade = 0;

    private void Start()
    {
        //upgradeUI_object = transform.parent.transform.parent.GetComponent<UpgradeUI>();
    }
    private void OnMouseOver()
    {
        Debug.Log("Overing : " + this.name);
        upgradeUI_object.UpdateCursorOver(m_numeroUpgrade);
        ComponentLinkerCrossScene.selectionOveringSprite.transform.position = this.transform.position;
    }

    private void OnMouseDown()
    {
        
    }

}
