using GuerhoubaGames.GameEnum;
using GuerhoubaGames.Resources;
using SpellSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSpellSpawnerBehavior : MonoBehaviour
{
    [Header("Attack Pattern")]
    [SerializeField] private float frequenceAttack;
    [SerializeField] private float totalDuration;
    private float timeCounterAttack = 0.0f;
    private float timeCounter = 0.0f;
    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private SpellNature spellNature;
    [SerializeField] private CharacterObjectType characterObjectType;

    [SerializeField] private float areaSpawnerRadius = 1.0f;
       

    private SpellProfil attackProfil;

    [Header("Debug Infos")]
    [SerializeField]  private bool m_activeAttackSpawnGizmoDebug;


    #region Unity Functions


    void Update()
    {
        UpdateAttackSpawner();
    }


    #endregion

    private void UpdateAttackSpawner()
    {
        if(timeCounter > totalDuration)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            timeCounter += Time.deltaTime;
        }

        if(timeCounterAttack > frequenceAttack)
        {

            timeCounterAttack = 0;
            Vector3 positionToSpawn = this.transform.position + new Vector3(UnityEngine.Random.Range(-areaSpawnerRadius, areaSpawnerRadius),0, UnityEngine.Random.Range(-areaSpawnerRadius, areaSpawnerRadius));  
            SpawnArea(positionToSpawn);
        }
        else
        {
            timeCounterAttack += Time.deltaTime;
        }
      
    }
    
    public void InitializeObject(float fullDuration,float newFrequenceAttack,float spawnerRadius,GameObject attackToSpawn)
    {
        totalDuration = fullDuration;
        frequenceAttack = newFrequenceAttack;
        areaSpawnerRadius = spawnerRadius;
        objectToSpawn = attackToSpawn;
    }

    public void InitializeAreaData(GameElement element, float radius, int damage)
    {

        attackProfil = new SpellProfil();
        attackProfil.gameEffectStats.tagData.element = element;
        attackProfil.gameEffectStats.tagData.spellNatureType = SpellNature.AREA;
        attackProfil.gameEffectStats.tagData.spellMovementBehavior = SpellMovementBehavior.Fix;
        attackProfil.UpdateStatistics();

        attackProfil.gameEffectStats.AddToIntStats(StatType.Damage, damage);
        attackProfil.gameEffectStats.AddToFloatStats(StatType.Size, radius);


    }

    public void SpawnArea(Vector3 position)
    {
        GameObject instance = GamePullingSystem.SpawnObject(objectToSpawn, position, Quaternion.identity);

        AreaData data = new AreaData();
        data.spellProfil = attackProfil;
        data.destination = position;
        data.objectType = characterObjectType;

        AreaMeta areaMeta = instance.GetComponent<AreaMeta>();
        areaMeta.areaData = data;
        areaMeta.ResetOnSpawn();
    }

    #region Debugs Functions 

    public void OnDrawGizmos()
    {
        if (!m_activeAttackSpawnGizmoDebug) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, areaSpawnerRadius);
    }
    #endregion
}
