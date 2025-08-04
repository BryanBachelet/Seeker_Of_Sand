using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class TeleporterFeebackController : MonoBehaviour
{
    public MeshRenderer previewMeshPlane;
    public MeshRenderer previewMeshSpawnGate;
    public VisualEffect[] vfx_elecPortal;
    [ColorUsage(true, true)]
    public Color[] color_Elem_Portal;

    [Range(0, 6)]
    [HideInInspector] public int rewardToUse = 0;
    [Range(0, 6)]
    [HideInInspector] public int[] eventReward;

    public bool activeChange = false;

    private int idReward;

    private int colorToUse;

    public GameObject previewRewardEvent;
    public Sprite[] eventRewardImage;
    public SpriteRenderer[] spriteReward;

    void Update()
    {
        if (activeChange)
        {
            activeChange = false;
            ChangeColorID(idReward);
            SetColorVfx(colorToUse, idReward);
        }
    }

    public void ChangeRewardID(int ID, Material mat)
    {
        previewMeshPlane.material = new Material(mat);
        for (int i = 0; i < eventReward.Length; i++)
        {
            spriteReward[i].sprite = eventRewardImage[eventReward[i]];
        }
    }

    public void ChangeColorID(int ID)
    {
        if (!previewRewardEvent.activeSelf) { previewRewardEvent.SetActive(true); }
    }
    public void SetColorVfx(int color, int ID)
    {
        Color colorElec = color_Elem_Portal[color];
        for (int i = 0; i < vfx_elecPortal.Length; i++)
        {
            vfx_elecPortal[i].SetVector4("Color_I", colorElec);
            vfx_elecPortal[i].SetVector4("Color_II", colorElec);
            vfx_elecPortal[i].SetVector4("Color_III", colorElec);
        }
    }
    public void ChangeColorVFX(int color)
    {
        colorToUse = color;
    }
}
