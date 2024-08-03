using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SpellSystem
{
    public struct DOTData
    {
        public Character.CharacterShoot characterShoot;
        public SpellProfil spellProfil;
        public int currentHitCount;
    }


    public class DOTMeta : MonoBehaviour
    {
        public DOTData dotData;
        public Action<Vector3> OnDamage;  
    }
}
