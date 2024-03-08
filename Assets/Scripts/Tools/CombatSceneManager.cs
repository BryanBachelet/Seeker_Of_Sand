using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CombatSceneManager : MonoBehaviour
{
    [Header("Combat Scene Setup")]
    public GameObject player;
    public GameObject enemisManager;

    [Header("Combat Scene Options")]
    public bool constantSpawning;
    public bool stopPlayerExperience;
    public bool specialEnemisSquad;

    private Enemies.EnemyManager m_enemyManager;
    private Experience_System m_experienceSystem;
    [HideInInspector] public int[] mobCount = new int[3];

#if UNITY_EDITOR

    public void Start()
    {
        m_experienceSystem = player.GetComponent<Experience_System>();
        m_enemyManager = enemisManager.GetComponent<Enemies.EnemyManager>();
        SetupEnemyManagerParameters();
        SetupPlayerParameters();
    }

    private void SetupPlayerParameters()
    {
        m_experienceSystem.cancelGainExperience = stopPlayerExperience;
    }
    private void SetupEnemyManagerParameters()
    {
        m_enemyManager.activeTestPhase = true;
        m_enemyManager.activeSpawnConstantDebug = constantSpawning;
        m_enemyManager.activeSpecialSquad = specialEnemisSquad;
        if (specialEnemisSquad)
            m_enemyManager.SetSpawnSquad(mobCount);
        m_enemyManager.DebugInit();
    }

    public void OnValidate()
    {
        m_enemyManager = enemisManager.GetComponent<Enemies.EnemyManager>();
        if (specialEnemisSquad)
            m_enemyManager.SetSpawnSquad(mobCount);
    }

#endif
}
