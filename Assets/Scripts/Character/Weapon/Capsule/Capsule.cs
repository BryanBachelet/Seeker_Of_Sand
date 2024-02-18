using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SpellSystem
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
        public Element element;
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

    public enum Element
    {
        WATER,
        AIR,
        FIRE,
        EARTH
    }
    [Serializable]
    public class Capsule 
    {
        public string name;
        [TextArea]
        public string description;
        public CapsuleType type;
        public Element elementType;
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
            profil = info.stats;
            sprite = info.sprite;
            vfx = info.vfx;
            elementType = info.element;


        }
       
        public GameObject projectile;
        public GameObject vfx;
        public CapsuleProfil profil;
        public Element element;
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