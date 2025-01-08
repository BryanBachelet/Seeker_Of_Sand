using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaTools;
using GuerhoubaGames.Resources;
using SeekerOfSand.Tools;

public class CharacterArtefact : MonoBehaviour
{
    public List<ArtefactsInfos> artefactsList;
    public Enemies.EnemyManager m_enemyManager;

    public bool activeDebug = false;
    private Character.CharacterShoot m_characterShoot;
    private HealthPlayerComponent m_healthComponent;

    public int radiusDetectionArtefact;
    public List<ArtefactHolder> nearArtefactHolder = new List<ArtefactHolder>();

    public GameObject ui_HintInteractionObject;
    public HintInteractionManager m_hintInteractionManager;

    public GameObject[] artefactAround_Prefab;
    public GameObject targetObjectAround;
    public List<GameObject> artefactAround_List = new List<GameObject>();

    public float rangeRandom = 5;

    private int pullingQuantity = 100;

    public UI_Fragment_Tooltip uiFragmentTooltip;
    private Vector3 positionRandom;

    public void Start()
    {
        m_characterShoot = GetComponent<Character.CharacterShoot>();
        m_healthComponent = GetComponent<HealthPlayerComponent>();
        positionRandom = Random.insideUnitSphere * rangeRandom;

        List<ArtefactsInfos> cloneList = new List<ArtefactsInfos>(artefactsList.ToArray());
        artefactsList.Clear();
        for (int i = 0; i < cloneList.Count; i++)
        {
            AddArtefact(cloneList[i]);
        }

        for (int i = 0; i < artefactsList.Count; i++)
        {
            SetupArtefact(artefactsList[i]);
        }
    }

    private void SetupArtefact(ArtefactsInfos artefacts)
    {

        artefacts.isDebugActive = activeDebug;
        switch (artefacts.conditionsTrigger)
        {
            case ConditionsTrigger.OnHit:
                m_characterShoot.onHit += artefacts.ActiveArtefactOnHit;
                break;
            case ConditionsTrigger.OnDeath:
                m_enemyManager.OnDeathEvent += artefacts.ActiveArtefactOnDeath;
                break;
            case ConditionsTrigger.Contact:
                m_healthComponent.OnContactEvent += artefacts.ActiveArtefactOnHit;
                break;
            default:
                break;
        }

        artefacts.characterGo = gameObject;

    }

    private void UnSetupArtefact(ArtefactsInfos artefacts)
    {
        artefacts.isDebugActive = activeDebug;
        switch (artefacts.conditionsTrigger)
        {
            case ConditionsTrigger.OnHit:
                m_characterShoot.onHit -= artefacts.ActiveArtefactOnHit;
                break;
            case ConditionsTrigger.OnDeath:
                m_enemyManager.OnDeathEvent -= artefacts.ActiveArtefactOnDeath;
                break;
            case ConditionsTrigger.Contact:
                m_healthComponent.OnContactEvent -= artefacts.ActiveArtefactOnHit;
                break;
            default:
                break;
        }

        artefacts.characterGo = gameObject;
    }


    public ArtefactsInfos[] GetMostDamageArtefactInfo(int count)
    {

        ArtefactsInfos[] artefactsInfosArray;
        if (artefactsList.Count <= count)
        {
            artefactsInfosArray = artefactsList.ToArray();
            return artefactsInfosArray;
        }

        artefactsInfosArray = new ArtefactsInfos[count];
        int[] artefactDamage = new int[count];

        for (int i = 0; i < artefactsList.Count; i++)
        {
            if (!GameStats.instance.IsContains(artefactsList[i].nameArtefact))
                continue;

            int damageOccur = GameStats.instance.GetDamage(artefactsList[0].nameArtefact);

            for (int j = 0; j < artefactsInfosArray.Length; j++)
            {
                if (damageOccur > artefactDamage[j])
                {
                    artefactsInfosArray[j] = artefactsList[i];
                    artefactDamage[j] = damageOccur;
                    break;
                }
            }
        }

        return artefactsInfosArray;
    }

    public void AddArtefact(ArtefactsInfos artefacts)
    {
        
        // Verify if the player doesn't already the artefact
        for (int i = 0; i < artefactsList.Count; i++)
        {
            if (artefactsList[i].AddAdditionalFragment(artefacts))
            {
                uiFragmentTooltip.UpdateFragmentStack(i, artefactsList[i].additionialItemCount +1);
                return;
            }
        }

        ArtefactsInfos clone = artefacts.Clone();

        artefactsList.Add(clone);

        CreatePull(clone);

        LogSystem.LogMsg("Artefact add  is " + clone.name);
        GenerateNewArtefactAround(clone);
        SetupArtefact(clone);
        uiFragmentTooltip.AddNewFragment(clone);
    }

    public void CreatePull(ArtefactsInfos instance)
    {
        PullConstructionData pullConstructionData = new PullConstructionData(instance.m_artefactToSpawn, pullingQuantity);

        if (GamePullingSystem.instance == null) return;
        GamePullingSystem.instance.CreatePull(pullConstructionData);

        if(instance.m_artefactProjectile != null)
        {
            PullConstructionData pullConstructionData1 = new PullConstructionData(instance.m_artefactProjectile, pullingQuantity);
            if (GamePullingSystem.instance == null) return;
            GamePullingSystem.instance.CreatePull(pullConstructionData1);

        }

    }

    public void GenerateNewArtefactAround(ArtefactsInfos artefacts)
    {
        GameObject newArtefactAround = Instantiate(artefactAround_Prefab[GeneralTools.GetElementalArrayIndex( artefacts.gameElement,true)], transform.position, transform.rotation);
        newArtefactAround.GetComponent<Klak.Motion.SmoothFollow>().target = targetObjectAround.transform;
        artefactAround_List.Add(newArtefactAround);

    }

    public void RemoveFragment(int indexFragmentAround)
    {
        artefactAround_List.RemoveAt(indexFragmentAround);
    }
    public void RemoveArtefact(int index)
    {
        artefactsList.RemoveAt(index);
        uiFragmentTooltip.RemoveFragment(index);
    }


    public void RemoveArtefact(ArtefactsInfos artefactsInfos)
    {
        int indexTargetArtefact = artefactsList.IndexOf(artefactsInfos);
        uiFragmentTooltip.RemoveFragment(indexTargetArtefact);
        RemoveFragment(indexTargetArtefact);

        UnSetupArtefact(artefactsInfos);
        artefactsList.Remove(artefactsInfos);
    }

}
