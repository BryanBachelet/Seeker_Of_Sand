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

    public virtual void Setup(int capsuleCount)
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
    public virtual void Apply(ref CapsuleStats playerStat)
    {
      
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
    public UpgradeCapsule(UpgradeProfil profil) : base(profil)
    {
        gain.type = UpgradeType.CAPSULE;
        m_baseString = gain.description;
    }

    public override void Setup(int capsuleCount)
    {
        base.Setup(capsuleCount);
        capsuleIndex = UnityEngine.Random.Range(0, capsuleCount);
        gain.description += " " + capsuleIndex.ToString();
    }

    public override void Apply(ref CapsuleStats playerStat)
    {
        base.Apply(ref playerStat);
        playerStat.damage += gain.capsulsStats.damage;
        playerStat.projectileNumber += gain.capsulsStats.projectileNumber;
        playerStat.range += gain.capsulsStats.range;
        playerStat.shootAngle += gain.capsulsStats.shootAngle;
        playerStat.shootNumber += gain.capsulsStats.shootNumber;
        Debug.Log(playerStat.shootNumber + "=  Shoot Number  |" + playerStat.projectileNumber + "=  Projectil Number");
        gain.description = m_baseString;
    }

    public override void Destroy()
    {
        gain.description = m_baseString;
    }
}


