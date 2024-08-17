using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class TeleporterFeebackController : MonoBehaviour
{
    [Range(0, 4)]
    public int elementToUse = 0; //0 --> Feu. 1 --> Elec. 2-->Eau. 3-->Terre. 4 --> Neutre

    public Gradient[] colorZoneAutour = new Gradient[4];
    [ColorUsage(true, true)]
    public Color[] colorSelfLite = new Color[4];
    [ColorUsage(true, true)]
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

    public GameObject rLow_Holder;
    public bool activeChange = false;

    public bool random = false;
    private int idReward;
    // Start is called before the first frame update
    void Awake()
    {
        if (socleMesh)
        {
            socleSpawn_mat = socleMesh.materials[0];
            dissonance_mat = socleMesh.materials[1];
            planeSymbole_mat = planeSymboleEffect.materials[0];
        }

    }


    public void OnEnable()
    {
        if (socleMesh)
        {
            socleSpawn_mat = socleMesh.materials[0];
            dissonance_mat = socleMesh.materials[1];
            planeSymbole_mat = planeSymboleEffect.materials[0];
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (activeChange)
        {
            activeChange = false;
            zoneAutourVFX.enabled = true;
            //if (random) 
            //{ 
            //    int IDReward = Random.Range(0, 5);
            //    int IDElement = Random.Range(0, 3);
            //    ChangeRewardID(IDReward);
            //    ChangeColorID(IDElement);
            //}
            //else
            //{
            //    ChangeRewardID(rewardToUse);
            ChangeColorID(idReward);
            //}

        }

    }

    public void ChangeRewardID(int ID)
    {

        if (socleMesh && socleSpawn_mat == null)
        {
            socleSpawn_mat = socleMesh.materials[0];
            dissonance_mat = socleMesh.materials[1];
            planeSymbole_mat = planeSymboleEffect.materials[0];
        }
        socleSpawn_mat.SetTexture("_MaskSelfLit", textureReward[ID]);
        planeSymbole_mat.SetTexture("_Symbole", textureReward[ID]);
        idReward = ID;
    }

    public void ChangeColorID(int ID)
    {
        zoneAutourVFX.SetGradient("Color", colorZoneAutour[ID]);
        socleSpawn_mat.SetColor("_SelfLitColor", colorSelfLite[ID]);
        planeSymbole_mat.SetColor("_Color", colorSymboleDecal[ID]);
        dissonance_mat.SetFloat("_Visibility", 0);
        for (int i = 0; i < rLow_Holder.transform.childCount; i++)
        {
            rLow_Holder.transform.GetChild(i).GetComponent<MeshRenderer>().materials[1].SetFloat("_Visibility", 0);
        }
    }
}
