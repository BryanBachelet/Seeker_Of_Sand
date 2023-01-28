using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WeaponStat", order = 2)]
public class WeaponStats : ScriptableObject
{
    public int projectileNumber;
    public float shootAngle;
}
