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
    public string nameUgrade;
    public Sprite icon_Associat;

    public UpgradeProfil Clone()
    {
        UpgradeProfil profil = new UpgradeProfil();
        profil.type = type;
        profil.characterStats = characterStats;
        profil.weaponStats = weaponStats;
        profil.capsulsStats = capsulsStats;
        profil.description = description;
        profil.nameUgrade = nameUgrade;
        profil.icon_Associat = icon_Associat;

        return profil;
    }
}
