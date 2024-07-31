using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellSystem
{
    public struct ProjectileMetaData
    {
        public bool isFromSpell;
        public SpellProfil spellProfil;

        public float lifetime;
        public float range;
        public Vector3 direction;
        public float angle;
        public Transform target;
    }


    public class ProjectileMeta : MonoBehaviour
    {
        public ProjectileMetaData metaData;

        public System.Action<Vector3> onDamage;
    }
}