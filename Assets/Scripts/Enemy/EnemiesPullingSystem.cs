using GuerhoubaGames.Enemies;
using GuerhoubaGames.Resources;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public struct EnemyPullingInfo
{
    public int maxCount;
    public GameObject prefabToSpawn;
    public EnemyType type;
    public List<GameObject> instanceArray;
}

public class EnemiesPullingSystem : MonoBehaviour
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

    private List<PullConstructionData> pullingObjectData = new List<PullConstructionData>();

    // Init all the system
    public void InitializePullingSystem()
    {
        // Collect Pulling info

        for (int i = 0; i < m_enemyInfoArray.Count; i++)
        {
            GameObject prefab = m_enemyInfoArray[i].prefabToSpawn;
            NPCPullingData NpcPullingData = prefab.GetComponent<NPCPullingData>();
            if (NpcPullingData == null) continue;

            for (int j = 0; j < NpcPullingData.pullDataList.Count; j++)
            {
                if (pullingObjectData.Contains(NpcPullingData.pullDataList[j]))
                {
                    int index = pullingObjectData.IndexOf(NpcPullingData.pullDataList[j]);
                    PullConstructionData pullConstrutionData = pullingObjectData[index];
                    pullConstrutionData.count += NpcPullingData.pullDataList[j].count * m_enemyInfoArray[i].maxCount;
                    pullingObjectData[index] = pullConstrutionData;
                }
                else
                {
                    PullConstructionData pullConstrutionData = NpcPullingData.pullDataList[j];
                    pullConstrutionData.count = pullConstrutionData.count * m_enemyInfoArray[i].maxCount;
                    pullingObjectData.Add(pullConstrutionData);
                }
            }
        }

        for (int i = 0; i < pullingObjectData.Count; i++)
        {
            GamePullingSystem.instance.CreatePull(pullingObjectData[i]);
        }

        for (int i = 0; i < m_enemyInfoArray.Count; i++)
        {
            SpawnEnemisFromPullingSystem(m_enemyInfoArray[i]);
        }

        isFullyInitialized = true;
        if (activeDebug) Debug.Log("Pulling system is initialize");
    }

    /// <summary>
    /// Spawn all instance of an enemy type
    /// </summary>
    /// <param name="enemyTypeInfo"></param>
    private void SpawnEnemisFromPullingSystem(EnemyPullingInfo enemyTypeInfo)
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

    public void ResetEnemy(GameObject instance, EnemyType type)
    {
        instance.GetComponent<NpcMetaInfos>().state = NpcState.MOVE;
        instance.GetComponent<NpcMetaInfos>().SetPauseState();
        instance.GetComponent<NavMeshAgent>().updatePosition = false;
        instance.GetComponent<NavMeshAgent>().enabled = false;
        instance.SetActive(false);
        instance.transform.position = m_spawnPosition;
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
            maxPop += m_enemyInfoArray[i].maxCount;
        }

        return maxPop;
    }
}
