using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintInteractionManager : MonoBehaviour
{
    public GameObject[] autelDataHint;
    public GameObject[] pnjDataHint;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void activateAutelData(bool statut)
    {
        for(int i = 0; i < autelDataHint.Length; i++)
        {
            autelDataHint[i].SetActive(statut);
        }
    }

    public void activatePnjData(bool statut)
    {
        for (int i = 0; i < pnjDataHint.Length; i++)
        {
            pnjDataHint[i].SetActive(statut);
        }
    }
}
