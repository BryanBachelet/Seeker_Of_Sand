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

    private Collider[] m_lastArtefactCol;
    private int nearByArtefact = 0;

    public GameObject ui_HintInteractionObject;
    public HintInteractionManager m_hintInteractionManager;
    public void Start()
    {
        m_characterShoot = GetComponent<Character.CharacterShoot>();
        m_healthComponent = GetComponent<HealthPlayerComponent>();

        for (int i = 0; i < artefactsList.Count; i++)
        {
            SetupArtefact(artefactsList[i]);
        }
    }

    private void SetupArtefact(ArtefactsInfos artefacts)
    {
        if (activeDebug) Debug.Log("Artefacts " + artefacts.name + " setup");
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
        SetupArtefact(artefacts);
    }

    public void RemoveArtefact(int index)
    {
        artefactsList.RemoveAt(index);
    }

    
}
