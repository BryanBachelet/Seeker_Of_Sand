using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace CapsuleSystem
{
    [Serializable]
    public struct CapsuleAttackInfo
    {
        public string name;
        [TextArea]
        public string description;
        public GameObject projectile;
        public GameObject vfx;
        public CapsuleProfil stats;
        public Sprite sprite;

    }

    [Serializable]
    public struct CapsuleBuffInfo
    {
        public string name;
        [TextArea]
        public string description;
        public CharacterData stats;
        public float duration;
        public Sprite sprite;
        public GameObject vfx;

    }

    [Serializable]
    public enum CapsuleType
    {
       ATTACK,
       DOUBLE,
       BUFF
    }

    [Serializable]
    public class Capsule 
    {
        public string name;
        [TextArea]
        public string description;
        public CapsuleType type;
        public Sprite sprite;
    }
    [Serializable]
    public class CapsuleAttack: Capsule
    {
        public CapsuleAttack(CapsuleAttackInfo info)
        {
            name = info.name;
            description = info.description;
            projectile = info.projectile;
            stats = info.stats;
            sprite = info.sprite;
            vfx = info.vfx;
            
        }
       
        public GameObject projectile;
        public GameObject vfx;
        public CapsuleProfil stats;

    }
    [Serializable]
    public class CapsuleBuff : Capsule
    {
        public CapsuleBuff(CapsuleBuffInfo info)
        {
            name = info.name;
            description = info.description;
            profil = info.stats;
            type = CapsuleType.BUFF;
            duration = info.duration;
            sprite = info.sprite;
            vfx = info.vfx;

        }
        
        public CharacterData profil;
        public float duration;
        public GameObject vfx;

    }
}