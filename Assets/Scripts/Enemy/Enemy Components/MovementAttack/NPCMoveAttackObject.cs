using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuerhoubaGames.Enemies
{

    public class NPCMoveAttackObject : ScriptableObject
    {
        public NPCMoveAttData data;
        public Action EndMovement;
        public virtual void StartMvt(NPCMoveAttData moveData) { }
        public virtual void UpdateMvt() { }

    }
}
