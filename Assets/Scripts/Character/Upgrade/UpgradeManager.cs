using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public UpgradeProfil[] upgradeList;

    public Upgrade[] RandomUpgrade(int count)
    {
        int[] index = GetRandomIndex(count,upgradeList.Length);
        Upgrade[] upgrades = new Upgrade[count];
        for (int i = 0; i < count; i++)
        {
            UpgradeProfil nxtProfil = upgradeList[index[i]];
            switch (upgradeList[index[i]].type)
            {
                case UpgradeType.CHARACTER:
                    upgrades[i] = new UpgradeCharacter(nxtProfil);
                    break;
                case UpgradeType.LAUNCHER:
                    upgrades[i] = new UpgradeLauncher(nxtProfil);
                    break;
                case UpgradeType.CAPSULE:
                    upgrades[i] = new UpgradeCapsule(nxtProfil);
                    break;
                default:
                    break;
            }
            
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
