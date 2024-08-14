using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GuerhoubaGames.GameEnum;

public class CristalInventory : MonoBehaviour
{
    public int[] cristalCount = new int[4];

    private GuerhoubaGames.UI.CristalUI m_cristalUI;

    // Start is called before the first frame update
    void Start()
    {

        if (GameState.m_uiManager)
        {
            UIDispatcher m_uiDispatcher = GameState.m_uiManager.GetComponent<UIDispatcher>();
            if (m_uiDispatcher == null) return;
            m_cristalUI = m_uiDispatcher.cristalUI;
        }
    }

    public void AddCristalCount(int cristalType, int cristalNumberAdd)
    {
        cristalCount[cristalType-1] += cristalNumberAdd;
        m_cristalUI.UpdateUICristal(cristalCount[cristalType - 1], cristalType-1);
    }
    
    public void RemoveCristalCount(int cristalType, int cristalNumberAdd)
    {
        cristalCount[cristalType-1] += cristalNumberAdd;
        m_cristalUI.UpdateUICristal(cristalCount[cristalType-1], cristalType);
    }

    public bool HasEnoughCristal(int value,GameElement element)
    {
        int indexCristal = (int)element;
        return cristalCount[indexCristal - 1] >= value;
    }

}
