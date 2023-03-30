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


public enum TrajectoryType
{
    LINE,
    CURVE
}

[Serializable]
public struct CapsuleStats
{
    public float lifetime;
    public float speed;
    public float range;
    public float damage;
    public float projectileNumber;
    public float totalShotTime;
    public float shootAngle;
    public TrajectoryType trajectory;
    public float shootNumber;
    [HideInInspector] public float timeBetweenShot 
    { 
        get 
        {   if (shootNumber == 1) return 0.2f;
            if (totalShotTime < 1) { Debug.LogError("Total Shot Time has a non valid time"); return 0.2f; }
            return (totalShotTime / shootNumber); } 
        private set { } 
    }

    public float GetSpeed(float rangeGive)
    {
        float speed = (rangeGive / ((lifetime-0.1f) * Mathf.Cos(45 * Mathf.Deg2Rad)));
        return speed;
    }

    public float GetVerticalSpeed(float rangeGive)
    {
        return GetSpeed(rangeGive) * Mathf.Sign(45 * Mathf.Deg2Rad);
    }

    public float GetGravitySpeed(float height, float rangeGive)
    {
        float speed = GetSpeed(rangeGive);
        float angle =  45 * Mathf.Deg2Rad;
        float gravity = 2 * (speed * Mathf.Sin(angle) * (lifetime - 0.1f) + height);
        gravity = gravity / ((lifetime - 0.1f)* (lifetime-0.1f) );
        return gravity;
    }
}



[CreateAssetMenu(fileName = "LauncherData", menuName = "ScriptableObjects/LauncherStat", order = 2)]
public class LauncherProfil : ScriptableObject
{
    public LauncherStats stats;
}



