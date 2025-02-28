using GuerhoubaGames.GameEnum;
using SeekerOfSand.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Rendering.FilterWindow;

public class EventHolder : MonoBehaviour
{
    static EventHolder instance;

    public List<AltarBehaviorComponent> altarBehaviorList = new List<AltarBehaviorComponent>();
    static public List<Vector3> altarTransformTab = new List<Vector3>();
    public List<Vector3> altarTransformTabDisplay = new List<Vector3>();

    [SerializeField] public GameObject[] DangerAddition;

    public AltarAttackData[] altarAttackDataArray;

    [SerializeField] private GameObject[] capacityGameObject = new GameObject[5];
    private GameObject instantiatedArea;
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

    public AltarAttackData GetAltarAttackData(GameElement element, int difficulty)
    {
        for (int i = 0; i < altarAttackDataArray.Length; i++)
        {
            if (altarAttackDataArray[i].element == element && altarAttackDataArray[i].difficulty == difficulty )
            {
                return altarAttackDataArray[i];
            }
        }
        Debug.LogError("An altar is missing for the case of " + element.ToString() + " and with the difficulty level of " + difficulty.ToString());
        return null;
    }

    public void SpawnAreaVFX(GameElement element, Vector3 position)
    {
        instantiatedArea = Instantiate(capacityGameObject[GeneralTools.GetElementalArrayIndex(element, true)], position, Quaternion.identity);
    }

    public void ActiveEndEvent()
    {
        Destroy(instantiatedArea);
    }
}
