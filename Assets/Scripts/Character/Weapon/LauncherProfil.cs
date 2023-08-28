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
    public float travelTime;
    [SerializeField] public bool useTravelTime;
    public float speed;
    public float range;
    public float damage;
    public float projectileNumber;
    public float totalShotTime;
    public float shootAngle;
    public TrajectoryType trajectory;
    public float angleTrajectory;
    public float shootNumber;
    public float timeInterval;
    public float size;
    public float sizeMultiplicatorFactor;
    public int piercingMax;
    public string description;

    [HideInInspector] public float timeBetweenShot 
    { 
        get 
        {   if (shootNumber == 1) return 0.2f;
            
            return (totalShotTime / shootNumber); } 
        private set { } 
    }

    public float GetSpeed(float rangeGive)
    {
        float speed = (rangeGive / ((GetTravelTime()) * Mathf.Cos(angleTrajectory * Mathf.Deg2Rad)));
        return speed;
    }

    public float GetVerticalSpeed(float rangeGive)
    {
        return GetSpeed(rangeGive) * Mathf.Sign(angleTrajectory * Mathf.Deg2Rad);
    }

    public float GetGravitySpeed(float height, float rangeGive)
    {
        float speed = GetSpeed(rangeGive);
        float angle = angleTrajectory * Mathf.Deg2Rad;
        float gravity = 2 * (speed * Mathf.Sin(angle) * (GetTravelTime()) + height);
        gravity = gravity / ((GetTravelTime()) * (GetTravelTime()) );
        return gravity;
    }

    public float GetTravelTime()
    {
        return travelTime != 0 ? travelTime : lifetime;
    }
}



[CreateAssetMenu(fileName = "LauncherData", menuName = "ScriptableObjects/LauncherStat", order = 2)]
public class LauncherProfil : ScriptableObject
{
    public LauncherStats stats;
}



