using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GuerhoubaGames.GameEnum;

public class CristalInventory : MonoBehaviour
{
    public int[] cristalCount = new int[4];
    public GameObject[] cristalDisplay = new GameObject[4]; //0 = Eau, 1 = Elec, 2 = Fire, 3 = Terre
    private Animator[] m_cristalAnimator = new Animator[4];
    private bool[] m_cristalState = new bool[4];

    [HideInInspector] public TMP_Text[] m_uiTextDisplay = new TMP_Text[4];

    static private float m_timeDisplaying = 1f;
    public float m_timeDisplayingSetup = 1f;

    // Start is called before the first frame update
    void Start()
    {
        m_timeDisplaying = m_timeDisplayingSetup;
        SetupText();
    }

    public void SetupText()
    {
        for(int i = 0; i < cristalDisplay.Length; i++)
        {
            m_uiTextDisplay[i] = cristalDisplay[i].GetComponentInChildren<TMP_Text>();
            m_cristalAnimator[i] = cristalDisplay[i].GetComponent<Animator>();
        }
    }

    public void AddCristalCount(int cristalType, int cristalNumberAdd)
    {
        cristalCount[cristalType] += cristalNumberAdd;
        m_uiTextDisplay[cristalType].text = "" + cristalCount[cristalType];
        StartCoroutine(DisplayUIFeedback(cristalType));
    }
    
    public void RemoveCristalCount(int cristalType, int cristalNumberAdd)
    {
        cristalCount[cristalType] += cristalNumberAdd;
    }

    public bool HasEnoughCristal(int value,GameElement element)
    {
        int indexCristal = (int)element;
        return cristalCount[indexCristal] >= value;
    }


    IEnumerator DisplayUIFeedback(int cristalType)
    {
        m_cristalState[cristalType] = true;
        m_cristalAnimator[cristalType].SetBool("Open", true);
        yield return new WaitForSeconds(m_timeDisplaying);
        m_cristalState[cristalType] = false;
        m_cristalAnimator[cristalType].SetBool("Open", false);
    }
}
