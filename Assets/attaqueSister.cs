using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attaqueSister : MonoBehaviour
{
    public GameObject attackSlashPrefab;
    public Material swordMaterial;
    public SkinnedMeshRenderer skinnedMesh_Renderer;
    // Start is called before the first frame update
    void Start()
    {
        swordMaterial = skinnedMesh_Renderer.materials[skinnedMesh_Renderer.materials.Length-1];
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateSlashAttack()
    {
        Instantiate(attackSlashPrefab, transform.position, transform.rotation);
    }

    public void SetGlow()
    {
        swordMaterial.SetFloat("_SelfLitPower", 0);
    }

    public void ModifyGlow(float progress)
    {
        if (progress < 0) return;

        swordMaterial.SetFloat("_SelfLitPower", (1-progress) * 50);
    }
    public void ExitGlow(int attaqueNumber)
    {
        swordMaterial.SetFloat("_SelfLitPower", 50);
        if(attaqueNumber == 3)
        {
            Instantiate(attackSlashPrefab, transform.position, transform.rotation);
        }
    }
}
