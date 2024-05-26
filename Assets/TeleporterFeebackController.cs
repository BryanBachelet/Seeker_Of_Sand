using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterFeebackController : MonoBehaviour
{
    public Gradient[] colorZoneAutour = new Gradient[4];
    [ColorUsage(true, true)]
    public Color[] colorSelfLite = new Color[4];
    public Color[] colorSymboleDecal = new Color[4];
    public Texture[] textureReward = new Texture[4];

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
