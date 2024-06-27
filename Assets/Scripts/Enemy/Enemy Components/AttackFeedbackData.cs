using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.GameEnum;

namespace Enemies
{
    public struct AttackFeedbackData 
    {
        public GameObject Vfx;
        public FMODUnity.EventReference sfx;
        public int attackIndex;
        public AttackPhase attackPhase;
    }
}
