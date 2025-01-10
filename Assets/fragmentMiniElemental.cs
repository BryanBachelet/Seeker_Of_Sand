using GuerhoubaGames.GameEnum;
using SeekerOfSand.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fragmentMiniElemental : MonoBehaviour
{
    [Header("Element")]
    public GameElement element;
    public GameElement[] elements;

    [Header("Material")]
    public Material materialGlassRef;
    private Material materialGlassUse;

    public Material materialNoiseRef;
    private Material materialNoiseUse;

    [ColorUsage(true, true)]
    public List<Color> glassColorsElement;
    [ColorUsage(true, true)]
    public List<Color> noiseColorsElement;

    [Header("Glass tint")]
    [ColorUsage(true, true)][SerializeField] Color glassColor_Water;
    [ColorUsage(true, true)][SerializeField] Color glassColor_Aer;
    [ColorUsage(true, true)][SerializeField] Color glassColor_Fire;
    [ColorUsage(true, true)][SerializeField] Color glassColor_Earth;

    [Header("Noise tint")]
    [ColorUsage(true, true)][SerializeField] Color noiseColor_Water;
    [ColorUsage(true, true)][SerializeField] Color noiseColor_Aer;
    [ColorUsage(true, true)][SerializeField] Color noiseColor_Fire;
    [ColorUsage(true, true)][SerializeField] Color noiseColor_Earth;

    public SkinnedMeshRenderer[] glassCristal_Mesh;
    public MeshRenderer[] noise_Mesh;

    private float tempsEcouleSwapColor = 0;
    private int currentColorUse = 0;
    // Start is called before the first frame update
    void Start()
    {
        //SelectElement(element);

    }

    // Update is called once per frame
    void Update()
    {
        if (elements.Length <= 1) return;
        else
        {
            tempsEcouleSwapColor += Time.deltaTime;
            if(tempsEcouleSwapColor > 1)
            {
                tempsEcouleSwapColor = 0;
                currentColorUse++;
                if(currentColorUse > elements.Length-1)
                {
                    currentColorUse = 0;
                }
            }
            Color glassColor;
            Color noiseColor;
            if (currentColorUse < elements.Length-1)
            {
                glassColor = Color.Lerp(glassColorsElement[currentColorUse], glassColorsElement[currentColorUse + 1], tempsEcouleSwapColor);
                noiseColor = Color.Lerp(noiseColorsElement[currentColorUse], noiseColorsElement[currentColorUse + 1], tempsEcouleSwapColor);
            }
            else
            {
                glassColor = Color.Lerp(glassColorsElement[currentColorUse], glassColorsElement[0], tempsEcouleSwapColor);
                noiseColor = Color.Lerp(noiseColorsElement[currentColorUse], noiseColorsElement[0], tempsEcouleSwapColor);
            }
            materialGlassUse.SetColor("_Color", glassColor);
            materialNoiseUse.SetColor("_Color", noiseColor);
        }
        Debug.Log("Elements are setup");
    }

    public void SelectElement(GameElement elementToUse)
    {
        GameElement[] baseElements = GeneralTools.GetBaseElementsArray(elementToUse);
        elements = baseElements;
        glassColorsElement.Clear();
        for (int i = 0; i < elements.Length; i++)
        {
            if(elements[i] == GameElement.WATER) { glassColorsElement.Add(glassColor_Water); noiseColorsElement.Add(noiseColor_Water); }
            if(elements[i] == GameElement.AIR) { glassColorsElement.Add(glassColor_Aer); noiseColorsElement.Add(noiseColor_Aer); }
            if (elements[i] == GameElement.FIRE) { glassColorsElement.Add(glassColor_Fire); noiseColorsElement.Add(noiseColor_Fire); }
            if (elements[i] == GameElement.EARTH) { glassColorsElement.Add(glassColor_Earth); noiseColorsElement.Add(noiseColor_Earth); }
        }
        materialGlassUse = new Material(materialGlassRef);
        materialGlassUse.SetColor("_Color", glassColorsElement[0]);
        materialNoiseUse = new Material(materialNoiseRef);
        materialNoiseUse.SetColor("_Color", noiseColorsElement[0]);
        for (int i = 0; i < glassCristal_Mesh.Length; i++)
        {
            glassCristal_Mesh[i].material = materialGlassUse;
        }
        for (int j = 0; j < noise_Mesh.Length; j++)
        {
            noise_Mesh[j].material = materialNoiseUse;
        }
    }
}
