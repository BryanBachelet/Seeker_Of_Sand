using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Character;
using GuerhoubaGames.GameEnum;
using System.Text.RegularExpressions;
using System;

public struct UpgradeLevelingData
{
    public int spellCount;
    public int upgradePoint;
    public int indexSpellFocus;
    public int[] capsuleIndex;
    public SpellSystem.SpellProfil[] spellState;
    public Sprite[] iconSpell;
    public UpgradeObject[] upgradeChoose;
    public GameElement roomElement;
}

public class UpgradeManager : MonoBehaviour
{
    private const int upgradeGenerateCount = 3;
    public UpgradeObject[] upgradeList;

    private List<UpgradeObject> m_sortUpgradeList;
    private int[] m_indexTagUpgradeArray;
    private List<UpgradeObject[]> spellUpgradeArrayList = new List<UpgradeObject[]>();
    public int[] indexUpgrade;
    public UpgradeData.UpgradeSort upgradeSort;



    public CharacterUpgrade m_characterUpgradeComponent;
    [HideInInspector] public DropInventory m_dropInventory;
    [Header("UI Upgrade")]
    [Tooltip("Not neccessary")]
    public GameObject upgradeLevelUi;
    public GameObject spellChoiceUI;
    public GameObject upgradeBook;
    public Animator book_Animator;
    private UpgradeChoosing m_upgradeChoosingComponent;
    private ChooseSpellManager m_chooseSpellManagerComponent;

    private bool isUpgradeUILevel;

    private UpgradeLevelingData m_upgradeLevelingData;

    private UpgradeData.UpgradeTable m_upgradeData = new UpgradeData.UpgradeTable();

    public TMPro.TMP_Text levelCurrentSpell;
    public Image progressRang;
    public Image progressNextRang;

    public int rerollPoint = 3;

    public int countUpgradePointUse;


    public float[] percentForUpgradeMatchingElementRoom =  new float[4] { 100, 75, 50 ,25};
    public float percentForSpellMatchingElementRoom = 75;

    public void Awake()
    {

        upgradeSort = new UpgradeData.UpgradeSort();
        UpgradeData.UpgradeDataSort upgradeDataSort = upgradeSort.SortUpgrade(upgradeList);
        m_sortUpgradeList = new List<UpgradeObject>(upgradeDataSort.upgradeArray);
        m_indexTagUpgradeArray = upgradeDataSort.indexArray;
        string path = Application.dataPath + "\\Resources\\UpgradeTable.csv";
        LoadUpgradeTable(path);
        if (!upgradeLevelUi) return;

        for (int i = 0; i < upgradeList.Length; i++)
        {
            upgradeList[i] = upgradeList[i].Clone();
        }

        m_upgradeChoosingComponent = upgradeLevelUi.GetComponent<UpgradeChoosing>();
        m_upgradeChoosingComponent.m_upgradeManager = this;
        m_chooseSpellManagerComponent = spellChoiceUI.GetComponent<ChooseSpellManager>();
        m_chooseSpellManagerComponent.m_upgradeManagerComponenet = this;
        m_dropInventory = m_characterUpgradeComponent.GetComponent<DropInventory>();
    }

    public UpgradeObject[] RandomUpgrade(int count)
    {
        int[] index = GetRandomIndex(count, upgradeList.Length);
        UpgradeObject[] upgrades = new UpgradeObject[count];
        for (int i = 0; i < count; i++)
        {
            upgrades[i] = upgradeList[index[i]];
        }
        return upgrades;
    }

    public UpgradeObject GetRandomUpgradeToSpell(int indexSpell)
    {
        int index = m_upgradeData.RandomUpgradeSpell(indexSpell);

        UpgradeObject nxtProfil = upgradeList[index].Clone();
        return nxtProfil;
    }

    public UpgradeObject[] GetRandomUpgradesToSpecificSpell(int spellIndex, int indexSpellEquip, GameElement roomElement)
    {
        UpgradeObject[] upgradeGenerate = new UpgradeObject[upgradeGenerateCount];
        int level = m_characterUpgradeComponent.m_characterShoot.spellProfils[indexSpellEquip].level;
        if (level >= 12)
        {
            progressRang.fillAmount = 1;
            progressNextRang.fillAmount = 1;
        }
        else
        {
            int Tier = level / 4;
            float fillAmountPR = ((float)(level - (float)(Tier * 4)) / 4);
            float fillAmoutPNR = (float)((float)(level + 1 - (float)(Tier * 4)) / 4);
            progressRang.fillAmount = fillAmountPR;
            progressNextRang.fillAmount = fillAmoutPNR;
        }
        levelCurrentSpell.text = "Lv. " + level;

        for (int i = 0; i < upgradeGenerate.Length; i++)
        {
            UpgradeObject nxtProfil = ChooseUpgrade(indexSpellEquip);
            upgradeGenerate[i] = nxtProfil;
            upgradeGenerate[i].indexSpellLink = indexSpellEquip;
        }
        return upgradeGenerate;
    }

    public void UpdateCharacterUpgradePool()
    {
        SpellSystem.SpellProfil[] spellProfils = m_characterUpgradeComponent.m_characterShoot.spellProfils.ToArray();
        spellUpgradeArrayList.Clear();
        List<UpgradeObject> listAray = new List<UpgradeObject>();


        for (int i = 0; i < spellProfils.Length; i++)
        {

            listAray.Clear();
            int[] validTagValue = spellProfils[i].tagData.GetValidTag();

            int countTagValue = 0;
            for (int j = 0; j < validTagValue.Length; j++)
            {
                string spellTagString = Regex.Replace(((SpellTagOrder)j).ToString(), @"[\d-]", string.Empty);
                Type enumType = Type.GetType("GuerhoubaGames.GameEnum." + spellTagString.ToString());
                int myEnumMemberCount = enumType.GetEnumNames().Length - 1;
                if (validTagValue[j] <= 0)
                {
                    countTagValue += myEnumMemberCount;
                    countTagValue++;
                    continue;
                }
                else
                {
                    int listOffset = m_indexTagUpgradeArray[countTagValue];
                    int elementCount = m_indexTagUpgradeArray[countTagValue + validTagValue[j]];

                    for (int h = 1; h < validTagValue[j]; h++)
                    {
                        listOffset += m_indexTagUpgradeArray[countTagValue + h];
                    }


                    AddUpgradeToList(ref listAray, m_sortUpgradeList.GetRange(listOffset, elementCount).ToArray(), spellProfils[i]);

                    countTagValue += myEnumMemberCount;
                    countTagValue++;
                }




            }

            spellUpgradeArrayList.Add(listAray.ToArray());
        }
    }

    private void AddUpgradeToList(ref List<UpgradeObject> listAray, UpgradeObject[] listToAdd, SpellSystem.SpellProfil spellProfil)
    {
        for (int i = 0; i < listToAdd.Length; i++)
        {

            if (listToAdd[i].IsMultiTagUpgrade && !listToAdd[i].IsAllTagMatching(spellProfil))
            {
                continue;
            }

            if (!listAray.Contains(listToAdd[i]))
            {
                listAray.Add(listToAdd[i]);
            }
        }
    }



    private UpgradeObject ChooseUpgrade(int spellIndex)
    {
        UpgradeObject[] upgradeObject = spellUpgradeArrayList[spellIndex];
        int indexUpgrade = UnityEngine.Random.Range(0, upgradeObject.Length);
        return upgradeObject[indexUpgrade];
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

    public void OpenSpellChoiceUI(GameElement roomElement)
    {
        if (!spellChoiceUI) return;

        spellChoiceUI.SetActive(true);
        upgradeBook.SetActive(true);
        if (book_Animator != null) book_Animator.SetBool("BookOpen", true);
        m_chooseSpellManagerComponent.lastRoomElement = roomElement;
        m_chooseSpellManagerComponent.OpenSpellChoice();
        m_chooseSpellManagerComponent.ResetRandomSpell();
        GuerhoubaTools.LogSystem.LogMsg("Open Spell Choice interface");
    }

    public void SendSpell(SpellSystem.SpellProfil spellProfil)
    {
        m_characterUpgradeComponent.ApplySpellChoise(spellProfil);
        GuerhoubaTools.LogSystem.LogMsg("Spell Choose is" + spellProfil.name);
    }

    public void CloseSpellChoiceUI()
    {
        if (!spellChoiceUI) return;


        if (book_Animator != null) book_Animator.SetBool("BookOpen", false);
        float time = Time.time;
        float timeToClose = book_Animator.GetCurrentAnimatorStateInfo(0).length;

        StartCoroutine(CloseUIWithDelay(timeToClose));

    }

    #region Upgrade Level UI Functions
    public void OpenUpgradeUI(UpgradeLevelingData upgradeLevelingData)
    {
        if (!upgradeLevelUi) return;
        upgradeLevelUi.SetActive(true);
        upgradeBook.SetActive(true);
        if (book_Animator != null) book_Animator.SetBool("BookOpen", true);
        m_upgradeLevelingData.capsuleIndex = upgradeLevelingData.capsuleIndex;
        m_upgradeLevelingData.spellCount = upgradeLevelingData.spellCount;

        countUpgradePointUse = 0;
        int indexSpellEquip = GenerateSpellIndex(upgradeLevelingData.roomElement);
        int indexSpell = m_upgradeLevelingData.capsuleIndex[indexSpellEquip];
        m_upgradeLevelingData.upgradeChoose = GetRandomUpgradesToSpecificSpell(indexSpell, indexSpellEquip, upgradeLevelingData.roomElement);
        m_upgradeLevelingData.indexSpellFocus = indexSpellEquip;


        m_upgradeLevelingData.spellState = upgradeLevelingData.spellState;
        m_upgradeLevelingData.iconSpell = upgradeLevelingData.iconSpell;
        m_upgradeLevelingData.upgradePoint = upgradeLevelingData.upgradePoint;
        m_upgradeLevelingData.roomElement = upgradeLevelingData.roomElement;

        m_upgradeChoosingComponent.OpenUpgradeUI();
        m_upgradeChoosingComponent.SetNewUpgradeData(m_upgradeLevelingData);
        Debug.Log("Open Upgrade interface");
    }

    private int GenerateSpellIndex(GameElement element)
    {
        GameElement[] elements = m_characterUpgradeComponent.m_characterInventory.GetElementSpellInRotation();
        bool isOwnElement = false;

        List<int> indexElement = new List<int>();
        List<int> indexOtherElement = new List<int>();
        for (int i = 0; i < elements.Length; i++)
        {
            if (element == elements[i])
            {
                isOwnElement = true;
                indexElement.Add(i);
            }
            else
            {
                indexOtherElement.Add(i);
            }
        }

        if (!isOwnElement) return UnityEngine.Random.Range(0, m_upgradeLevelingData.spellCount);

        float percent = UnityEngine.Random.Range(0.0f, 100.0f);

        if (percent < percentForUpgradeMatchingElementRoom[countUpgradePointUse])
        {
            int indexSpell = UnityEngine.Random.Range(0, indexElement.Count);
            return indexElement[indexSpell];
        }
        else
        {
            int indexSpell = UnityEngine.Random.Range(0, indexOtherElement.Count);
            return indexOtherElement[indexSpell];
        }
    }

    public void UpdateUpgradeLevelingData(UpgradeLevelingData upgradeLevelingData)
    {
        m_upgradeLevelingData = upgradeLevelingData;
    }

    public void CloseUpgradeUI()
    {
        if (!upgradeLevelUi) return;

        if (book_Animator != null)
        {
            book_Animator.SetBool("BookOpen", false);
            float timeToClose = book_Animator.GetNextAnimatorStateInfo(0).length;
            StartCoroutine(CloseUIWithDelay(timeToClose));
        }

        Debug.Log("Close Upgrade interface");
    }

    public void SendUpgrade(UpgradeObject upgradeChoose)
    {


        m_characterUpgradeComponent.ApplyUpgrade(upgradeChoose);
        m_upgradeLevelingData.spellState = m_characterUpgradeComponent.m_characterShoot.spellProfils.ToArray();
        int indexSpellEquip = GenerateSpellIndex(m_upgradeLevelingData.roomElement);
        int indexSpell = m_upgradeLevelingData.capsuleIndex[indexSpellEquip];
        m_upgradeLevelingData.upgradeChoose = GetRandomUpgradesToSpecificSpell(indexSpell, indexSpellEquip, m_upgradeLevelingData.roomElement);
        m_upgradeLevelingData.indexSpellFocus = indexSpellEquip;
        m_upgradeLevelingData.upgradePoint--;
        countUpgradePointUse++;
        m_upgradeChoosingComponent.SetNewUpgradeData(m_upgradeLevelingData);

    }

    public void ReDrawUpgrade()
    {
        if (rerollPoint <= 0) return;
        int indexSpellEquip = GenerateSpellIndex(m_upgradeLevelingData.roomElement);
        int indexSpell = m_upgradeLevelingData.capsuleIndex[indexSpellEquip];
        m_upgradeLevelingData.upgradeChoose = GetRandomUpgradesToSpecificSpell(indexSpell, indexSpellEquip, m_upgradeLevelingData.roomElement);
        m_upgradeLevelingData.indexSpellFocus = indexSpellEquip;
        rerollPoint--;

        m_upgradeChoosingComponent.SetNewUpgradeData(m_upgradeLevelingData);
    }

    public IEnumerator CloseUIWithDelay(float time)
    {
        yield return new WaitForSeconds(time);
        upgradeBook.SetActive(false);
        spellChoiceUI.SetActive(false);
        upgradeLevelUi.SetActive(false);
        GuerhoubaTools.LogSystem.LogMsg("Close Spell Choice interface");
    }
    #endregion



}





namespace UpgradeData
{
    using System;
    using System.Text.RegularExpressions;

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

    public struct UpgradeDataSort
    {
        public int[] indexArray;
        public UpgradeObject[] upgradeArray;
    }


    public class UpgradeSort
    {
        public UpgradeDataSort SortUpgrade(UpgradeObject[] arrayUpgrade)
        {
            UpgradeObject[] tempArray = new UpgradeObject[arrayUpgrade.Length];
            List<UpgradeObject> upgradeList = new List<UpgradeObject>();
            int myEnumMemberCount = Enum.GetNames(typeof(SpellTagOrder)).Length;
            List<int> indexList = new List<int>();

            UpgradeDataSort upgradeDataSort = new UpgradeDataSort();
            for (int i = 0; i < myEnumMemberCount; i++)
            {
                upgradeDataSort = SortByEnum(arrayUpgrade, (SpellTagOrder)i);
                upgradeDataSort.indexArray[0] = upgradeList.Count;
                upgradeList.AddRange(upgradeDataSort.upgradeArray);
                indexList.AddRange(upgradeDataSort.indexArray);
            }


            UpgradeDataSort upgradeDataSortFinal = new UpgradeDataSort();
            upgradeDataSortFinal.upgradeArray = upgradeList.ToArray();
            upgradeDataSortFinal.indexArray = indexList.ToArray();
            return upgradeDataSortFinal;
        }

        public UpgradeDataSort SortByEnum(UpgradeObject[] arrayUpgrade, SpellTagOrder spellTagOrder)
        {
            string spellTagString = Regex.Replace(spellTagOrder.ToString(), @"[\d-]", string.Empty);

            Type enumType = Type.GetType("GuerhoubaGames.GameEnum." + spellTagString.ToString());
            int myEnumMemberCount = enumType.GetEnumNames().Length;
            string[] enumValue = enumType.GetEnumNames();

            int[] indexEnum = new int[myEnumMemberCount];
            List<UpgradeObject> tempArray = new List<UpgradeObject>();
            for (int i = 1; i < myEnumMemberCount; i++)
            {
                indexEnum[i] = 0;
                for (int j = 0; j < arrayUpgrade.Length; j++)
                {
                    if (enumValue[i] == arrayUpgrade[j].tagData.GetValueTag(spellTagOrder))
                    {
                        indexEnum[i]++;
                        tempArray.Add(arrayUpgrade[j]);
                    }
                }
            }

            UpgradeDataSort upgradeDataSort = new UpgradeDataSort();
            upgradeDataSort.upgradeArray = tempArray.ToArray();
            upgradeDataSort.indexArray = indexEnum;
            return upgradeDataSort;
        }

        public UpgradeDataSort SortByEnum<T>(UpgradeObject[] arrayUpgrade) where T : Enum
        {
            int myEnumMemberCount = Enum.GetNames(typeof(T)).Length;
            string[] enumValue = Enum.GetNames(typeof(T));

            int[] indexEnum = new int[myEnumMemberCount];
            List<UpgradeObject> tempArray = new List<UpgradeObject>();
            for (int i = 1; i < myEnumMemberCount; i++)
            {
                indexEnum[i] = 0;
                for (int j = 0; j < arrayUpgrade.Length; j++)
                {
                    if (enumValue[i] == arrayUpgrade[j].tagData.element.ToString())
                    {
                        indexEnum[i]++;
                        indexEnum[0]++;
                        tempArray.Add(arrayUpgrade[j]);
                    }
                }
            }

            UpgradeDataSort upgradeDataSort = new UpgradeDataSort();
            upgradeDataSort.upgradeArray = tempArray.ToArray();
            upgradeDataSort.indexArray = indexEnum;
            return upgradeDataSort;
        }
    }
}



