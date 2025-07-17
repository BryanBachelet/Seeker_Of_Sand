using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


#region Affliction Struct Data


[Serializable]
public class AfflictionData
{
    public AfflictionType type;
    public float duration;
    public int stackMax;
}


[Serializable]
public class ChillData : AfflictionData
{
    public int slowPerStack;
}

public class FreezeData : AfflictionData
{
    public float antiFreezeDuration;
}

[Serializable]
public class BurnData : AfflictionData
{
    public int stackToTranform;
    public int damagePerStack;
}

[Serializable]
public class BlazeData : AfflictionData
{
    public float effectRemovePerDamage;
    public float damageIncreasePercentage;
    public float timeBetweenTick;
}

[Serializable]
public class ElectrifyData : AfflictionData
{

    public float increaseDamagePercentPerStack;
}

[Serializable]
public class ElectrocuteData : AfflictionData
{

    public float radiusElectrocute;
    public int dammagePerLightning;
    public float frequenceLightningStrike;
}
[Serializable]
public class PoisonData : AfflictionData
{
    public int dammagePoisonPerStack;
}
[Serializable]
public class IntoxicateData : AfflictionData
{

    public float exectutionPercentPerStack;
}
[Serializable]
public class LacerateData : AfflictionData
{
    public int damagerPerStack;
}

[Serializable]
public class BleedingData : AfflictionData
{ 
    public float increaseDamageBleeding;
    public float movementQuantityToReduceHitCooldown;
}
[Serializable]
public class ScareData : AfflictionData
{
    public int stackMaxToTranformTerrify;
    public float reduceDamagePerStack;
}
[Serializable]
public class TerrifyData : AfflictionData
{
    public float mouvementSpeedIncreasePercent;
}




#endregion




[CreateAssetMenu(fileName = "Affliction Profil", menuName = "Entites/Affliction Profil")]
public class AfflictionProfil : ScriptableObject
{

    public ChillData chillData;
    public FreezeData freezeData;
    public BurnData burnData;
    public BlazeData blazeData;
    public ElectrifyData electrifyData;
    public ElectrocuteData electrocuteData;
    public PoisonData poisonData;
    public IntoxicateData intoxicateData;
    public LacerateData lacerateData;
    public BleedingData bleedingData;
    public ScareData scareData;
    public TerrifyData terrifyData;


 


 
}
