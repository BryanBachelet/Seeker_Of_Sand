using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Affliction Type enum
public enum AfflictionType
{
    NONE =0 ,
    LACERATION= 1 ,
    BLEEDING =2,
    BURN =3,
    BLAZE =4,
    CHILL =5,
    FREEZE = 6,
    POISON = 7,
    INTOXICATE = 8,
    ELECTRIFIED = 9,
    ELECTROCUTE = 10,
    SCARE = 11,
    TERRIFY = 12,

}

public class Affliction
{
    public float duration;
    public int stackCount;
    public AfflictionType type;

}
