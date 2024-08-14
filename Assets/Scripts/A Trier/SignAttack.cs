using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignAttack : MonoBehaviour
{
    public float speedMovement = 50f;
    public Vector3 positionToGo;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, positionToGo, speedMovement * Time.deltaTime);
    }
}
