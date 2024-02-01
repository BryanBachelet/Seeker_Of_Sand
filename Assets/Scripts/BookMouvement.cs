using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookMouvement : MonoBehaviour
{
    public Transform bookPositionHolder;
    public float offset;

    public float distanceWithHolder;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        distanceWithHolder = Vector3.Distance(transform.position, bookPositionHolder.position);
        
        transform.position = bookPositionHolder.position;
    }
}
