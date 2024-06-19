using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public UI_Fragment_Tooltip uiFragmentTooltip;
    private Vector3 positionRandom;
    public void Start()
    {
        m_characterShoot = GetComponent<Character.CharacterShoot>();
        m_healthComponent = GetComponent<HealthPlayerComponent>();
        positionRandom = Random.insideUnitSphere * rangeRandom;
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

    public void AddArtefact(ArtefactsInfos artefacts)
    {
        artefactsList.Add(artefacts);
        GenerateNewArtefactAround(artefacts);
        SetupArtefact(artefacts);
        uiFragmentTooltip.AddNewFragment(artefacts);
    }

    public void GenerateNewArtefactAround(ArtefactsInfos artefacts)
    {

        GameObject newArtefactAround = Instantiate(artefactAround_Prefab[(int)artefacts.gameElement], transform.position, transform.rotation);
        newArtefactAround.GetComponent<Klak.Motion.SmoothFollow>().target = targetObjectAround.transform;
        artefactAround_List.Add(newArtefactAround);

    }
    public void RemoveArtefact(int index)
    {
        artefactsList.RemoveAt(index);
    }


}
