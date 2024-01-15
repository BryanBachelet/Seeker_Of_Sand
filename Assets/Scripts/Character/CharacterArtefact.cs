using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterArtefact : MonoBehaviour
{
    public List<ArtefactsInfos> artefactsList;
    public Enemies.EnemyManager m_enemyManager;

    public bool activeDebug = false;
    private Character.CharacterShoot m_characterShoot;

    public void Start()
    {
        m_characterShoot = GetComponent<Character.CharacterShoot>();

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
                m_characterShoot.onHit += artefacts.ActiveArtefact;
                break;
            case ConditionsTrigger.OnDeath:
                break;
            default:
                break;
        }
    }

    public void AddArtefact(ArtefactsInfos artefacts)
    {
        artefactsList.Add(artefacts);
    }

    public void RemoveArtefact(int index)
    {
        artefactsList.RemoveAt(index);
    }


}
