using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseShot : ChainEffect
{

   [SerializeField] private int bonusShotNumber = 1;
   [SerializeField] private float defaultTimeBetweenShoot = 0.2f;
    public override CapsuleStats Active(CapsuleStats stats, LauncherStats launcherStats)
    {
        base.Active(stats,launcherStats) ;
        stats.shootNumber += bonusShotNumber;
        if (launcherStats.timeBetweenCapsule == 0) launcherStats.timeBetweenCapsule = defaultTimeBetweenShoot;
        return stats;
    }
}
