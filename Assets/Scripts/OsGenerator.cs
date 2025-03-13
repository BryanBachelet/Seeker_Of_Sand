using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class OsGenerator : MonoBehaviour
{
    public GameObject osPrefab;
    public int numberOfOs;

    public bool activationGeneration;

    public float range;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(activationGeneration)
        {
            activationGeneration = false;
            GenerateOs(numberOfOs);
        }
    }

    public void GenerateOs(int os)
    {
        for (int i = 0; i < numberOfOs; i++)
        {
            float RandomX = Random.Range(-range / 2, range / 2);
            float RandomZ = Random.Range(-range / 2, range / 2);
            float randomYRot = Random.Range(0, 360);
            Vector3 pos = new Vector3(RandomX, 0, RandomZ);
            //Quaternion quaternion = quaternion.SetEulerRotation(new Vector3(22, 0, 55));
            GameObject newOs = Instantiate(osPrefab, transform.position + pos, transform.rotation, this.transform);
            newOs.transform.Rotate(new Vector3(22, randomYRot, 55));
        }

        
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
