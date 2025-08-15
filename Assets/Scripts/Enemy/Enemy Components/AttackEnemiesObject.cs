using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuerhoubaGames.Enemies

{
    [CreateAssetMenu(fileName = "AttackData", menuName = "Enemmis/Attack/NpcAttackData", order = 5)]
    public class AttackEnemiesObject : ScriptableObject
    {
        public AttackNPCData data;
        [HideInInspector] public float tempCooldoown;
    }
}
