using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public enum UpgradeType
{
    CHARACTER,
    LAUNCHER,
    CAPSULE
}

[Serializable]
public class Upgrade
{
    [Tooltip("Stats give by the upgrade")]
    public UpgradeProfil gain;
    [HideInInspector] public int capsuleIndex;

    public  Upgrade(UpgradeProfil profil) { gain = profil; }

    public virtual void Setup(int capsuleCount, Sprite sprite)
    {
    
    }

    public virtual void Destroy()
    {

    }
    public virtual void Apply(ref CharacterStat playerStat)
    {

    }
    public virtual void Apply(ref LauncherStats playerStat)
    {

    }
    public virtual CapsuleStats Apply( CapsuleStats playerStat)
    {
        return playerStat;
    }
}

public class UpgradeCharacter : Upgrade
{

    public UpgradeCharacter (UpgradeProfil profil) :base(profil)
    {
        gain.type = UpgradeType.CHARACTER;
    }

    public override void Apply(ref CharacterStat playerStat)
    {
        base.Apply(ref playerStat);
        playerStat.baseStat.healthMax += gain.characterStats.baseStat.healthMax;
        playerStat.baseStat.speed += gain.characterStats.baseStat.speed;
        playerStat.attrackness += gain.characterStats.attrackness;
        playerStat.luck += gain.characterStats.luck;
    }
}

public class UpgradeLauncher : Upgrade
{
    public UpgradeLauncher(UpgradeProfil profil) : base(profil)
    {
        gain.type = UpgradeType.LAUNCHER;
    }
    
    public override void Apply(ref LauncherStats playerStat)
    {
        base.Apply(ref playerStat);
        playerStat.degatAdd += gain.weaponStats.degatAdd;
        playerStat.rangeAdd += gain.weaponStats.rangeAdd;
        playerStat.reloadTime += gain.weaponStats.reloadTime;
        playerStat.slotCapsule += gain.weaponStats.slotCapsule;
        playerStat.timeBetweenCapsule += gain.weaponStats.timeBetweenCapsule;
    }
}

public class UpgradeCapsule : Upgrade
{
    private string m_baseString;
    private string m_baseStringDescription;
    public UpgradeCapsule(UpgradeProfil profil) : base(profil)
    {
        gain.type = UpgradeType.CAPSULE;
        m_baseString = gain.nameUpgrade;
        m_baseStringDescription = gain.description;
    }

    public override void Setup(int capsuleCount, Sprite sprite)
    {
        base.Setup(capsuleCount,sprite);
        capsuleIndex = capsuleCount;
        gain.icon_Associat = sprite;
    }

    public override CapsuleStats Apply( CapsuleStats playerStat)
    {
        playerStat =  base.Apply( playerStat);
        playerStat.damage += gain.capsulsStats.damage;
        playerStat.projectileNumber += gain.capsulsStats.projectileNumber;
        playerStat.range += gain.capsulsStats.range;
        playerStat.shootAngle += gain.capsulsStats.shootAngle;
        playerStat.shootNumber += gain.capsulsStats.shootNumber;
        playerStat.timeInterval += gain.capsulsStats.timeInterval;
        playerStat.size += gain.capsulsStats.size;
        playerStat.sizeMultiplicatorFactor += gain.capsulsStats.sizeMultiplicatorFactor;
        playerStat.piercingMax += gain.capsulsStats.piercingMax;
        playerStat.lifetime += gain.capsulsStats.lifetime;
        playerStat.level += 1;
        return playerStat;
        //gain.description = m_baseString;
    }

    public override void Destroy()
    {
        gain.description = m_baseStringDescription;
        gain.nameUpgrade = m_baseString;
    }
}


