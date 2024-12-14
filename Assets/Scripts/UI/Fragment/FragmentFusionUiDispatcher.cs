using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class FragmentFusionUiDispatcher : MonoBehaviour
{
    public bool generateNewGradient;
    [GradientUsage(true)]
    public Gradient hdrGradient;

    [ColorUsage(true, true)]
    public List<Color> ColorList;

    [ColorUsage(true, true)]
    public Color[] ColorElement = new Color[4];

    public List<Image> img_Fill = new List<Image>();
    public Animator[] animatorFill = new Animator[5];
    public List<Material> mat_imageFill = new List<Material>();

    public Texture[] spriteFill = new Texture[5];
    private bool[] isUsed = new bool[5];

    public Material matToCopy;
    // Start is called before the first frame update
    void Start()
    {

        for (int i = 0; i < img_Fill.Count; i++)
        {
            Image image = img_Fill[i].GetComponent<Image>();
            Material mat = new Material(matToCopy);
            mat.SetTexture("_Alpha", spriteFill[i]);
            img_Fill[i].material = mat;
            mat_imageFill.Add(mat);
            animatorFill[i] = img_Fill[i].GetComponent<Animator>();

        }
        ResetFill();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void GenerateGradient()
    {
        float keyCount = ColorList.Count;
        if (keyCount == 0 || keyCount > 8) return;
        float keyStep = 1 / keyCount;
        Debug.Log("Key count : " + keyCount + " KeyStep : " + keyStep);
        GradientColorKey[] colorKeys = new GradientColorKey[(int)keyCount];
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[(int)keyCount];
        for (int i = 0; i < keyCount; i++)
        {
            colorKeys[i] = new GradientColorKey(ColorList[i], keyStep * i);
            alphaKeys[i] = new GradientAlphaKey(1f, keyStep * i);

        }
        hdrGradient.SetKeys(colorKeys, alphaKeys);
    }

    public void ChangeFill(int indexFillToChange)
    {
        if (isUsed[indexFillToChange] == true) return;
        isUsed[indexFillToChange] = true;
        img_Fill[indexFillToChange].material.SetColor("_Color1", ColorElement[indexFillToChange]);
        img_Fill[indexFillToChange].material.SetFloat("_ColorNumber", 1);
        ColorList.Add(ColorElement[indexFillToChange]);
        animatorFill[indexFillToChange].SetBool("Activation", true);
        ChangeMultipleFill();
    }

    public void ResetFill()
    {
        ColorList.Clear();
        for(int i = 0; i < img_Fill.Count; i++)
        {
            img_Fill[i].material.SetColor("_Color1", Color.black);
            img_Fill[i].material.SetColor("_Color2", Color.black);
            img_Fill[i].material.SetColor("_Color3", Color.black);
            img_Fill[i].material.SetColor("_Color4", Color.black);
            img_Fill[i].material.SetFloat("_ColorNumber", 0);
            animatorFill[i].SetBool("Activation", false);
            isUsed[i] = false;
        }

    }
    public void ChangeMultipleFill()
    {
        int colorCount = ColorList.Count;
        if (colorCount < 2) { return; }
        GenerateGradient();
        if (colorCount == 2)
        {
            img_Fill[4].material.SetColor("_Color1", ColorList[0]);
            img_Fill[4].material.SetColor("_Color2", ColorList[1]);
        }
        else if (colorCount == 3)
        {
            img_Fill[4].material.SetColor("_Color1", ColorList[0]);
            img_Fill[4].material.SetColor("_Color2", ColorList[1]);
            img_Fill[4].material.SetColor("_Color3", ColorList[2]);
        }
        else if (colorCount == 4)
        {
            img_Fill[4].material.SetColor("_Color1", ColorList[0]);
            img_Fill[4].material.SetColor("_Color2", ColorList[1]);
            img_Fill[4].material.SetColor("_Color3", ColorList[2]);
            img_Fill[4].material.SetColor("_Color4", ColorList[3]);
        }


        img_Fill[4].material.SetFloat("_ColorNumber", colorCount);
        animatorFill[4].SetBool("Activation", true);
    }
}
