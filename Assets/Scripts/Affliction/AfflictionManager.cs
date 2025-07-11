using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AfflictionManager : MonoBehaviour
{
    public Affliction[] afflictionArray;


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
        int arrayLength = Enum.GetNames(typeof(AfflictionType)).Length;
        afflictionArray = new Affliction[arrayLength-1];
    }

    public void UpdateAffliction()
    {
        for (int i = 0; i < afflictionArray.Length; i++)
        {
            if(afflictionArray[i].type == AfflictionType.NONE) continue;

            afflictionArray[i].duration -= Time.deltaTime;
            if (afflictionArray[i].duration <= 0)
            {
                RemoveAffliction(afflictionArray[i]);
            }
        }

    }

    public void AddAffliction(Affliction affliction)
    {
        Affliction currentAffliction = afflictionArray[(int)affliction.type];

        if (currentAffliction == null)
        {
            currentAffliction.type = affliction.type;
            currentAffliction.duration= affliction.duration;
            currentAffliction.stackCount += affliction.stackCount;
            currentAffliction.freeze = affliction.freeze;
            currentAffliction.damage = affliction.damage;
            currentAffliction.damageIncrease= affliction.damageIncrease;
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
