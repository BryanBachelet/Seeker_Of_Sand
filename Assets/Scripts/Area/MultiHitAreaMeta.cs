using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GuerhoubaGames.Character;

namespace SpellSystem
{
    public struct MultiHitAreaData
    {
        public CharacterShoot characterShoot;
        public SpellProfil spellProfil;
        public int currentMaxHitCount;
    }


    public class MultiHitAreaMeta : MonoBehaviour
    {
        public MultiHitAreaData dotData;
        public Action<Vector3> OnDamage;
        public Action OnSpawn;

        public void ResetOnSpawn()
        {
            OnSpawn?.Invoke();
        }
    }
}
