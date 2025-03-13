using GuerhoubaGames.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltarAttackComponent : MonoBehaviour
{

    [Header("Infos Variables")]
    [SerializeField] private AltarAttackData m_attackData;


    private float m_cooldownBehaviorTimer;
    private Vector3 m_eventBasePosition;
    private Vector3 m_VFXspawnPosition;
    private Transform m_playerTransform;
    private Coroutine[] m_PatternCoroutine = new Coroutine[0];
    private List<GameObject> m_obstacleList = new List<GameObject>();

    private int indexBehaviorLaunch;

    #region Unity Functions
    public void Start()
    {
        InitComponent();
        DeactivateAltarAttack();
    }

    public void Update()
    {
        if (m_attackData.IsOneShot)
            return;

        if (m_cooldownBehaviorTimer >= m_attackData.cooldownBehavior)
        {
            LaunchBehavior();
        }
        else
        {
            m_cooldownBehaviorTimer += Time.deltaTime;
        }

    }
    #endregion

    public void InitComponent()
    {
        m_playerTransform = GameState.s_playerGo.transform;

    }

    public void LaunchBehavior()
    {
        Vector3 predictPlayerPosition = GetStartPosition();
        Vector3 startDirection = GetStartDirection(predictPlayerPosition);

        PatternDataInfo patternDataInfo = new PatternDataInfo();
        patternDataInfo.axeRotation = Vector3.up;
        patternDataInfo.basePosition = predictPlayerPosition;
        patternDataInfo.baseDirection = startDirection.normalized;

        predictPlayerPosition = m_attackData.behaviorPattern.GetPosition(indexBehaviorLaunch, patternDataInfo);
        startDirection = m_attackData.behaviorPattern.GetDirection(indexBehaviorLaunch, patternDataInfo);

        for (int i = 0; i < m_attackData.maxShapeCount; i++)
        {
            StartCoroutine(LaunchAttackPattern(i, predictPlayerPosition, startDirection));
        }
        m_cooldownBehaviorTimer = 0.0f;
        indexBehaviorLaunch++;
    }

    public Vector3 GetStartPosition()
    {
        Vector3 startingPosition = Vector3.zero;
        if (m_attackData.isStartAtPredictingPosition)
        {
            startingPosition = m_attackData.PredictionTargetPosition(m_playerTransform.position, m_playerTransform.GetComponent<Rigidbody>().velocity);
        }
        else
        {
            startingPosition = m_eventBasePosition;
        }

        return startingPosition;
    }

    public Vector3 GetStartDirection(Vector3 PredictPosition)
    {
        Vector3 startDirection = Vector3.zero;

        if (m_attackData.hasPredictingDirection)
        {
            startDirection = m_attackData.PredictionTargetPosition(m_playerTransform.position, m_playerTransform.GetComponent<Rigidbody>().velocity) - m_eventBasePosition;
        }
        else
        {
            startDirection = Vector3.forward;
        }

        if (m_attackData.inverseDirection)
            startDirection = -startDirection;

        return startDirection.normalized;
    }

    public IEnumerator LaunchAttackPattern(int index, Vector3 predictPlayerPosition, Vector3 direction)
    {

        // Pattern
        PatternDataInfo patternDataInfo = new PatternDataInfo();
        patternDataInfo.axeRotation = Vector3.up;
        patternDataInfo.basePosition = predictPlayerPosition;
        patternDataInfo.baseDirection = direction.normalized;

        predictPlayerPosition = m_attackData.shapePattern.GetPosition(index, patternDataInfo);
        Vector3 directionAttack = m_attackData.shapePattern.GetDirection(index, patternDataInfo);
        for (int i = 0; i < m_attackData.maxAttackPerShape; i++)
        {
            LaunchAttack(i, predictPlayerPosition, directionAttack);
            yield return new WaitForSeconds(m_attackData.cooldownBetweenAttack);
        }

    }



    public void LaunchAttack(int indexAttack, Vector3 predictPlayerPosition, Vector3 direction)
    {
        Vector3 positionToSpawn = predictPlayerPosition;
        Vector3 directionToGo = m_playerTransform.position - transform.position;

        direction = new Vector3(direction.x, 0, direction.z);
        // Pattern
        PatternDataInfo patternDataInfo = new PatternDataInfo();
        patternDataInfo.axeRotation = Vector3.up;
        patternDataInfo.basePosition = positionToSpawn + m_attackData.offsetAttackPosition;
        patternDataInfo.baseDirection = direction.normalized;

        positionToSpawn = m_attackData.attackPattern.GetPosition(indexAttack, patternDataInfo);
        directionToGo = m_attackData.attackPattern.GetDirection(indexAttack, patternDataInfo);



        // -- Spawn Object --
        GameObject attackGO = GamePullingSystem.SpawnObject(m_attackData.prefabAttack, positionToSpawn + Vector3.up*0.2f, Quaternion.identity);

        // -- Check if Area --
        if (m_attackData.isArea)
        {
            AttackTrainingArea attackTrainingArea = attackGO.GetComponent<AttackTrainingArea>();
            if (attackTrainingArea == null)
            {
                Debug.LogError(attackGO.name + " doesn't have the script AttackTrainingArea");
                return;
            }
            attackTrainingArea.lifeTimeVFX = m_attackData.releaseAreaAttackTime;
            attackTrainingArea.playerTransform = m_playerTransform;
            attackTrainingArea.rangeHit = m_attackData.radiusAttack;
            attackTrainingArea.damage = m_attackData.damageAttack;
        }
        else if (m_attackData.isProjectile)
        {
            if (m_attackData.hasFaceDirection)
            {
                attackGO.transform.rotation *= Quaternion.FromToRotation(transform.forward, directionToGo);
            }
            SignAttack signAttack = attackGO.GetComponent<SignAttack>();
            signAttack.speedMovement = m_attackData.mvtSpeedProjectile;
            signAttack.positionToGo = attackGO.transform.position + directionToGo * m_attackData.durationProjectileLife * m_attackData.mvtSpeedProjectile;

            ProjectileEventDamage projectileEventDamage = attackGO.GetComponent<ProjectileEventDamage>();
            if (projectileEventDamage == null)
            {
                Debug.LogError(attackGO.name + " doesn't have the script ProjectileEventDamage");
                return;
            }
            projectileEventDamage.damage = m_attackData.damageAttack;


        }

        if (m_attackData.isObstacle)
        {
            m_obstacleList.Add(attackGO);
            attackGO.transform.rotation *= Quaternion.FromToRotation(transform.forward, directionToGo);
        }


        // --- Spawn VFX object --
        if (!m_attackData.hasVfx) return;
        GameObject vfx = Instantiate(m_attackData.vfx, m_VFXspawnPosition, Quaternion.identity);

        SignAttack signVfx = vfx.GetComponent<SignAttack>();
        signVfx.speedMovement = Vector3.Distance(vfx.transform.position, positionToSpawn) / m_attackData.releaseAreaAttackTime * Time.deltaTime;
        signVfx.positionToGo = positionToSpawn;

    }

    public void ActivateAltarAttack(AltarAttackData attackData, Vector3 eventBasePosition)
    {
        m_attackData = attackData;
        this.enabled = true;
        m_cooldownBehaviorTimer = 0.0f;
        indexBehaviorLaunch = 0;
        m_eventBasePosition = eventBasePosition;
        m_PatternCoroutine = new Coroutine[m_attackData.maxShapeCount];
        if (m_attackData.IsOneShot)
        {
            LaunchBehavior();
        }
    }

    public void DeactivateAltarAttack()
    {
        for (int i = 0; i < m_obstacleList.Count; i++)
        {
            Destroy(m_obstacleList[i]);
        }
        m_obstacleList.Clear();

        for (int i = 0; i < m_PatternCoroutine.Length; i++)
        {
            if (m_PatternCoroutine[i] != null)
                StopCoroutine(m_PatternCoroutine[i]);
        }
        this.enabled = false;

    }

}
