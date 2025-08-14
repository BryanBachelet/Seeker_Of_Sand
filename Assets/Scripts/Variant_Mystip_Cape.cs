using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Variant_Mystip_Cape : MonoBehaviour
{
    public Material cape_Material;
    public SkinnedMeshRenderer skinMeshRender;
    // Start is called before the first frame update
    void Start()
    {
        cape_Material = skinMeshRender.material;
        float rnd = Random.Range(-5f, 5f);
        cape_Material.SetTextureScale("_SecondaryCutout", new Vector3(rnd, 1));

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
