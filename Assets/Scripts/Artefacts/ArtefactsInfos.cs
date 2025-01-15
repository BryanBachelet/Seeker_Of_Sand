using GuerhoubaGames.GameEnum;
using UnityEngine;
using GuerhoubaGames.Resources;
using System.Collections.Generic;
using SeekerOfSand.Tools;
using GuerhoubaGames;
using UnityEngine.VFX;

public enum ConditionsTrigger
{
    OnHit,
    OnDeath,
    Contact,
    StatGeneral,
}

public enum EntitiesTrigger
{
    Enemies,
    Self,
    Decor,
}


public enum EntitiesTargetSystem
{
    EnemyHit,
    ClosestEnemyAround,
    EnemyRandomAround,
}

[System.Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Artefacts Info", order = 1)]
public class ArtefactsInfos : ScriptableObject
{
    [CustomArrayName("Tier")]
    [Range(0, 100)]
    public float[] spawnRatePerTier;

    public float radius = 30;
    public GameElement gameElement;
    public ConditionsTrigger conditionsTrigger;
    public EntitiesTrigger entitiesTrigger;
    public EntitiesTargetSystem entitiesTargetSystem;

    public GameObject m_artefactToSpawn;
    public GameObject m_artefactProjectile;
    [HideInInspector] public GameObject characterGo;
    public Sprite icon;
    [TextArea]
    public string baseDescription;
    public string nameArtefact;
    public bool isDebugActive = false;

    [TextArea]
    public string descriptionResult;

    public VisualEffect effectActivation;
    public string description
    {
        get
        {
            ResultString();
            return descriptionResult;
        }
    }


    [HideInInspector] public int activationCount;

    private int proc = 0;
    public int maxProc = 10;
    private float lastTimeRefresh = 0;

    public int damageArtefact = 1;


    [Header("StatGeneral Features")]
    public DamageType damageTypeBonus;

    // Reinforcement Variables
    [Header("ReInforce Feature Variables")]
    public bool isReinforceFeatureActivate = true;
    [HideInInspector] public int additionialItemCount = 0;
    public int damageGainPerCount = 1;

    [Header("Upgrade Feature Variables")]
    public bool isUpgradeFeactureActive;
    public LevelTier levelTierFragment;

    [Header("Merge Feature Variables")]
    [SerializeField] private bool m_ActiveMergeFeature = true;
    private bool m_HasBeenMergeOnce = false;
    private List<GameObject> additionalEffectToSpawn = new List<GameObject>();
    public int idFamily = 0;

    [HideInInspector]
    public int damageToApply
    {
        get { return damageArtefact + damageGainPerCount * additionialItemCount; } 
    }

    public ArtefactsInfos Clone()
    {
        ArtefactsInfos clone = Instantiate(this);
        return clone;
    }

    public bool IsSameFragment(ArtefactsInfos artefactInfos)
    {
        if (artefactInfos.nameArtefact != nameArtefact || !isReinforceFeatureActivate)
        {
            return false;
        }
        return true;
    }

    public bool AddAdditionalFragment(ArtefactsInfos artefactInfos)
    {
        if (artefactInfos.nameArtefact != nameArtefact || !isReinforceFeatureActivate)
        {
            return false;
        }

        additionialItemCount++;
        ResultString();
        return true;
    }

    public bool MergeFragment(ArtefactsInfos artefactsInfos)
    {
        if (!m_ActiveMergeFeature || idFamily != artefactsInfos.idFamily)
        {
            return false;
        }

        // TODO : Change of name and description for the merge mechanics

        // Change of element
        gameElement = gameElement | artefactsInfos.gameElement;
        // Add Additional Object to spanw
        additionalEffectToSpawn.Add(artefactsInfos.m_artefactToSpawn);
        // Change boolean 
        m_HasBeenMergeOnce = true;



        return true;
    }

    public bool UpgradeTierFragment()
    {
        if (!isUpgradeFeactureActive)
        {
            Debug.LogWarning("Tier Upgrade feature isn't active");
            return false;
        }
        levelTierFragment++;
        levelTierFragment = (LevelTier)Mathf.Clamp((int)levelTierFragment, 0, spawnRatePerTier.Length - 1);
        ResultString();
        return true;
    }

    public void ActiveArtefactOnHit(Vector3 position, EntitiesTrigger tag, GameObject objectPre, GameElement element)
    {

        if (entitiesTrigger != tag) return;

         if(conditionsTrigger == ConditionsTrigger.OnHit) 
            if (!GeneralTools.IsThisElementPresent(gameElement, element) || element == GameElement.NONE) 
                return;


        float change = Random.Range(0, 100.0f);
        float spawnRateValue = spawnRatePerTier[(int)levelTierFragment];
        if (change < spawnRateValue)
        {
            if (isDebugActive)
            {
                Debug.Log("Artefact active OnHit");
            }
            if (!CanApplyOnHit()) return;
            GameObject obj = GamePullingSystem.SpawnObject(m_artefactToSpawn, position, Quaternion.identity);
            Artefact.ArtefactData artefactData = obj.GetComponent<Artefact.ArtefactData>();
            SetupArtefactData(artefactData, objectPre);
            artefactData.OnSpawn?.Invoke();
            activationCount++;
            effectActivation.Play();
            if (!m_HasBeenMergeOnce) return;
            for (int i = 0; i < additionalEffectToSpawn.Count; i++)
            {
                GameObject objEffect = GamePullingSystem.SpawnObject(additionalEffectToSpawn[i], position, Quaternion.identity);
                artefactData = objEffect.GetComponent<Artefact.ArtefactData>();
                SetupArtefactData(artefactData, objectPre);
                artefactData.OnSpawn?.Invoke();
            }
        }

    }

    public void ActiveArtefactOnDeath(Vector3 position, EntitiesTrigger tag, GameObject agent, float distance)
    {
        if (isDebugActive) Debug.Log("Artefact not in range");

        if (entitiesTrigger != tag || distance > radius) return;

        if (isDebugActive) Debug.Log("Artefact in range");

        float change = Random.Range(0, 100);
        float spawnRateValue = spawnRatePerTier[(int)levelTierFragment];
        if (change < spawnRateValue)
        {
            if (isDebugActive) Debug.Log("Artefact in launch");
            GameObject obj = GamePullingSystem.SpawnObject(m_artefactToSpawn, position, Quaternion.identity);
            Artefact.ArtefactData artefactData = obj.GetComponent<Artefact.ArtefactData>();
            SetupArtefactData(artefactData, agent);
            artefactData.OnSpawn?.Invoke();
            activationCount++;
            effectActivation.Play();
            if (!m_HasBeenMergeOnce) return;
            for (int i = 0; i < additionalEffectToSpawn.Count; i++)
            {
                GameObject objEffect = GamePullingSystem.SpawnObject(additionalEffectToSpawn[i], position, Quaternion.identity);
                artefactData = objEffect.GetComponent<Artefact.ArtefactData>();
                SetupArtefactData(artefactData, agent);
                artefactData.OnSpawn?.Invoke();
            }
        }
    }

    public void SetupArtefactData(Artefact.ArtefactData artefactData, GameObject agent)
    {
        artefactData.agent = agent;
        artefactData.entitiesTargetSystem = entitiesTargetSystem;
        artefactData.characterGo = characterGo;
        artefactData.radius = radius;
        artefactData.nameArtefact = nameArtefact;
        artefactData.elementIndex = (int)gameElement;
        artefactData.element = gameElement;
        artefactData.damageToApply = damageToApply;
    }

    public bool CanApplyOnHit()
    {
        bool usable = false;
        if (Time.time >= lastTimeRefresh + 0.1f)
        {
            lastTimeRefresh = Time.time;
            proc = 0;
        }
        if (proc < maxProc)
        {
            proc++;
            usable = true;
        }
        else
        {
            usable = false;
        }


        return usable;
    }

    public void ResultString()
    {
        string result = string.Empty;

        if (baseDescription == null)
        {
            descriptionResult = baseDescription;
            return;
        }
            string[] bracketArray = baseDescription.Split("{");
        if (bracketArray == null)
        {
            descriptionResult = baseDescription;
            return;
        }

        int countBracket = bracketArray.Length-1;
        int indexEndString = 0;

        if (countBracket <= 0)
        {
            descriptionResult = baseDescription;
            return;
        }
        int indexStart = baseDescription.IndexOf("{");


        for (int i = 0; i < countBracket; i++)
        {


            int indexStartProperty = baseDescription.IndexOf("{", indexEndString);
            if (indexStartProperty == -1)
            {

                descriptionResult = baseDescription;
                return;

            }
            int indexEndProperty = baseDescription.IndexOf("}", indexEndString +1);
            if (indexEndProperty == -1)
            {
                Debug.LogError("Artefact string missing }");
                descriptionResult = baseDescription.Substring(0, indexStartProperty);
                return;
            }

            if (indexEndProperty - indexStartProperty < 0)
            {
                Debug.LogError("Artefact string missing {");
                descriptionResult = baseDescription;
                return;
            }

            result += baseDescription.Substring(indexEndString, indexStartProperty -indexEndString);
            indexEndString = indexEndProperty+1;
            string substringCode = baseDescription.Substring(indexStartProperty + 1, indexEndProperty - indexStartProperty - 1); 
            if (substringCode == "probability")
            {
                result += spawnRatePerTier[(int)levelTierFragment].ToString();

                continue;
            }

            if (substringCode == "damage")
            {
                result += (damageArtefact + damageGainPerCount * additionialItemCount).ToString();
                continue;
            }

          

        }
        result += baseDescription.Substring(indexEndString);
        descriptionResult = result;
        return;
    }

    public void OnValidate()
    {


        ResultString();
    }


}
