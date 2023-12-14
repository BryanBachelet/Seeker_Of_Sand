using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class UpgradeManager : MonoBehaviour
{
    public UpgradeProfil[] upgradeList;

    private UpgradeData.UpgradeTable m_upgradeData = new UpgradeData.UpgradeTable();

    public void Start()
    {
        string path = Application.dataPath + "\\Temp\\UpgradeTable.csv";
        LoadUpgradeTable(path);
    }

    public Upgrade[] RandomUpgrade(int count)
    {
        int[] index = GetRandomIndex(count, upgradeList.Length);
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
        List<int> indexArray = new List<int>();
        for (int i = 0; i < length; i++)
        {
            indexArray.Add(i);
        }

        int[] indexChoose = new int[elementRange];
        for (int i = 0; i < indexChoose.Length; i++)
        {
            int choose = Random.Range(0, indexArray.Count);
            indexChoose[i] = indexArray[choose];
            indexArray.RemoveAt(choose);
        }

        return indexChoose;
    }


    public void LoadUpgradeTable(string path)
    {

        //FileStream fs = File.OpenRead(path);
        //StreamReader streamReader = new StreamReader(fs);
        //string[] lineHeader = GuerhoubaTools.FileManagement.GetLineCSV(streamReader);

        //int spellNumber = int.Parse(lineHeader[1]);
        //int upgradeNumber = int.Parse(lineHeader[3]);

        //string[,] upgradeTableData = new string[spellNumber,upgradeNumber];

        

        //for (int i = 0; i < spellNumber; i++)
        //{
        //    string[] data =  GuerhoubaTools.FileManagement.GetLineCSV(streamReader);
        //    for (int j = 0; j < upgradeNumber; j++)
        //    {
        //        upgradeTableData[i, j] = data[j];
        //    }
        //}

        //m_upgradeData.ReadDocumentData(upgradeTableData, spellNumber, upgradeNumber);

    }
}

namespace UpgradeData
{
    public class UpgradeTable
    {
        public int spellNumber;
        public int upgradeNumber;
        public bool[,] upgradeTableArray;

        public void ReadDocumentData(string[,] data, int spellNumber, int upgradeNumber)
        {

        }
    }
}



