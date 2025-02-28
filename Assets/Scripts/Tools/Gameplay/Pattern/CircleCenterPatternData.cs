using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Circle Center Pattern Data", menuName = "Pattern 3D/ Circle Center")]

public class CircleCenterPatternData : PatternData
{
    public float angleOffset;
    public float radius;

    public override Vector3 GetPosition(int index, PatternDataInfo patternDataInfo)
    {
        Vector3 center = patternDataInfo.basePosition + patternDataInfo.baseDirection * -radius;
        return center + GetDirection(index, patternDataInfo) * radius;
    }

    public override Vector3 GetDirection(int index, PatternDataInfo patternDataInfo)
    {
     
        return Quaternion.AngleAxis(angleOffset * index, patternDataInfo.axeRotation) * patternDataInfo.baseDirection;
    }
}
