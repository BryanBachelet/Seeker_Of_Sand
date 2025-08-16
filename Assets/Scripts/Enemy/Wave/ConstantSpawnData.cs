using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GuerhoubaGames.Enemies
{
    [CreateAssetMenu(fileName = "Constant_Spawn_Data", menuName = "Ennemies/ConstantSpawnData")]
    public class ConstantSpawnData : ScriptableObject
    {
        [Tooltip("The value is count in minute")]
        public float spawnRate = 0;
        public int groupMinSize=0;
        public int groupMaxSize=0;
        public int maxEnemySimultaneous = 0;
    }
}
