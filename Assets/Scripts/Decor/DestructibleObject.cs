using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
    public GameObject ObjDestroyedVersion;
    public bool test = false;
    private Rigidbody rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Update()
    {
        if (test == true)
        {
            test = false;
            Activedestruction();
        }
    }


    void Activedestruction()
    {

        GameObject instance =  Instantiate(ObjDestroyedVersion, transform.position, transform.rotation);
        Rigidbody rigidBody = instance.GetComponent<Rigidbody>();
        rigidBody.velocity = rigidbody.velocity;
        rigidBody.rotation = rigidbody.rotation;
        Destroy(gameObject);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Player")
        {
            Activedestruction();
        }
    }

}
