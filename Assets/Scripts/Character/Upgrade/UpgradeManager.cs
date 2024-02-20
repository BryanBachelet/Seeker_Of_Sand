using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public struct UpgradeLevelingData
{
    public int spellCount;
    public int upgradePoint;
    public int indexSpellFocus;
    public CapsuleStats[] spellState;
    public Sprite[] iconSpell;
    public Upgrade[] upgradeChoose;
}

public class UpgradeManager : MonoBehaviour
{
    private const int upgradeGenerateCount = 3;
    public UpgradeProfil[] upgradeList;
    public CharacterUpgrade m_characterUpgradeComponent;
    [HideInInspector] public DropInventory m_dropInventory;
    [Header("UI Upgrade")]
    [Tooltip("Not neccessary")]
    public GameObject upgradeLevelUi;
    public GameObject upgradeBook;
    private UpgradeChoosing m_upgradeChoosingComponent;
    private bool isUpgradeUILevel;

    private UpgradeLevelingData m_upgradeLevelingData ;

    private UpgradeData.UpgradeTable m_upgradeData = new UpgradeData.UpgradeTable();

    public void Start()
    {

        string path = Application.dataPath + "\\Resources\\UpgradeTable.csv";
        LoadUpgradeTable(path);
        if (!upgradeLevelUi) return;
        m_upgradeChoosingComponent = upgradeLevelUi.GetComponent<UpgradeChoosing>();
        m_upgradeChoosingComponent.m_upgradeManager = this;
        m_dropInventory = m_characterUpgradeComponent.GetComponent<DropInventory>();
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

    public Upgrade GetRandomUpgradeToSpell(int indexSpell)
    {
        int index = m_upgradeData.RandomUpgradeSpell(indexSpell);

        UpgradeProfil nxtProfil = upgradeList[index].Clone();
        Upgrade upgrade = new UpgradeCapsule(nxtProfil);
        return upgrade;
    }

   public Upgrade[] GetRandomUpgradesToSpecificSpell(int spellIndex)
    {
        Upgrade[] upgradeGenerate = new Upgrade[upgradeGenerateCount];
        for (int i = 0; i < upgradeGenerate.Length; i++)
        {
            int index = m_upgradeData.RandomUpgradeSpell(spellIndex);
            UpgradeProfil nxtProfil = upgradeList[index].Clone();
            upgradeGenerate[i] = new UpgradeCapsule(nxtProfil);
        }
        return upgradeGenerate;
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
            int choose = UnityEngine.Random.Range(0, indexArray.Count);
            indexChoose[i] = indexArray[choose];
            indexArray.RemoveAt(choose);
        }

        return indexChoose;
    }

    public void LoadUpgradeTable(string path)
    {

        FileStream fs = File.OpenRead(path);
        StreamReader streamReader = new StreamReader(fs);
        string[] lineHeader = GuerhoubaTools.FileManagement.GetLineCSV(streamReader);

        int spellNumber = int.Parse(lineHeader[1]);
        int upgradeNumber = int.Parse(lineHeader[3]);

        string[,] upgradeTableData = new string[spellNumber, upgradeNumber];

        for (int i = 0; i < spellNumber; i++)
        {
            string[] data = GuerhoubaTools.FileManagement.GetLineCSV(streamReader);
            for (int j = 0; j < upgradeNumber; j++)
            {
                upgradeTableData[i, j] = data[j];
            }
        }

        m_upgradeData.ReadDocumentData(upgradeTableData, spellNumber, upgradeNumber);

    }

    #region Upgrade Level UI Functions
    public void OpenUpgradeUI(UpgradeLevelingData upgradeLevelingData)
    {
        if (!upgradeLevelUi) return;
        upgradeLevelUi.SetActive(true);
        upgradeBook.SetActive(true);
        if (m_upgradeLevelingData.upgradeChoose == null)
        {
            int indexSpell = Random.Range(0, m_upgradeLevelingData.spellCount);
            m_upgradeLevelingData.upgradeChoose = GetRandomUpgradesToSpecificSpell(indexSpell);
            m_upgradeLevelingData.indexSpellFocus = indexSpell;
        }

        m_upgradeLevelingData.spellCount = upgradeLevelingData.spellCount;
        m_upgradeLevelingData.spellState = upgradeLevelingData.spellState;
        m_upgradeLevelingData.iconSpell = upgradeLevelingData.iconSpell;
        m_upgradeLevelingData.upgradePoint = upgradeLevelingData.upgradePoint;
    
        m_upgradeChoosingComponent.SetNewUpgradeData(m_upgradeLevelingData);
        Debug.Log("Open Upgrade interface");
    }

    public void UpdateUpgradeLevelingData(UpgradeLevelingData upgradeLevelingData)
    {
        m_upgradeLevelingData = upgradeLevelingData;
    }

    public void CloseUpgradeUI()
    {
        if (!upgradeLevelUi) return;
        upgradeLevelUi.SetActive(false);
        upgradeBook.SetActive(false);

        Debug.Log("Close Upgrade interface");
    }


    public void SendUpgrade(Upgrade upgradeChoose)
    {
        m_characterUpgradeComponent.ApplyUpgrade(upgradeChoose);
        int indexSpell = Random.Range(0, m_upgradeLevelingData.spellCount);
        m_upgradeLevelingData.upgradeChoose = GetRandomUpgradesToSpecificSpell(indexSpell);
        m_upgradeLevelingData.indexSpellFocus = indexSpell;
        m_upgradeLevelingData.upgradePoint--;
        m_upgradeChoosingComponent.SetNewUpgradeData(m_upgradeLevelingData);

    }


    #endregion
}


namespace UpgradeData
{
    using System;
    public class UpgradeTable
    {
        public int spellNumber;
        public int upgradeNumber;
        public bool[,] upgradeTableArray;

        public void ReadDocumentData(string[,] data, int spellNumber, int upgradeNumber)
        {
            upgradeTableArray = new bool[spellNumber, upgradeNumber];
            this.spellNumber = spellNumber;
            this.upgradeNumber = upgradeNumber;

            for (int i = 1; i < spellNumber; i++)
            {
                for (int j = 1; j < upgradeNumber; j++)
                {
                    upgradeTableArray[i - 1, j - 1] = Convert.ToBoolean(int.Parse(data[i, j]));
                }
            }
        }

        public bool CheckValidUpgrade(int indexSpell, int indexUpgrade)
        {
            return upgradeTableArray[indexSpell, indexUpgrade];
        }

        public int RandomUpgradeSpell(int indexSpell)
        {
            int[] array = new int[upgradeNumber];
            int ugradeAdd = 0;
            for (int i = 0; i < upgradeNumber; i++)
            {
                if (upgradeTableArray[indexSpell, i])
                {
                    array[ugradeAdd] = i;
                    ugradeAdd++;
                }
            }

            int indexUpgrade = UnityEngine.Random.Range(0, ugradeAdd);
            return array[indexUpgrade];
        }
    }
}



