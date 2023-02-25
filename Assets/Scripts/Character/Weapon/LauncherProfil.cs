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
    public float totalShotTime;
    public float shootAngle;
    public float shootNumber;
    [HideInInspector] public float timeBetweenShot 
    { 
        get 
        {   if (shootNumber == 1) return 0.2f;
            if (totalShotTime < 1) { Debug.LogError("Total Shot Time has a non valid time"); return 0.2f; }
            return (totalShotTime / shootNumber); } 
        private set { } 
    }
}



[CreateAssetMenu(fileName = "LauncherData", menuName = "ScriptableObjects/LauncherStat", order = 2)]
public class LauncherProfil : ScriptableObject
{
    public LauncherStats stats;
}



