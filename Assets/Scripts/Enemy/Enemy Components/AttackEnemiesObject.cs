using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies

{
    [CreateAssetMenu(fileName = "AttackData", menuName = "ScriptableObjects/AttackData", order = 5)]
    public class AttackEnemiesObject : ScriptableObject
    {
        public AttackNPCData data;
    }
}
