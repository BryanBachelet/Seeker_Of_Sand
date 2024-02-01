using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lightMovement : MonoBehaviour
{
    public Vector3 movementDirection;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + movementDirection * speed * Time.deltaTime;
    }
}
