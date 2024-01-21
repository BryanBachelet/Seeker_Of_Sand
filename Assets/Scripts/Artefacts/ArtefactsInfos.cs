using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConditionsTrigger
{
    OnHit,
    OnDeath,
    Contact,
}

public enum EntitiesTrigger
{
    Enemies,
    Self,
    Decor,
}


public enum EntitiesTargetSystem
{
    EnemyHit,
    ClosestEnemyAround,
    EnemyRandomAround,
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Artefacts Info", order = 1)]
public class ArtefactsInfos : ScriptableObject
{
    [Range(0, 100)]
    public float spawnRate = 5;
    public float radius = 30;
    public ConditionsTrigger conditionsTrigger;
    public EntitiesTrigger entitiesTrigger;
    public EntitiesTargetSystem entitiesTargetSystem;
    public GameObject m_artefactToSpawn;
    public Sprite icon;

    public void ActiveArtefactOnHit(Vector3 position, EntitiesTrigger tag, GameObject objectPre)
    {

        if (entitiesTrigger != tag) return;

        float change = Random.Range(0, 100);
        if (change < spawnRate)
        {
            GameObject obj = GameObject.Instantiate(m_artefactToSpawn, position, Quaternion.identity);
            Artefact.ArtefactData artefactData = obj.GetComponent<Artefact.ArtefactData>();
            SetupArtefactData(artefactData, objectPre);
        }

    }

    public void ActiveArtefactOnDeath(Vector3 position, EntitiesTrigger tag, GameObject agent, float distance)
    {
        Debug.Log("Artefact not in range");
        if (entitiesTrigger != tag || distance > radius) return;

        Debug.Log ("Artefact in range");

        float change = Random.Range(0, 100);
        if (change < spawnRate)
        {
            Debug.Log("Artefact in launch");
            GameObject obj = GameObject.Instantiate(m_artefactToSpawn, position, Quaternion.identity);
            Artefact.ArtefactData artefactData = obj.GetComponent<Artefact.ArtefactData>();
            SetupArtefactData(artefactData,agent);
        }
    }

    public void SetupArtefactData(Artefact.ArtefactData artefactData , GameObject agent)
    {
        artefactData.agent = agent;
        artefactData.entitiesTargetSystem = entitiesTargetSystem;
    }
}
