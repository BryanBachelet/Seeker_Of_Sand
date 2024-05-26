using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class TeleporterFeebackController : MonoBehaviour
{
    public Gradient[] colorZoneAutour = new Gradient[4];
    [ColorUsage(true, true)]
    public Color[] colorSelfLite = new Color[4];
    public Color[] colorSymboleDecal = new Color[4];
    public Texture[] textureReward = new Texture[4];

    [Range(0, 6)]
    public int rewardToUse = 0;
    public MeshRenderer socleMesh;
    public Material socleSpawn_mat;
    public Material dissonance_mat;

    public VisualEffect zoneAutourVFX;
    public MeshRenderer planeSymboleEffect;
    public Material planeSymbole_mat;

    public bool activeChange = false;
    // Start is called before the first frame update
    void Start()
    {
        if(socleMesh)
        {
            socleSpawn_mat = socleMesh.materials[0];
            dissonance_mat = socleMesh.materials[1];
            planeSymbole_mat = planeSymboleEffect.material;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(activeChange)
        {
            activeChange = false;
            ChangeRewardID(rewardToUse);
        }
        
    }

    public void ChangeRewardID(int ID)
    {
        socleSpawn_mat.SetTexture("_MaskSelfLit", textureReward[ID]);
        planeSymbole_mat.SetTexture("_Symbole", textureReward[ID]);
    }
}
