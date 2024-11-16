using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GuerhoubaGames.GameEnum;

public class CristalInventory : MonoBehaviour
{
    public int[] cristalCount = new int[4];
    public bool hasEnoughCristalToSpawn = false;

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
        hasEnoughCristalToSpawn = false;
        for (int i = 0; i < cristalCount.Length; i++)
        {
            if (cristalCount[i] > 25)
            {
                hasEnoughCristalToSpawn = true;
            }
        }
    }
    
    public void RemoveCristalCount(int cristalType, int cristalNumberAdd)
    {
        cristalCount[cristalType-1] += cristalNumberAdd;
        m_cristalUI.UpdateUICristal(cristalCount[cristalType-1], cristalType-1);
        hasEnoughCristalToSpawn = false;
        for (int i = 0; i < cristalCount.Length; i++)
        {
            if(cristalCount[i] > 25)
            {
                hasEnoughCristalToSpawn = true;
            }
        }
    }

    public bool HasEnoughCristal(int value,GameElement element, string name)
    {
        int indexCristal = (int)element;
        Debug.Log("Element :" + element.ToString());
        if(element == GameElement.NONE)
        {
            Debug.LogError("This element is None " + name);
        }
        return cristalCount[indexCristal - 1] >= value;
    }

}
