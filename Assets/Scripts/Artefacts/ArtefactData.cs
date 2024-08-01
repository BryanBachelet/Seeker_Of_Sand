using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.GameEnum;

namespace Artefact
{
    public class ArtefactData : MonoBehaviour
    {
        [HideInInspector] public GameObject agent;
        [HideInInspector] public GameObject characterGo;
        [HideInInspector] public EntitiesTargetSystem entitiesTargetSystem;
        [HideInInspector] public float radius;
        [HideInInspector] public CharacterObjectType objectType = CharacterObjectType.FRAGMENT;
    }
}