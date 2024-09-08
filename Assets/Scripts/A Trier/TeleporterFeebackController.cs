using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class TeleporterFeebackController : MonoBehaviour
{
    [Range(0, 4)]
    public int elementToUse = 0; //0 --> Feu. 1 --> Elec. 2-->Eau. 3-->Terre. 4 --> Neutre
    public MeshRenderer previewMeshPlane;
    public VisualEffect vfx_lightPortal;
    public VisualEffect[] vfx_elecPortal;
    [ColorUsage(true, true)]
    public Color[] color_Elem_Portal;
    public Gradient[] colorZoneAutour = new Gradient[4];
    [ColorUsage(true, true)]
    public Color[] colorSelfLite = new Color[4];
    [ColorUsage(true, true)]
    public Color[] colorSymboleDecal = new Color[4];
    public Texture[] textureReward = new Texture[4];
    public Material[] materialReward = new Material[5];
    public MeshRenderer meshPortal;
    public Material mat_meshPortal;

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

    public Animator animatorPortal;
    private bool portalState = false;

    private int colorToUse;
    // Start is called before the first frame update
    void Awake()
    {
        if (socleMesh)
        {
            socleSpawn_mat = socleMesh.materials[0];
            dissonance_mat = socleMesh.materials[1];
            planeSymbole_mat = planeSymboleEffect.materials[0];
        }
        if(animatorPortal == null) { animatorPortal = transform.GetComponentInChildren<Animator>(); }

        //mat_meshPortal = meshPortal.material;
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
            if(!portalState) { animatorPortal.SetBool("Ouverture", true); portalState = true; }
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
            SetColorVfx(colorToUse, idReward);
            //}

        }

    }

    public void ChangeRewardID(int ID, Material mat)
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
        previewMeshPlane.material = mat;

    }

    public void ChangeColorID(int ID)
    {
        zoneAutourVFX.SetGradient("Color", colorZoneAutour[ID]);
        socleSpawn_mat.SetColor("_SelfLitColor", color_Elem_Portal[ID]);
        planeSymbole_mat.SetColor("_Color", color_Elem_Portal[ID]);
        dissonance_mat.SetFloat("_Visibility", 0);

        for (int i = 0; i < rLow_Holder.transform.childCount; i++)
        {
            rLow_Holder.transform.GetChild(i).GetComponent<MeshRenderer>().materials[1].SetFloat("_Visibility", 0);
        }
    }
    public void SetColorVfx(int color, int ID)
    {
        vfx_lightPortal.SetVector4("SmokeColor", color_Elem_Portal[color]);
        vfx_elecPortal[0].SetVector4("Color_I", color_Elem_Portal[color]);
        vfx_elecPortal[0].SetVector4("Color_II", color_Elem_Portal[color]);
        vfx_elecPortal[0].SetVector4("Color_III", color_Elem_Portal[color]);
        vfx_elecPortal[1].SetVector4("Color_I", color_Elem_Portal[color]);
        vfx_elecPortal[1].SetVector4("Color_II", color_Elem_Portal[color]);
        vfx_elecPortal[1].SetVector4("Color_III", color_Elem_Portal[color]);
        //
        if (mat_meshPortal == null)
        {
            mat_meshPortal = meshPortal.material;
        }
        if (meshPortal.material == null)
        {
            meshPortal.material = materialReward[ID];
        }
        meshPortal.material = materialReward[ID];
        meshPortal.material.SetColor("_SelfLitColor", color_Elem_Portal[color]);
    }
    public void ChangeColorVFX(int color)
    {
        colorToUse = color;
    }
}
