using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleManager : MonoBehaviour
{
    public CapsuleSystem.Capsule[] capsules;

    public CapsuleSystem.CapsuleAttackInfo[] attackInfo;
    public CapsuleSystem.CapsuleBuffInfo[] buffInfos;


    private int indexAttackInfo;
    private int indexBuffInfo;

    public void Awake()
    {
        CreateInfo();
    }
    public void CreateInfo()
    {
        for (int i = 0; i < capsules.Length; i++)
        {
            if(capsules[i].type == CapsuleSystem.CapsuleType.ATTACK)
            {
                capsules[i] = new CapsuleSystem.CapsuleAttack(attackInfo[indexAttackInfo]);
                indexAttackInfo++;
            }
            if (capsules[i].type == CapsuleSystem.CapsuleType.BUFF)
            {
                capsules[i] = new CapsuleSystem.CapsuleBuff(buffInfos[indexBuffInfo]);
                indexBuffInfo++;
            }
        }
    }

}
