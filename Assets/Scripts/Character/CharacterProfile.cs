using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct CharacterStats
{
    [HideInInspector] public float movementSpeed;
     public int health;
}
public class CharacterProfile : MonoBehaviour
{
    public CharacterStats stats;
}
