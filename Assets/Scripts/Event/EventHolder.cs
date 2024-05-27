using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHolder : MonoBehaviour
{
    static EventHolder instance;

    public List<AltarBehaviorComponent> altarBehaviorList = new List<AltarBehaviorComponent>();
    static public List<Vector3> altarTransformTab = new List<Vector3>();
    public List<Vector3> altarTransformTabDisplay = new List<Vector3>();

    [SerializeField] public GameObject[] DangerAddition;
    // Start is called before the first frame update

    private void Awake()
    {
        instance = this;

    }
    void Start()
    {
        GetAltar();
    }

    /// <summary>
    ///  Singleton function
    /// </summary>
    /// <returns></returns>
    public static EventHolder GetInstance()
    {
        return instance;
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
                altarTransformTab.Add(altarBehaviorComponent.transform.position);
                altarBehaviorComponent.eventHolder = this;
            }
        }
   
        altarTransformTabDisplay = altarTransformTab;
        return altarBehaviorList;
    }

    static public Vector4 NewSpot(int currentSpot)
    {
        int newSpotNumber = currentSpot;
        while (newSpotNumber == currentSpot)
        {
            newSpotNumber = Random.Range(0, altarTransformTab.Count);
        }
        Vector4 nextAltarPosition = altarTransformTab[newSpotNumber];
        nextAltarPosition.w = newSpotNumber;
        return nextAltarPosition;
    }

    public void GetNewAltar(AltarBehaviorComponent newAltar)
    {

        altarBehaviorList.Add(newAltar);
        altarTransformTab.Add(newAltar.transform.position);
        altarTransformTabDisplay = altarTransformTab;

    }
}
