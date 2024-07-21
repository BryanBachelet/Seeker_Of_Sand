using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;
using GuerhoubaGames.GameEnum;

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

public enum FormTypeSpell
{
    PROJECTILE,
    AREA,
}

[Serializable]
public struct CapsuleStats
{
    public float lifetime;
    public GameElement elementType;
    public float travelTime;
    [SerializeField] public bool useTravelTime;
    public float speed;
    public float range;
    public float damage;
    public int projectileNumber;
    public float totalShotTime;
    public float shootAngle;
    public TrajectoryType trajectory;
    public FormTypeSpell formType;
    public float shootNumber;
    public float timeInterval;
    public float size;
    public float sizeMultiplicatorFactor;
    public int piercingMax;
    public float spellCanalisation;
    public string description;
    public float stackDuration;
    public int stackPerGain;
    [Header("Curve Trajectory")]
    public float angleTrajectory;
    public float trajectoryTimer;
    public int level;
    

    public string DebugStat()
    {
        string debugString = "Lifetime :" + lifetime.ToString() +"\n";
        debugString += "Speed : " + speed.ToString() + "\n";
        debugString += "Range : " +range.ToString() + "\n";
        debugString += "Damage : " + damage.ToString() + "\n"; 
        debugString += "Projectile Number : " + projectileNumber.ToString() + "\n"; 
        debugString += "Shoot Number : " + shootNumber.ToString() + "\n";
        debugString += "Piercing Max : " + piercingMax.ToString() + "\n";
        debugString += "Level : " + level.ToString() + "\n";
        Debug.Log(debugString);
        return debugString;
    }


    [HideInInspector] public float timeBetweenShot
    {
        get
        {
            if (shootNumber == 1) return 0.2f;

            return (totalShotTime / shootNumber);
        }
        private set { }
    }


    public float[] GetVisibleStat()
    {
        float[] statsVisible = new float[7];

        statsVisible[0] = damage;
        statsVisible[1] = size;
        statsVisible[2] = speed;
        statsVisible[3] = projectileNumber;
        statsVisible[4] = shootNumber;
        statsVisible[5] = piercingMax;
        statsVisible[6] = level;
        return statsVisible;
    }
}



[CreateAssetMenu(fileName = "LauncherData", menuName = "ScriptableObjects/LauncherStat", order = 2)]
public class LauncherProfil : ScriptableObject
{
    public LauncherStats stats;
}



