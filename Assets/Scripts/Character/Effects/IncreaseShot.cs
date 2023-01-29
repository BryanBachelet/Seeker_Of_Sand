using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseShot : ChainEffect
{

   [SerializeField] private int bonusShotNumber = 1;
   [SerializeField] private float defaultTimeBetweenShoot = 0.2f;
    public override WeaponStats Active(WeaponStats stats)
    {
        base.Active(stats);
        stats.shootNumber += bonusShotNumber;
        if (stats.timeBetweenShot == 0) stats.timeBetweenShot = defaultTimeBetweenShoot;
        return stats;
    }
}
