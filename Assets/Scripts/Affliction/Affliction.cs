using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AfflictionType
{
    NONE =0 ,
    BLEEDING= 1 ,
    DEEP_BLEEDING =2,
    BURN =3,
    COMBUSTION =4,
    FROSTBITE =5,
    FREEZE = 6,
    POISON = 7,
    INTOXICATE = 8,

}

public class Affliction
{
    public float duration;
    public int stackCount;
    public AfflictionType type;

    // Stats special
    public float damage;
    public float slowness;
    public bool freeze;
    public float damageIncrease;
}
