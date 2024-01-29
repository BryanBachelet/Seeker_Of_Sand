using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Chosereward : MonoBehaviour
{
    public int ArtefactQuantity = 3;
    public int radiusDistribution;
    public GameObject piedestalReward;
    public GameObject[] artefactPrefab = new GameObject[4]; //0 --> Elec, 1 --> Eau, 2 --> Terre, 3 - Feu
    public List<ArtefactsInfos> artefactToChose = new List<ArtefactsInfos>();
    public List<Animator> artefactPiedestalAnimator = new List<Animator>();
    public List<ArtefactHolder> artefactHolder = new List<ArtefactHolder>();
    [HideInInspector] public List<ExperienceMouvement> artefactMouvement = new List<ExperienceMouvement>();
    private Transform m_playerTransform;

    [SerializeField] private GameObject m_lastPiedestal;

    public bool activeGeneration = true;
    public InteractionEvent interactionEvent;

    public Vector3[] positionArtefact = new Vector3[3];
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(activeGeneration)
        {
            activeGeneration = false;
            GetArtefactAttribution();
        }
    }

    public void GenerateNewArtefact(int index, int type)
    {
        Vector3 position = positionArtefact[index];
        //Vector3 position = Random.insideUnitSphere * (radiusDistribution + index * 10);
        m_lastPiedestal = Instantiate(piedestalReward, transform.position + new Vector3(position.x, -8, position.z), Quaternion.identity, transform);
        artefactPiedestalAnimator.Add(m_lastPiedestal.GetComponent<Animator>());
        GameObject newArtefact = Instantiate(artefactPrefab[type], transform.position + new Vector3(position.x, 8, position.z), transform.rotation, m_lastPiedestal.transform.Find("ArtefactContainer"));
        ExperienceMouvement m_ExperienceMouvement = newArtefact.GetComponent<ExperienceMouvement>();
        artefactMouvement.Add(m_ExperienceMouvement);
        ArtefactHolder m_artefactHolder = m_ExperienceMouvement.GetComponentInChildren<ArtefactHolder>();
        artefactHolder.Add(m_artefactHolder);
        m_artefactHolder.m_artefactsInfos = artefactToChose[type];
    }

    public void GetArtefactAttribution()
    {
        for(int i = 0; i < ArtefactQuantity-1; i++)
        {
            int rndArtefact = Random.Range(0, artefactToChose.Count);
            GenerateNewArtefact(i, (int)artefactToChose[rndArtefact].elementAffiliation);
        }
    }

    public void ChoseAnArtefact(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && interactionEvent.lastArtefact != null)
        {
            for (int i = 0; i < artefactToChose.Count; i++)
            {
                if(artefactToChose[i] == interactionEvent.lastArtefact)
                {
                    artefactMouvement[i].m_playerPosition = interactionEvent.gameObject.transform;
                }
                else
                {

                }
            }
        }
    }
}
