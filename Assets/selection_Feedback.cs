using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class selection_Feedback : MonoBehaviour
{
    public GameObject[] objectToOutlineOnSelection;

    public void ChangeLayerToSelection()
    {
        for(int i = 0; i < objectToOutlineOnSelection.Length; i++)
        {
            objectToOutlineOnSelection[i].layer = 17;
        }

        foreach (Transform child in objectToOutlineOnSelection[0].transform)
        {
            child.gameObject.layer = 17;
        }
    }

    public void ChangeLayerToDefault()
    {
        for (int i = 0; i < objectToOutlineOnSelection.Length; i++)
        {
            objectToOutlineOnSelection[i].layer = 0;
        }
        foreach (Transform child in objectToOutlineOnSelection[0].transform)
        {
            child.gameObject.layer = 0;
        }
    }
}
