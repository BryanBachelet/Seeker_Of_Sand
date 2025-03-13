using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.GameEnum;
using System;
namespace SpellSystem
{
    public struct AreaData
    {
        public Character.CharacterShoot characterShoot;
        public SpellProfil spellProfil;
        public Vector3 destination;
        public Vector3 direction;
        public CharacterObjectType objectType;
    }

    public class AreaMeta : MonoBehaviour
    {
        public AreaData areaData;

        public Action OnSpawn;
        public Action OnRelaunch;

        public void ResetOnSpawn()
        {
            OnSpawn?.Invoke();
        }

        public void RelaunchArea()
        {
            OnRelaunch?.Invoke();
        }
    }

}