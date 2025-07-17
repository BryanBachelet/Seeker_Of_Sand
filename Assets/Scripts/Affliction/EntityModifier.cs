using GuerhoubaGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.GameEnum;
using Enemies;


public class EntityModifier : MonoBehaviour
{
    private float m_tickDurationBetweenDot = 0.5f;
    private float m_timerTickDot;

    [HideInInspector] public int bleedingDamage = 0;
    [HideInInspector] public float increaseBleedingDamage = 0.0f;

    //Check the entity movement 
    private Vector3 m_prevPosition;
    private float m_distanceCounter;
    [HideInInspector]  public float distanceProcBleeding = 50.0f;


    [HideInInspector] private IDamageReceiver m_damageReceiver;
    [HideInInspector] private AfflictionManager m_afflictionManager;


    [Header("Debug Variables and Infos")]
    public bool m_activeEnityModifierDebug = false;


    #region Unity Functions
    public void Start()
    {
        InitComponent();
    }

    public void Update()
    {
        if(m_damageReceiver.IsDead()) return;

        UpdateDamageOverTime();
        CheckEntityMovement();
    }

    #endregion



    private void InitComponent()
    {
        m_damageReceiver = GetComponent<IDamageReceiver>();
        m_afflictionManager = GetComponent<AfflictionManager>();
    }

    public void ActiveBleedingState()
    {
        m_prevPosition = transform.position;
    }

    private void UpdateDamageOverTime()
    {
        if (!m_afflictionManager.IsCarryAffliction(AfflictionType.LACERATION)
            && !m_afflictionManager.IsCarryAffliction(AfflictionType.BLEEDING))
            return;

        m_timerTickDot += Time.deltaTime;

        if (m_timerTickDot > m_tickDurationBetweenDot)
        {
            m_timerTickDot = 0;
            ApplyDamageOverTime();
        }
    }

    private void CheckEntityMovement()
    {

        if (!m_afflictionManager.IsCarryAffliction(AfflictionType.BLEEDING))
            return;

        float distance = Vector3.Distance(m_prevPosition, transform.position);
        m_prevPosition = transform.position;
        m_distanceCounter += distance;

        if(m_distanceCounter > distanceProcBleeding)
        {
            m_distanceCounter = 0;
            ApplyBleedingDamage() ;

            if (m_activeEnityModifierDebug)
            {
                Debug.Log("Affliction : Bleeding distance has been covered" + bleedingDamage);
            }
        }
    }

    private void ApplyDamageOverTime()
    {
        ApplyBleedingDamage();
    }

    private void ApplyBleedingDamage()
    {
        DamageStatData damageStatData = new DamageStatData();
        damageStatData.characterObjectType = CharacterObjectType.AFFLICTION;
        damageStatData.element = GameElement.NONE;
        damageStatData.damage = Mathf.RoundToInt(bleedingDamage * (1.0f + increaseBleedingDamage));

        m_damageReceiver.ReceiveDamage("Lacerate affliction", damageStatData, Vector3.zero, 0, -1, 0);

        if (m_activeEnityModifierDebug)
        {
            Debug.Log("Affliction : Bleeding Damage apply " + bleedingDamage);
        }
    }

    

}
