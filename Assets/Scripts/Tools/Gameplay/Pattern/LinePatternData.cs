using UnityEngine;

[CreateAssetMenu(fileName = "Pattern Data", menuName = "Pattern 3D/Line Pattern") ]
public class LinePatternData : PatternData
{
    public float rangeOffset;
    public override Vector3 GetPosition(int index, PatternDataInfo patternDataInfo)
    {

        return patternDataInfo.basePosition+  patternDataInfo.baseDirection * (index * rangeOffset);
    }
}
