using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    [CreateAssetMenu(fileName = "MoveAttackObject", menuName = "Enemmis/Move/MoveAttackObjet", order = 5)]
    public class NPCMoveAttackObject : ScriptableObject
    {
        public NPCMoveAttData data;
        public Action EndMovement;
        public virtual void StartMvt(NPCMoveAttData moveData) { }
        public virtual void UpdateMvt() { }

    }
}
