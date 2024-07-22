using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellSystem
{
    public struct AreaData
    {
        public Character.CharacterShoot characterShoot;
        public SpellProfil spellProfil;
        public Vector3 destination;
        public Vector3 direction;
    }

    public class AreaMeta : MonoBehaviour
    {
        public AreaData areaData;
    }

}