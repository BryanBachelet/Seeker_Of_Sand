using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AfflictionManager : MonoBehaviour
{
    [SerializeField] private AfflictionProfil m_afflictionProfil;
    public Affliction[] afflictionArray ;
    [Header("Debug Infos")]
    public bool m_activeAfflictionInfo;


    private EntityModifier m_entityModifier;

    #region Unity Functions
    public void Start()
    {
        InitializeAfflictionArray();
    }

    public void Update()
    {
        UpdateAffliction();
    }

    #endregion

    public void InitializeAfflictionArray()
    {
        m_afflictionProfil.OnInit();

        m_entityModifier= GetComponent<EntityModifier>();

        int arrayLength = Enum.GetNames(typeof(AfflictionType)).Length;
        afflictionArray = new Affliction[arrayLength-1];
    }

    public void UpdateAffliction()
    {
        for (int i = 0; i < afflictionArray.Length; i++)
        {
                if (afflictionArray[i] == null || afflictionArray[i].type == AfflictionType.NONE) continue;

            afflictionArray[i].duration -= Time.deltaTime;
            if (m_activeAfflictionInfo)
            {
                Debug.Log("Affliction  " + afflictionArray[i].type + " is update");
            }
            if (afflictionArray[i].duration <= 0)
            {
                RemoveAffliction(afflictionArray[i]);
            }
        }

    }

    public void AddAfflictions(AfflictionType[] afflictionTypes)
    {
        for (int i = 0; i < afflictionTypes.Length; i++)
        {
            
            Affliction affliction = new Affliction();
            affliction.type = afflictionTypes[i];
            affliction.duration = m_afflictionProfil.afflictionData[(int)affliction.type -1 ].duration;
            affliction.stackCount = 1;
            AddAffliction(affliction);
            if (m_activeAfflictionInfo)
            {
                Debug.Log("Affliction : Add " +  affliction.type);
            }
        }

        
    }


    public void AddAffliction(Affliction affliction)
    {
        Affliction currentAffliction = afflictionArray[(int)affliction.type];

        if (currentAffliction == null)
        {
            currentAffliction = new Affliction();
            currentAffliction.type = affliction.type;
            currentAffliction.duration= affliction.duration;
            currentAffliction.stackCount += affliction.stackCount;
    
        }
        else
        {
            currentAffliction.duration = affliction.duration;
            currentAffliction.stackCount += affliction.stackCount;
        }
        
        afflictionArray[(int)affliction.type] = currentAffliction;
        // 3. Update Entity modifier
        // 4. Update UI and feedback

    }

    public void RemoveAffliction(Affliction affliction)
    {
        affliction.stackCount = 0;
        affliction.duration = 0;
        affliction.type = AfflictionType.NONE;
        // 3. Update Entity modifier
        // 4. Update UI and feedback

    }


}
