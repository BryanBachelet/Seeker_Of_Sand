using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
[CreateAssetMenu(fileName = "GeneralStatData", menuName = "ScriptableObjects/Artefacts Data", order = 2)]
public class GeneralStatData : ScriptableObject
{
  public  CharacterStat CharacterStat;
}
