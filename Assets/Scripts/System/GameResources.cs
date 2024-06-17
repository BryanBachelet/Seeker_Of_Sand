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

        public void Awake()
        {
            instance = this;
        }
    }
}