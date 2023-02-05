using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Upgrade_Profile", order = 3)]
[Serializable]
public class UpgradeProfile : ScriptableObject
{
    public CharacterStat statGain;
    public string description;
}

[Serializable]
public class Upgrade
{
    [Tooltip("Stats give by the upgrade")]
    public UpgradeProfile gain;

    public Upgrade(UpgradeProfile profil) { gain = profil; }
    public void Apply(ref CharacterStat playerStat)
    {
        playerStat.baseStat.attackSpeed += gain.statGain.baseStat.attackSpeed;
        playerStat.baseStat.healthMax += gain.statGain.baseStat.healthMax;
        playerStat.baseStat.speed += gain.statGain.baseStat.speed;
        playerStat.attrackness += gain.statGain.attrackness;
        playerStat.luck += gain.statGain.luck;
    }
}
