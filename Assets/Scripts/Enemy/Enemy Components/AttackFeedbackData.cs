using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.GameEnum;

namespace GuerhoubaGames.Enemies
{
    [System.Serializable]
    public struct AttackFeedbackData 
    {
        public enum FeedbackPosition
        {
            Target,
            Self,
            LastHit,
        }

        public FeedbackType feedbackType;
        public FeedbackPosition areaSpawnType;
        public bool isSpawn;
        public GameObject Vfx;
        public FMODUnity.EventReference sfx;
        public int sfxIndex;
        public int attackIndex;
        public AttackPhase attackPhase;
        public Vector3 offsetSpawnPosition;
        public bool isDelayed;
        public int attackTrigger;
    }
}
