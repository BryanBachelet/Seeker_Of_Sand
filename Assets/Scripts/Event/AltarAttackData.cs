using GuerhoubaGames.GameEnum;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static JBooth.MicroSplat.TraxManager;


[CreateAssetMenu(fileName = "Altar Attack Data", menuName = "Altar/ Altar Attack Data")]
public class AltarAttackData : ScriptableObject
{
    public GameElement element;
    public int difficulty;

    public GameObject prefabAttack;

    public bool isArea;
    public bool isProjectile;
    public bool isObstacle;

    public bool hasVfx;
    public GameObject vfx;

    

    [Header("Altar Attack Stats")]
    public int damageAttack;
    public float maxAttackPerShape;
    public float cooldownBetweenAttack;

    public bool IsOneShot;
    [Tooltip("Cooldown before the repetition of the attack pattern")]
    public float cooldownBehavior;
    public float cooldownShape;
    public int maxShapeCount;

    [Header("Area Attack ")]
    public float releaseAreaAttackTime;
    public float radiusAttack;

    [Header("Projectile Attack")]
    public float mvtSpeedProjectile;
    public float durationProjectileLife;
    public bool hasFaceDirection;


    [Header("Predictions Variables")]
    public bool isStartAtPredictingPosition;
    public bool hasPredictingDirection;
    public bool inverseDirection;
    public bool isPredictingPosition;
    public float imprecisionLevel;
    public float predicitonPercent;


    [Header("PatternData")]
    public PatternData behaviorPattern;
    public PatternData shapePattern;
    public PatternData attackPattern;
    public Vector3 offsetAttackPosition;

    public Vector3 PredictionTargetPosition(Vector3 targetPosition, Vector3 targetVelocity)
    {
        float posX = Random.Range(-imprecisionLevel, imprecisionLevel);
        float posZ = Random.Range(-imprecisionLevel, imprecisionLevel);
        Vector3 imprecisionPosition = targetPosition + new Vector3(posX, 0, posZ);


        return imprecisionPosition + targetVelocity * predicitonPercent;
    }
}
