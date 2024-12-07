using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.GameEnum;
using System;

[System.Serializable]
public class CustomArrayName : PropertyAttribute
{
    public readonly string nametest;
    public CustomArrayName(string names) { nametest = names; }
}


namespace Artefact
{
    public class ArtefactData : MonoBehaviour
    {
        [HideInInspector] public GameObject agent;
        [HideInInspector] public GameObject characterGo;
        [HideInInspector] public EntitiesTargetSystem entitiesTargetSystem;
        [HideInInspector] public float radius;
        [HideInInspector] public CharacterObjectType objectType = CharacterObjectType.FRAGMENT;
        [HideInInspector] public string nameArtefact;
        [HideInInspector] public int elementIndex;
        [HideInInspector] public GameElement element;
        [HideInInspector] public int damageToApply;
        public Action OnSpawn;
    }
}