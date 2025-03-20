using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using System.IO;
using Character;
public class Experience_System : MonoBehaviour, CharacterComponent
{
    [SerializeField] private AnimationCurve m_ExperienceQuantity;
    [SerializeField] private AnimationCurve m_ExperienceQuantityControl;
    [SerializeField] private AnimationCurve m_ExperienceQuantitySansCSV;
    [SerializeField] private float m_NumberEnemyKilled = 0;
    [SerializeField] private int m_CurrentLevel = 1;
    [SerializeField] public int m_LevelTaken = 1;

    [SerializeField] public Image m_LevelDisplayFill;
    [SerializeField] private float m_RadiusPickupXp;
    [SerializeField] private bool m_ActiveGizmo;
    [SerializeField] private LayerMask m_ExperienceLayer;

    [SerializeField] private RectTransform m_xpPointer;
    [SerializeField] private VisualEffect levelUpEffect;
    [SerializeField] public VisualEffect levelUpEffectUi;
    [SerializeField] private SerieController m_serieController;


    private List<ExperienceMouvement> m_worldExp = new List<ExperienceMouvement>();
    private CharacterUpgrade m_characterUpgrade;
    private CharacterProfile m_characterProfile;
    private CristalInventory m_cristalInventory;
    private HealthPlayerComponent m_healthPlayer;
    [HideInInspector] public SeekerOfSand.UI.UI_PlayerInfos m_UiPlayerInfo;

    private bool m_xperienceBuffered = false;
    private float lastXpBuffered = 0;
    private float levelProgress;


#if UNITY_EDITOR
    [HideInInspector] public bool cancelGainExperience;
#endif

    // Start is called before the first frame update
    void Start()
    {
        m_characterUpgrade = GetComponent<CharacterUpgrade>();
        m_characterProfile = GetComponent<CharacterProfile>();
        m_cristalInventory = GetComponent<CristalInventory>();
        m_healthPlayer = GetComponent<HealthPlayerComponent>();
        m_UiPlayerInfo = GameObject.Find("UI_Manager").GetComponent<SeekerOfSand.UI.UI_PlayerInfos>();
        TestReadDataSheet();
    }

    public void InitComponentStat(CharacterStat stat)
    {
        m_RadiusPickupXp = stat.attrackness;
    }

    // Update is called once per frame
    void Update()
    {
        #region oldLoot reward
        /*
        Collider[] experienceTouched = Physics.OverlapSphere(transform.position, m_RadiusPickupXp, m_ExperienceLayer);
        if (experienceTouched.Length > 0 && TerrainGenerator.staticRoomManager.isRoomHasBeenValidate)
        {
            for (int i = 0; i < experienceTouched.Length; i++)
            {
                ExperienceMouvement xpMvtScript = experienceTouched[i].GetComponent<ExperienceMouvement>();
                if(xpMvtScript.GetComponent<RewardTypologie>())
                {
                    RewardTypologie rewardTypo = xpMvtScript.GetComponent<RewardTypologie>();
                    rewardTypo.ActivationDistribution();
                }
                xpMvtScript.ActiveExperienceParticule(this.transform);
                m_worldExp.Remove(xpMvtScript);
            }
        }
        */
        #endregion
        if (m_xperienceBuffered)
        {
            BufferXpDisplay(lastXpBuffered, levelProgress);
        }
    }

    public void OnEnemyKilledNew(int quantity)
    {
#if UNITY_EDITOR
        //if (cancelGainExperience) return;
#endif
        float time = Time.time;

        m_NumberEnemyKilled += (quantity * m_serieController.GetXpMultiplicator());
        levelProgress = m_ExperienceQuantitySansCSV.Evaluate(m_CurrentLevel);
        if (m_NumberEnemyKilled > levelProgress)
        {
            m_CurrentLevel += 1;
            GetUpgradePoint(m_CurrentLevel);
            m_NumberEnemyKilled = 0;
            levelUpEffect.Play();
            GlobalSoundManager.PlayOneShot(7, Vector3.zero);
        }
        else
        {
            //Debug.Log("Progression is : " + (levelProgress - m_CurrentLevel) + "%");
            lastXpBuffered = time;
            m_xperienceBuffered = true;
            //m_xpPointer.anchoredPosition = new Vector3(Mathf.Lerp(m_posXInit, m_posXFinal, (levelProgress - m_CurrentLevel)), -520, 0);
        }
    }


    public int GetCurrentLevel() { return m_CurrentLevel; }
    // Add xp particule to the list
    public void AddExpParticule(ExperienceMouvement xpMvtScript)
    {
        m_worldExp.Add(xpMvtScript);
    }

    public void ActiveExpHarvest()
    {
        ExperienceMouvement[] expArray = m_worldExp.ToArray();
        for (int i = 0; i < expArray.Length; i++)
        {
            if (expArray[i]) expArray[i].ActiveExperienceParticule(this.transform);
        }

        m_worldExp.Clear();
    }

    public void GetUpgradePoint(int level)
    {
        //Debug.Log("Add new upgrade : " + level);
        m_characterUpgrade.GainLevel();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, m_RadiusPickupXp);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Experience")
        {
            ExperienceMouvement lastXpDrop = collision.GetComponent<ExperienceMouvement>();
            if (!lastXpDrop.m_destruction)
            {
                lastXpDrop.InitDestruction();
                //Destroy(collision.gameObject);
                GlobalSoundManager.PlayOneShot(3, Vector3.zero);
                //OnEnemyKilled(lastXpDrop.quantity);
                OnEnemyKilledNew(lastXpDrop.quantity);
            }

        }
        else if (collision.gameObject.tag == "CristalDrop")
        {
            if(m_cristalInventory == null) m_cristalInventory = GetComponent<CristalInventory>();
            m_cristalInventory.AddCristalCount(collision.GetComponent<ExperienceMouvement>().cristalType, 1);
            Destroy(collision.gameObject);
            GlobalSoundManager.PlayOneShot(3, Vector3.zero);
        }
        else if (collision.gameObject.tag == "HealDrop")
        {
            Destroy(collision.gameObject);
            m_healthPlayer.RestoreQuarter();
            GlobalSoundManager.PlayOneShot(3, Vector3.zero);
        }

    }

    private void UpdateMagnet(ref CharacterStat playerStat)
    {
        m_RadiusPickupXp = playerStat.attrackness;
    }

    static string ReadSpecificLine(string filePath, int lineNumber)
    {
        string content = null;

        using (StreamReader file = new StreamReader(filePath))
        {
            for (int i = 1; i < lineNumber; i++)
            {
                file.ReadLine();

                if (file.EndOfStream)
                {
                    //Console.WriteLine($"End of file.  The file only contains {i} lines.");
                    break;
                }
            }
            content = file.ReadLine();
        }
        return content;

    }

    private void BufferXpDisplay(float time, float levelProgress)
    {
        m_LevelDisplayFill.fillAmount = Mathf.Lerp(m_LevelDisplayFill.fillAmount, m_NumberEnemyKilled / levelProgress, Time.time - time);

    }

    public void TestReadDataSheet()
    {
        AnimationCurve tempAnimationCurve = new AnimationCurve();
        string debugdata = "";
#if UNITY_EDITOR
        string filePath = Application.dataPath + "\\Game data use\\Progression Demo - SpawnSheet.csv";

#else
#if UNITY_STANDALONE_WIN
        string filePath = Application.dataPath + "\\Progression Demo -SpawnSheet.csv";
        Debug.LogError("Is Right path");
#endif
#endif
        int lineNumber = 17;

        string lineContents = ReadSpecificLine(filePath, lineNumber);
        string[] data_values = lineContents.Split(',');
        long[] dataTransformed = new long[data_values.Length - 1];
        for (int i = 0; i < dataTransformed.Length; i++)
        {
            //Debug.Log(i + " : Job done");
            if (data_values[i] == "") continue;
            dataTransformed[i] = long.Parse(data_values[i]);
            tempAnimationCurve.AddKey(dataTransformed[i], i);
            debugdata = debugdata + " , " + dataTransformed[i];

        }


        m_ExperienceQuantityControl = tempAnimationCurve;
        //  Debug.Log(debugdata);

    }
}
