using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Protecteur_MeshMask : MonoBehaviour
{
    public SkinnedMeshRenderer[] mesh;
    public int probability = 15;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < mesh.Length; i++)
        {
            int random = Random.Range(0, 100);
            if(random <= probability)
            {
                mesh[i].enabled = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
