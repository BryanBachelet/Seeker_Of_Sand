using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayer : MonoBehaviour
{

    public static GameLayer instance;

    public LayerMask groundLayerMask;
    public LayerMask propsGroundLayerMask;
    public LayerMask decoLayerMask;
    public LayerMask playerLayerMask;
    public LayerMask enemisLayerMask;
    public LayerMask traderLayerMask;
    public LayerMask interactibleLayerMask;
    public LayerMask artefactLayerMask;
    void Awake()
    {
        instance = this;
    }

  
}
