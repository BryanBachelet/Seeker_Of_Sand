using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
    public GameObject ObjDestroyedVersion;
    public bool test = false;
    private Rigidbody rigidbody;
    private bool hasSpawn = false;
    public LayerMask playerLayer;

    public bool sd_Destruction = true;
    public int indexSound = 51;
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
        if (sd_Destruction) { GlobalSoundManager.PlayOneShot(51, transform.position); }
        Vector3 parentScale = transform.localScale;
        GameObject instance = Instantiate(ObjDestroyedVersion, transform.position, transform.rotation,transform.parent);
        instance.transform.localScale = parentScale;
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
        if (sd_Destruction) { GlobalSoundManager.PlayOneShot(51, transform.position); }
        GameObject instance = Instantiate(ObjDestroyedVersion, transform.position, transform.rotation, transform.parent);
        Rigidbody[] rigidBodies = instance.GetComponentsInChildren<Rigidbody>();
        Collider[] colliders = instance.GetComponentsInChildren<Collider>();   
        for (int i = 0; i < rigidBodies.Length; i++)
        {
            rigidBodies[i].AddForce( direction.normalized * power * 1000,ForceMode.Impulse);
            rigidBodies[i].mass = 10;
            rigidBodies[i].excludeLayers = playerLayer;
            colliders[i].excludeLayers = playerLayer;
          //  Debug.Log(power + "--> Is power of impulse. Direction is [" + direction + "]");
        }

        Destroy(gameObject);
        hasSpawn = true;
    }

}
