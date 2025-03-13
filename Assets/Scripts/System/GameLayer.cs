using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayer : MonoBehaviour
{

    public static GameLayer instance;

    public LayerMask groundLayerMask;
    public LayerMask propsGroundLayerMask;
    public LayerMask playerLayerMask;
    public LayerMask enemisLayerMask;


    void Awake()
    {
        instance = this;
    }

  
}
