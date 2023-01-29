using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainEffect :MonoBehaviour
{
    public virtual WeaponStats Active(WeaponStats stats)
    {
        return stats;
    }
}


