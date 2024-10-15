using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Enemies
{
    public struct CustomAttackData
    {
        public NpcAttackComponent npcAttacksComp;
        public NPCAttackFeedbackComponent npcAttackFeedback;
        public Transform targetTransform;
        public Transform ownTransform;
        public string name;
        public int attackIndex;
        public int damage;


    }


    public class NPCCustomAttack  : ScriptableObject
    {

        [HideInInspector]
        public CustomAttackData customAttackData;
        
        public virtual void ResetAttack() { }

        public virtual void ActivePrepPhase() { }
        public virtual void ActiveContactPhase() { }
        public virtual void ActiveRecoverPhase() { }

        public virtual bool UpdatePrepPhase() { return true; }
        public virtual bool UpdateContactPhase() { return true; }
        public virtual bool UpdateRecoverPhase() { return true; }

        public virtual void EndPrepPhase() { }
        public virtual void EndContactPhase() { }
        public virtual void EndRecoverPhase() { }


    }
}