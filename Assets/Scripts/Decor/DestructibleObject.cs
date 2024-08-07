using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
    public GameObject ObjDestroyedVersion;
    public bool test = false;
    private Rigidbody rigidbody;
    private bool hasSpawn = false;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void Update()
    {
        if (test == true)
        {
            test = false;
            ActiveDestruction();
        }
    }


    void ActiveDestruction()
    {
        if (hasSpawn) return;
        GameObject instance = Instantiate(ObjDestroyedVersion, transform.position, transform.rotation,transform.parent);
        Rigidbody[] rigidBodies = instance.GetComponentsInChildren<Rigidbody>();
      
        for (int i = 0; i < rigidBodies.Length; i++)
        {
            rigidBodies[i].velocity = rigidbody.velocity;
            rigidBodies[i].rotation = rigidbody.rotation;
        }

     
        Destroy(gameObject);
        hasSpawn = true;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            ActiveDestruction();
        }

    }


    public void SetupDestruction(float power,Vector3 direction)
    {
        if (hasSpawn) return;
        GameObject instance = Instantiate(ObjDestroyedVersion, transform.position, transform.rotation, transform.parent);
        Rigidbody[] rigidBodies = instance.GetComponentsInChildren<Rigidbody>();

        for (int i = 0; i < rigidBodies.Length; i++)
        {
            rigidBodies[i].AddForce( direction.normalized *power,ForceMode.Impulse);
        }

        Destroy(gameObject);
        hasSpawn = true;
    }

}
