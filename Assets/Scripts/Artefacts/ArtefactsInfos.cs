using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConditionsTrigger
{
    OnHit,
    OnDeath,
}

public enum EntitiesTrigger
{
    Enemies,
    Self,
    Decor,
}


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Artefacts Info", order = 1)]
public class ArtefactsInfos : ScriptableObject
{
    [Range(0, 100)]
    public float spawnRate = 5;
    public ConditionsTrigger conditionsTrigger;
    public EntitiesTrigger entitiesTrigger;
    public GameObject m_artefactToSpawn;
    public Sprite icon;

    public void ActiveArtefact(Vector3 position, EntitiesTrigger tag, GameObject objectPre)
    {

        if (entitiesTrigger != tag) return;

        float change = Random.Range(0, 100);
        if (change < spawnRate)
        {
            GameObject obj = GameObject.Instantiate(m_artefactToSpawn, position, Quaternion.identity);
            Artefact.ArtefactData artefactData = obj.GetComponent<Artefact.ArtefactData>();
            artefactData.agent = objectPre;
        }
   
    }
}
