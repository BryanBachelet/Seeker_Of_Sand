using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
       
    }
}
