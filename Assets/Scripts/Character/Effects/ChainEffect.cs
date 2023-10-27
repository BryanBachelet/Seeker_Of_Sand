using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainEffect :MonoBehaviour
{
    public virtual CapsuleStats Active(CapsuleStats stats, LauncherStats launcherStats)
    {
        return stats;
    }
}


