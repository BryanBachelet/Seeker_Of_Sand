using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attaqueSister : MonoBehaviour
{
    public GameObject attackSlashPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateSlashAttack()
    {
        Instantiate(attackSlashPrefab, transform.position, transform.rotation);
    }
}
