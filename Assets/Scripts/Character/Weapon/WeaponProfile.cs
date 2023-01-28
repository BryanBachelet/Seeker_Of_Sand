using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


[Serializable]
public struct WeaponStats
{
    public int projectileNumber;
    public float shootAngle;
    public float shootNumber ;
    public float timeBetweenShot ;
}


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WeaponStat", order = 2)]
public class WeaponProfile : ScriptableObject
{
    public WeaponStats stats;
}
