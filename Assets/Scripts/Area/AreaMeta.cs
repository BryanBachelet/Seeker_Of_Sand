using GuerhoubaGames.Character;
using GuerhoubaGames.GameEnum;
using System;
using UnityEngine;

namespace SpellSystem
{
    public struct AreaData
    {
        public CharacterShoot characterShoot;
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