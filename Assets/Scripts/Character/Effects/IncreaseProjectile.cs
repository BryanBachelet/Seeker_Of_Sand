using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseProjectile : ChainEffect
{
    [SerializeField] private int bonusProjectileNumber = 1;
    [SerializeField] private float defaultAngle = 1;
    public override WeaponStats Active(WeaponStats stats)
    {
        base.Active(stats);
        stats.projectileNumber+= bonusProjectileNumber;
        if (stats.shootAngle == 0) stats.shootAngle = defaultAngle;
        return stats;
    }
}
