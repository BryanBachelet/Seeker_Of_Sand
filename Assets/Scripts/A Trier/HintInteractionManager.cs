using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintInteractionManager : MonoBehaviour
{
    public GameObject[] autelDataHint;
    public GameObject[] pnjDataHint;
    public GameObject[] artefactDataHint;
   
    public void ActivateAutelData(bool statut)
    {

        return;
        for(int i = 0; i < autelDataHint.Length; i++)
        {
            autelDataHint[i].SetActive(statut);
        }
    }

    public void ActivatePnjData(bool statut)
    {
        for (int i = 0; i < pnjDataHint.Length; i++)
        {
            pnjDataHint[i].SetActive(statut);
        }
    }

    public void ActiveArtefactData(bool statut)
    {
        for (int i = 0; i < artefactDataHint.Length; i++)
        {
            artefactDataHint[i].SetActive(statut);
        }
    }
}
