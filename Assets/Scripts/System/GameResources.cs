using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuerhoubaGames.Resources
{
    public class GameResources : MonoBehaviour
    {
        public static GameResources instance;
        
        [Tooltip("This array need to be align withe enum GameElement")]
        public Sprite[] cristalIconArray;

        [Tooltip("This array need to be align withe enum GameElement")]
        public Sprite[] fragmentIconArray;

        [Tooltip("This array contain T0 to T3 rarity")]
        public Material[] raritySprite;

        [Tooltip("This array contain Gradient that are used for decal (0 = neutre --> 1 Eau, 2 Air, 3 Feu, 4 Terre")]
        [GradientUsage(true)]
        public Gradient[] gradientDecalElement;

        [Tooltip("This array contain mini fragment prefab from T0 to T3")]
        public GameObject[] artefactAround_Prefab;

        [Tooltip("This array contain texture of different objectif")]
        public Sprite[] spriteObjectif_tab;

        [Tooltip("This array contain the text of objectif")]
        public string[] text_Objectif;

        [Tooltip("This array contain texture of different objectif")]
        public Sprite[] optional_spriteObjectif_tab;

        [Tooltip("This array contain the text of objectif")]
        public string[] optional_text_Objectif;

        [Tooltip("This array contain texture of different reward (Upgrade, heal, spell, fragment)")]
        public Sprite[] spriteReward_tab;

        [Tooltip("This array contain Prefab for physical Artefact Object")]
        public GameObject[] artefactPrefab = new GameObject[5]; //0 --> Neutre, 1 --> Eau, 2 --> Elec, 3 - Feu, 4 - Terre


        [Tooltip("This array contain Prefab for physical Artefact Object")]
        public GameObject[] costPrefab = new GameObject[5]; //0 --> Eau, 1 --> Elec, 2 --> Feu, 3 - Terre, 4 - Dissonance

        [Tooltip("This array contain Prefab for physical Artefact Object")]
        public Texture[] textureGradient_Ornement = new Texture[5]; //0 --> Eau, 1 --> Elec, 2 --> Feu, 3 - Terre, 4 - Dissonance
        public void Awake()
        {
            instance = this;
        }
    }
}