using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Circle Pattern Data", menuName = "Pattern 3D/ Circle Pattern Data")]
public class CirclePatternData : PatternData
{

    public float angleOffset;
    public float radius;
    public float baseAngle;

    public override Vector3 GetPosition(int index, PatternDataInfo patternDataInfo)
    {
        return patternDataInfo.basePosition + GetDirection(index, patternDataInfo) * radius;
    }

    public override Vector3 GetDirection(int index, PatternDataInfo patternDataInfo)
    {
        return Quaternion.AngleAxis(angleOffset * index, patternDataInfo.axeRotation) * Quaternion.AngleAxis(baseAngle , patternDataInfo.axeRotation) *patternDataInfo.baseDirection;
    }
}
