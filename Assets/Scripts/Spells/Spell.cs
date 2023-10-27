using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spell
{
    #region Enums

    public enum Form
    {
        PROJECTILE = 0,
        AREA = 1,
        LINK = 2,
        INSTANTE = 3,
        AURA = 4,
    }

    public enum Type
    {
        ATTACK =0 ,
        BUFF = 1,
        SPECIAL=2,
    }

    #endregion

    [CreateAssetMenu(fileName = "Default", menuName = "ScriptableObjects/Spell", order = 5)]
    public class Spell : ScriptableObject
    {
        public string name;
        public float canalisationTIme;
        public float timeBetweenSpell;
        public int level;
        public GameObject SpellObj;
    }

}