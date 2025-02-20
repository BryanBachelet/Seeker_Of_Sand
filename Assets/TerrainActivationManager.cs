using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainActivationManager : MonoBehaviour
{
    public GameObject portalGate;

    public AltarBehaviorComponent[] altarBehavior_tab;

    public GameObject lootHolder;

    public GameObject[] decorationAdditionnel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActiveEvent()
    {
        if (altarBehavior_tab.Length < 1) { return; }
        else
        {
            for (int i = 0; i < altarBehavior_tab.Length; i++)
            {
                altarBehavior_tab[i].enabled = true;
                altarBehavior_tab[i].LaunchInit();
            }
        }
    }

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
