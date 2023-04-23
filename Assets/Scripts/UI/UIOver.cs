using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOver : MonoBehaviour
{
    public UpgradeUI upgradeUI_object;
    [SerializeField] private int m_numeroUpgrade = 0;

    private void Start()
    {
        upgradeUI_object = transform.parent.transform.parent.GetComponent<UpgradeUI>();
    }
    private void OnMouseOver()
    {
        upgradeUI_object.UpdateCursorOver(m_numeroUpgrade);
    }
}
