using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemies;
using UnityEngine.AI;

[System.Serializable]
public struct EnemyPullingInfo
{
    public int maxCount;
    public GameObject prefabToSpawn;
    public EnemyType type;
    public List<GameObject> instanceArray;
}

public class PullingSystem : MonoBehaviour
{
    #region Variables

    [Tooltip("Important to set enemyArrayInfo in the same order than the enemyType enum")]
    public List<EnemyPullingInfo> m_enemyInfoArray;

    [Header("Debug Parameters")]
    public bool activeDebug = false;

    [HideInInspector] public bool isFullyInitialized;

    private Vector3 m_spawnPosition = new Vector3(0, -1000, 0);
    #endregion

    public GameObject gameObjectHolder;
    // Init all the system
    public void InitializePullingSystem()
    {
        for (int i = 0; i < m_enemyInfoArray.Count; i++)
        {
            SpawnElementFromPullingSystem(m_enemyInfoArray[i]);
        }

        isFullyInitialized = true;
        if (activeDebug) Debug.Log("Pulling system is initialize");
    }

    /// <summary>
    /// Spawn all instance of an enemy type
    /// </summary>
    /// <param name="enemyTypeInfo"></param>
    private void SpawnElementFromPullingSystem(EnemyPullingInfo enemyTypeInfo)
    {
        for (int i = 0; i < enemyTypeInfo.maxCount; i++)
        {
            GameObject instance = GameObject.Instantiate(enemyTypeInfo.prefabToSpawn, m_spawnPosition, Quaternion.identity, gameObjectHolder.transform);
            instance.SetActive(false);
            instance.GetComponent<NpcMetaInfos>().SetPauseState();
            instance.GetComponent<NavMeshAgent>().enabled = false;
            enemyTypeInfo.instanceArray.Add(instance);
        }

        if (activeDebug) Debug.Log(enemyTypeInfo.type.ToString() + " is spwaned");
    }

    /// <summary>
    /// Check if there is still some instance of an enemy type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool IsStillInstanceOf(EnemyType type)
    {
        bool state = false;

        state = m_enemyInfoArray[(int)type].instanceArray.Count > 0;
        return state;
    }

    public GameObject GetEnemy(EnemyType type)
    {
        GameObject instance = null;
        instance = m_enemyInfoArray[(int)type].instanceArray[0];
        m_enemyInfoArray[(int)type].instanceArray.RemoveAt(0);
        NpcHealthComponent healthComponent = instance.GetComponent<NpcHealthComponent>();
        healthComponent.RemovePauseState();
        instance.SetActive(true);

        if (activeDebug) Debug.Log(type.ToString() + " is pull out");

        return instance;
    }

    public void ResetEnemy(GameObject instance,EnemyType type)
    {
        instance.GetComponent<NpcMetaInfos>().state = NpcState.MOVE;
        instance.GetComponent<NpcMetaInfos>().SetPauseState();
        instance.GetComponent<NavMeshAgent>().updatePosition = false;
        instance.GetComponent<NavMeshAgent>().enabled = false;
        instance.SetActive(false);
        m_enemyInfoArray[(int)type].instanceArray.Add(instance);


        if (activeDebug) Debug.Log(type.ToString() + " is reset");
    }

    public void ResetEnemyNavMesh(GameObject instance, EnemyType type)
    {
        instance.GetComponent<NpcMetaInfos>().state = NpcState.MOVE;
        instance.GetComponent<NpcMetaInfos>().SetPauseState();
        instance.GetComponent<NavMeshAgent>().updatePosition = false;
        instance.GetComponent<NavMeshAgent>().enabled = false;
    }
        public int GetAllEnemyCount()
    {
        int maxPop = 0;
        for (int i = 0; i < m_enemyInfoArray.Count; i++)
        {
           maxPop +=  m_enemyInfoArray[i].maxCount;
        }

        return maxPop;
    }
}
