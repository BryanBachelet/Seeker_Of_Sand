using GuerhoubaGames.Artefact;
using GuerhoubaGames.Enemies;
using UnityEngine;

public class DamageUp_Aer : MonoBehaviour
{
    private ArtefactData m_artefactData;
    [HideInInspector] public int element;
    [Header("Around Parameter")]
    private float radiusEffect;
    public LayerMask enemyMask;
    public void Start()
    {
        m_artefactData = GetComponent<ArtefactData>();
        element = m_artefactData.elementIndex;
        radiusEffect = m_artefactData.radius;
        if (m_artefactData.entitiesTargetSystem == EntitiesTargetSystem.EnemyHit) OnDirectTarget();
        if (m_artefactData.entitiesTargetSystem == EntitiesTargetSystem.EnemyRandomAround) AroundTargetRandom();
        if (m_artefactData.entitiesTargetSystem == EntitiesTargetSystem.ClosestEnemyAround) ClosestTarget();
    }


    private void OnDirectTarget()
    {
        if (!m_artefactData.agent) return;

        NpcHealthComponent healthComponent = m_artefactData.agent.GetComponent<NpcHealthComponent>();
        ApplyEffect(healthComponent);
    }

    private void AroundTargetRandom()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, radiusEffect, enemyMask);

        if (enemies.Length == 0)
        {
            Destroy(gameObject);
            return;
        }

        int indexEnemy = Random.Range(0, enemies.Length);

        NpcHealthComponent healthComponent = enemies[indexEnemy].GetComponent<NpcHealthComponent>();
        ApplyEffect(healthComponent);
    }

    private void ClosestTarget()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, radiusEffect, enemyMask);

        if (enemies.Length == 0)
        {
            Destroy(gameObject);
            return;
        }

        int indexEnemy = -1;
        float minDistance = 10000;
        for (int i = 0; i < enemies.Length; i++)
        {
            float currentDistance = Vector3.Distance(m_artefactData.agent.transform.position, enemies[i].transform.position);
            if (currentDistance < minDistance)
            {
                indexEnemy = i;
                minDistance = currentDistance;
            }
        }
        if (indexEnemy == -1)
        {
            Destroy(gameObject);
            return;
        }


        NpcHealthComponent healthComponent = enemies[indexEnemy].GetComponent<NpcHealthComponent>();
        ApplyEffect(healthComponent);
    }

    private void ApplyEffect(NpcHealthComponent targetHealthComponent)
    {
        DamageStatData damageStatData = new DamageStatData(m_artefactData.damageToApply, m_artefactData.objectType);
        if (targetHealthComponent) targetHealthComponent.ReceiveDamage(m_artefactData.nameArtefact, damageStatData, Vector3.up, 1, element, (int)CharacterProfile.GetCharacterStat().baseDamage.totalValue);
    }
}
