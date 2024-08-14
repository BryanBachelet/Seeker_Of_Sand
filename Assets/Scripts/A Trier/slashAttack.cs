using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slashAttack : MonoBehaviour
{
    public float speed = 50;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}
