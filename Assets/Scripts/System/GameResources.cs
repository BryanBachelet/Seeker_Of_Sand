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

        public void Awake()
        {
            instance = this;
        }
    }
}