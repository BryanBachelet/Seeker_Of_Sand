using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHolder : MonoBehaviour
{
    public List<AltarBehaviorComponent> altarBehaviorList = new List<AltarBehaviorComponent>();
    static public Vector3[] altarTransformTab;
    public Vector3[] altarTransformTabDisplay;

    [SerializeField] public GameObject[] DangerAddition;
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
        for (int i = 0; i < childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<AltarBehaviorComponent>())
            {
                AltarBehaviorComponent altarBehaviorComponent = transform.GetChild(i).GetComponent<AltarBehaviorComponent>();
                altarBehaviorList.Add(altarBehaviorComponent);
                newAltarTransform[i] = altarBehaviorComponent.transform.position;
                altarBehaviorComponent.eventHolder = this;
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

    public AltarBehaviorComponent GetNewAltar(AltarBehaviorComponent newAltar)
    {
        altarBehaviorList.Clear();
        altarBehaviorList[0] = newAltar;
        Vector3[] newAltarTransform = new Vector3[1];

        altarBehaviorList.Add(newAltar);
        newAltarTransform[0] = newAltar.transform.position;

        altarTransformTab = newAltarTransform;
        altarTransformTabDisplay = altarTransformTab;
        return altarBehaviorList[0];
    }
}
