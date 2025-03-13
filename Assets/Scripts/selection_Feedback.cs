using UnityEngine;
using GuerhoubaTools.Gameplay;
public class Selection_Feedback : MonoBehaviour
{
    public GameObject[] objectToOutlineOnSelection;

    public const int layerSelection = 17;

    public void ChangeLayerToSelection()
    {
        for (int i = 0; i < objectToOutlineOnSelection.Length; i++)
        {
            objectToOutlineOnSelection[i].layer = layerSelection;
            Tools.ChangeLayerGameObject(layerSelection,objectToOutlineOnSelection[i].gameObject);

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
