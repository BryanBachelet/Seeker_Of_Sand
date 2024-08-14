using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simulate_Shot : MonoBehaviour
{
    public GameObject currentShotVFX;
    public Vector3 offset;
    public bool play = false;

    public float setScale = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(play)
        {
            play = false;
            GameObject lastshot = Instantiate(currentShotVFX, transform.position + offset, transform.rotation);
            Vector3 scale = lastshot.transform.localScale;
            lastshot.transform.localScale = scale * setScale;
        }
    }
}
