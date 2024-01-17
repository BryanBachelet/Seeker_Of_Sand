using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHolder : MonoBehaviour
{
    public List<AltarBehaviorComponent> altarBehaviorList = new List<AltarBehaviorComponent>();
    static public Vector3[] altarTransformTab;
    public Vector3[] altarTransformTabDisplay;
    // Start is called before the first frame update
    void Start()
    {
        GetAltar();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public List<AltarBehaviorComponent> GetAltar()
    {
        altarBehaviorList.Clear();
        int childCount = transform.childCount;
        Vector3[] newAltarTransform = new Vector3[childCount];
        for(int i = 0; i < childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<AltarBehaviorComponent>())
            {
                altarBehaviorList.Add(transform.GetChild(i).GetComponent<AltarBehaviorComponent>());
                newAltarTransform[i] = transform.GetChild(i).transform.position;
            }
        }
        altarTransformTab = newAltarTransform;
        altarTransformTabDisplay = altarTransformTab;
        return altarBehaviorList;
    }

    static public Vector4 newSpot(int currentSpot)
    {
        int newSpotNumber = currentSpot;
        while (newSpotNumber == currentSpot)
        {
            newSpotNumber = Random.Range(0, altarTransformTab.Length);
        }
        Vector4 nextAltarPosition = altarTransformTab[newSpotNumber];
        nextAltarPosition.w = newSpotNumber;
        return nextAltarPosition;
    }
}
