using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DestroyRandomly : MonoBehaviour
{
    public bool activeDestroy = false;
    public int probability = 50;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(activeDestroy)
        {
            activeDestroy = false;
            DestroyActivation();
        }
    }

    public void DestroyActivation()
    {
        foreach (Transform holder in transform)
        {
            for(int i = 0; i < holder.childCount; i++)
            {
                int randomDestroy = Random.Range(0, 100);
                if(randomDestroy < probability)
                {
                    DestroyImmediate(holder.GetChild(i).gameObject);
                }
                //RandomlyDestroyChild(holder.GetChild(i).gameObject);
            }
        }
    }

    public void RandomlyDestroyChild(GameObject paveHolder)
    {

    }
}
