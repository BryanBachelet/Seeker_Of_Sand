using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentUIRessources : MonoBehaviour
{
    public static FragmentUIRessources instance;

    public Sprite[] backgroundSprite;
    public Sprite[] raretySprite;
    public Sprite[] elementSprite;

    public string[] prefixElementNamesArray;

    public GameObject fragmentHolder_Prefab;

    public void Awake()
    {
        instance = this;
    }





}
