using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class CristalInventory : MonoBehaviour
{
    public int[] cristalCount = new int[4];
    public GameObject[] cristalDisplay = new GameObject[4];

    private TMP_Text[] m_uiTextDisplay = new TMP_Text[4];
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetupText()
    {
        for(int i = 0; i < cristalDisplay.Length; i++)
        {
            m_uiTextDisplay[i] = cristalDisplay[i].GetComponentInChildren<TMP_Text>();
        }
    }

    public void AddCristalCount(int cristalType, int cristalNumberAdd)
    {
        cristalCount[cristalType] += cristalNumberAdd;
        m_uiTextDisplay[cristalType].text = "" + cristalCount[cristalType];
    }
    
    public void RemoveCristalCount(int cristalType, int cristalNumberAdd)
    {
        cristalCount[cristalType] += cristalNumberAdd;
    }
}
