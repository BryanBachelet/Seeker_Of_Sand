using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainActivationManager : MonoBehaviour
{
    public GameObject[] decorationAdditionnel;
      
    public void ActiveDecorationAdditionnel()
    {
        if (decorationAdditionnel.Length < 1) { return; }
        else
        {
            for (int i = 0; i < decorationAdditionnel.Length; i++)
            {
                decorationAdditionnel[i].SetActive(true);
            }
        }
    }
}
