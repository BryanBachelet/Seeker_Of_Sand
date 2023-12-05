using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHolder : MonoBehaviour
{
    public List<AltarBehaviorComponent> altarBehaviorList = new List<AltarBehaviorComponent>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<AltarBehaviorComponent> GetAltar()
    {
        altarBehaviorList.Clear();
        for(int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<AltarBehaviorComponent>())
            {
                altarBehaviorList.Add(transform.GetChild(i).GetComponent<AltarBehaviorComponent>());
            }
        }
        return altarBehaviorList;
    }


}
