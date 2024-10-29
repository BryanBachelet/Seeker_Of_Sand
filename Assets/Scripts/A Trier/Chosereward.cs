using GuerhoubaGames.GameEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
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
    public List<VisualEffect> vfxArtefact = new List<VisualEffect>();
    public Transform m_playerTransform;
    private InteractionEvent m_interactionEventScript;

    [SerializeField] private GameObject m_lastPiedestal;

    public bool activeGeneration = true;
    public InteractionEvent interactionEvent;

    public Vector3[] positionArtefact = new Vector3[3];

    public bool isStart = true;
    public GameObject colliderDome;

    public int[] m_artefactIndex =new int[3];
    public bool IsDebugActive=false;
    // Start is called before the first frame update
    void Start()
    {
        m_artefactIndex = new int[3];
        for (int i = 0; i < m_artefactIndex.Length; i++)
        {
            m_artefactIndex[i] = -1;
        }
        if (m_playerTransform == null) { m_playerTransform = GameObject.Find("Player").transform; }
        m_interactionEventScript = m_playerTransform.GetComponent<InteractionEvent>();
        if (isStart)
        {
            isStart = false;
            DayCyclecontroller.choosingArtefactStart = false;
            colliderDome.SetActive(false);
        }
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
        GameObject newArtefact = Instantiate(artefactPrefab[(int)artefactToChose[type].gameElement], transform.position + new Vector3(position.x, 8, position.z), transform.rotation, m_lastPiedestal.transform.Find("ArtefactContainer"));
        ExperienceMouvement m_ExperienceMouvement = newArtefact.GetComponent<ExperienceMouvement>();
        artefactMouvement.Add(m_ExperienceMouvement);
        ArtefactHolder m_artefactHolder = m_ExperienceMouvement.GetComponentInChildren<ArtefactHolder>();
        artefactHolder.Add(m_artefactHolder);
        m_artefactHolder.m_artefactsInfos = artefactToChose[type];
        VisualEffect vfx = m_ExperienceMouvement.GetComponentInChildren<VisualEffect>();
        vfxArtefact.Add(vfx);
       if(IsDebugActive) Debug.Log("Artefact (" + index + ") is type (" + m_artefactHolder.m_artefactsInfos.gameElement.ToString() + ") and is named " + m_artefactHolder.m_artefactsInfos.nameArtefact);
    }

    public void GetArtefactAttribution()
    {
        ClearArtefact();
        for (int i = 0; i < ArtefactQuantity-1; i++)
        {
            int rndArtefact = -1;
            for (int j = 0; j < i; j++)
            {
                while (rndArtefact == m_artefactIndex[j] && rndArtefact == -1)  
                {
                    rndArtefact = Random.Range(0, artefactToChose.Count);
                }
                m_artefactIndex[i] = rndArtefact;
            }
            if (rndArtefact == -1) 
                rndArtefact = Random.Range(0, artefactToChose.Count);
            GenerateNewArtefact(i, rndArtefact);
        }
    }

    public void ChoseAnArtefact(InputAction.CallbackContext ctx)
    {
        return;
        if (ctx.performed && interactionEvent.lastArtefact != null)
        {
            for (int i = 0; i < artefactHolder.Count; i++)
            {
                if(artefactHolder[i] == interactionEvent.lastArtefact)
                {
                    artefactMouvement[i].m_playerPosition = m_playerTransform;
                    interactionEvent.StartCoroutine(interactionEvent.CloseUIWithDelay(2));
                    StartCoroutine(ChosedArtefact(i, 30));

                }
                else
                {
                    StartCoroutine(ChosedArtefact(i, 3));
                }
            }
            if(isStart)
            {
                isStart = false;
                DayCyclecontroller.choosingArtefactStart = false;
                colliderDome.SetActive(false);
            }
        }
    }

    public void GenerateNewArtefactReward(Transform positionAltar)
    {
        int rndArtefact = Random.Range(0, artefactToChose.Count);
        Vector3 position = positionAltar.position;
        //Vector3 position = Random.insideUnitSphere * (radiusDistribution + index * 10);
        GameObject newArtefact = Instantiate(artefactPrefab[(int)artefactToChose[rndArtefact].gameElement], position, transform.rotation, positionAltar);
        ExperienceMouvement m_ExperienceMouvement = newArtefact.GetComponent<ExperienceMouvement>();
        ArtefactHolder m_artefactHolder = m_ExperienceMouvement.GetComponentInChildren<ArtefactHolder>();
        VisualEffect vfx = m_ExperienceMouvement.GetComponentInChildren<VisualEffect>();
        m_ExperienceMouvement.m_playerPosition = m_playerTransform;
        m_artefactHolder.m_artefactsInfos = artefactToChose[rndArtefact];
        //interactionEvent.StartCoroutine(interactionEvent.CloseUIWithDelay(2));
        //StartCoroutine(ChosedArtefact(i, 30));
        if (IsDebugActive)
        {
            Debug.Log("Artefact (" + ") is type (" + m_artefactHolder.m_artefactsInfos.gameElement.ToString() + ") and is named " + m_artefactHolder.m_artefactsInfos.nameArtefact);
            Debug.Log(" Artefact N°" + rndArtefact);
        }
    }

    public void GiveArtefact(GameElement gameElement)
    {
        int rndArtefact = Random.Range(0, artefactToChose.Count);

        m_playerTransform.GetComponent<CharacterArtefact>().AddArtefact(artefactToChose[rndArtefact]);
        m_playerTransform.GetComponent<DropInventory>().AddNewArtefact(artefactToChose[rndArtefact]);
    }

    public IEnumerator ChosedArtefact(int artefactToClear, float timeBeforeDestroy)
    {

        artefactPiedestalAnimator[artefactToClear].SetBool("Choosed", true);
        m_interactionEventScript.m_lastHintAnimator.SetTrigger("ActiveLoot");
        if(m_interactionEventScript.m_lastArtefactAnimator != null)
        {
            m_interactionEventScript.m_lastArtefactAnimator.transform.parent = null;
            m_interactionEventScript.m_lastArtefactAnimator.gameObject.AddComponent<DestroyAfterBasic>();
            m_interactionEventScript.m_lastArtefactAnimator.gameObject.GetComponent<DestroyAfterBasic>().m_DestroyAfterTime = 6;
            m_interactionEventScript.m_lastArtefactAnimator.gameObject.GetComponent<ArtefactEffect>().ActiveEffect();
            m_interactionEventScript.m_lastArtefactAnimator.enabled = false;
        }
        yield return new WaitForSeconds(timeBeforeDestroy);
        Destroy(artefactPiedestalAnimator[artefactToClear].gameObject);
    }
    public void ClearArtefact()
    {
        for (int i = 0; i < m_artefactIndex.Length; i++)
        {
            m_artefactIndex[i] = -1;
        }
        artefactPiedestalAnimator.Clear();
        artefactHolder.Clear();
        artefactMouvement.Clear();
    }
}
