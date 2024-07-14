using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.GameEnum;

namespace Enemies
{
    [System.Serializable]
    public struct AttackFeedbackData 
    {
        public enum FeedbackPosition
        {
            Target,
            Self,
        }

        public FeedbackType feedbackType;
        public FeedbackPosition areaSpawnType;
        public bool isSpawn;
        public GameObject Vfx;
        public FMODUnity.EventReference sfx;
        public int sfxIndex;
        public int attackIndex;
        public AttackPhase attackPhase;
    }
}
