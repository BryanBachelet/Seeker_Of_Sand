using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Upgrade_", menuName = "ScriptableObjects/Upgrade_Profile", order = 3)]
[Serializable]
public class UpgradeProfil : ScriptableObject
{
    public UpgradeType type;
    public CharacterStat characterStats;
    public LauncherStats weaponStats;
    public CapsuleStats capsulsStats;
    public string description;
}
