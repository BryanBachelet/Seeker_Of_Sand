using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public UpgradeProfile[] upgradeList;

    public Upgrade[] RandomUpgrade(int count)
    {
        int[] index = GetRandomIndex(count,upgradeList.Length);
        Upgrade[] upgrades = new Upgrade[count];
        for (int i = 0; i < count; i++)
        {
            UpgradeProfile nxtProfil = upgradeList[index[i]];
            upgrades[i] = new Upgrade(nxtProfil);
        }
        return upgrades;
    }

    public int[] GetRandomIndex(int elementRange, int length)
    {
        List<int> indexArray =new List<int>();
        for (int i = 0; i < length; i++)
        {
            indexArray.Add( i);
        }

        int[] indexChoose = new int[elementRange];
        for (int i = 0; i < indexChoose.Length; i++)
        {
            int choose =   Random.Range(0, indexArray.Count);
            indexChoose[i] = indexArray[choose];
            indexArray.RemoveAt(choose);
        }

        return indexChoose;
    }
}
