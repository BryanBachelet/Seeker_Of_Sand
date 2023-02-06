using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


[Serializable]
public struct LauncherStats
{
    public float slotCapsule;
    public float timeBetweenCapsule;
    public float reloadTime;
    public float rangeAdd;
    public float degatAdd;
}

[Serializable]
public struct CapsuleStats
{
    public float speed;
    public float range;
    public float damage;
    public float projectileNumber;
    public float timeBetweenShot;
    public float shootAngle;
    public float shootNumber;
}



[CreateAssetMenu(fileName = "LauncherData", menuName = "ScriptableObjects/LauncherStat", order = 2)]
public class LauncherProfil : ScriptableObject
{
    public LauncherStats stats;
}



