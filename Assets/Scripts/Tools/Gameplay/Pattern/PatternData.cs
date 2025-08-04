using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct PatternDataInfo
{
  public  Vector3 basePosition;
  public  Vector3 baseDirection;
  public  Vector3 axeRotation;
}

[Serializable]
[CreateAssetMenu(fileName = "Empty Pattern Data", menuName = "Pattern 3D/ Empty Pattern Data")]
public class PatternData : ScriptableObject
{

    public virtual Vector3 GetPosition(int index, PatternDataInfo patternDataInfo)
    {
        return patternDataInfo.basePosition;
    }

    public virtual Vector3 GetDirection(int index, PatternDataInfo patternDataInfo)
    {
        return patternDataInfo.baseDirection;
    }
}
