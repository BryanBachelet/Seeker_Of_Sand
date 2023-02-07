using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseProjectile : ChainEffect
{
    [SerializeField] private int bonusProjectileNumber = 1;
    [SerializeField] private float defaultAngle = 1;
    public override CapsuleStats Active(CapsuleStats stats,LauncherStats launcherStats)
    {
        base.Active(stats, launcherStats);
        stats.projectileNumber+= bonusProjectileNumber;
        if (stats.shootAngle == 0) stats.shootAngle = defaultAngle;
        return stats;
    }
}
