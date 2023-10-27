using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Character Data", order = 1)]
public class CharacterData : ScriptableObject
{
    public CharacterStat stats;
}
