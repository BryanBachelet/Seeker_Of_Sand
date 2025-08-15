using GuerhoubaGames.Character;
using GuerhoubaGames.GameEnum;
using GuerhoubaGames.Resources;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class Chosereward : MonoBehaviour
{
    private GameResources m_gameRessources;
    public int ArtefactQuantity = 3;
    public GameObject piedestalReward;
    public List<ArtefactsInfos> artefactToChose = new List<ArtefactsInfos>();
    [HideInInspector] private List<Animator> artefactPiedestalAnimator = new List<Animator>();
    [HideInInspector] private List<ArtefactHolder> artefactHolder = new List<ArtefactHolder>();
    [HideInInspector] private List<ExperienceMouvement> artefactMouvement = new List<ExperienceMouvement>();
    [HideInInspector] private List<VisualEffect> vfxArtefact = new List<VisualEffect>();
    [HideInInspector] private Transform m_playerTransform;
    [HideInInspector] private InteractionEvent m_interactionEventScript;

    [HideInInspector] private GameObject m_lastPiedestal;

    [HideInInspector] public bool activeGeneration = true;

    public Vector3[] positionArtefact = new Vector3[3];

    [HideInInspector] private bool isStart = true;

    [HideInInspector] private int[] m_artefactIndex =new int[3];
    public bool IsDebugActive=false;

    [SerializeField] private GameObject chooseFragmentUI;
    [HideInInspector] private FragmentChoiceUI m_fragmentChoose;
    // Start is called before the first frame update
    void Start()
    {
        m_gameRessources = GameObject.Find("General_Manager").GetComponent<GameResources>();
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
        }
        m_fragmentChoose = chooseFragmentUI.GetComponent<FragmentChoiceUI>();
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
        GameObject newArtefact = Instantiate(m_gameRessources.artefactPrefab[(int)artefactToChose[type].gameElement], transform.position + new Vector3(position.x, 8, position.z), transform.rotation, m_lastPiedestal.transform.Find("ArtefactContainer"));
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

    public void GenerateNewArtefactReward(Transform positionAltar)
    {
        int rndArtefact = Random.Range(0, artefactToChose.Count);
        Vector3 position = positionAltar.position;
        //Vector3 position = Random.insideUnitSphere * (radiusDistribution + index * 10);
        GameObject newArtefact = Instantiate(m_gameRessources.artefactPrefab[(int)artefactToChose[rndArtefact].gameElement], position, transform.rotation, positionAltar);
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
        chooseFragmentUI.SetActive(true);
        List<ArtefactsInfos> temp_artefactToChose = new List<ArtefactsInfos>(artefactToChose);
        int rndArtefact = 0;
        ArtefactsInfos artefactInfo = artefactToChose[rndArtefact];
        for (int i = 0; i < artefactToChose.Count;)
        {
            rndArtefact = Random.Range(0, temp_artefactToChose.Count);

            if (temp_artefactToChose[rndArtefact].gameElement != gameElement)
            {
                i++;
                temp_artefactToChose.RemoveAt(rndArtefact);
            }
            else
            {
                i = artefactToChose.Count + 1;
                artefactInfo = temp_artefactToChose[rndArtefact];
            }
        }

        m_playerTransform.GetComponent<CharacterArtefact>().AddArtefact(artefactInfo);
        m_playerTransform.GetComponent<DropInventory>().AddNewArtefact(artefactInfo);
    }
    public void GiveArtefactChoice(GameElement gameElement)
    {
        m_fragmentChoose.gameObject.SetActive(true);
        List<ArtefactsInfos> temp_artefactToChose = new List<ArtefactsInfos>(artefactToChose);
        int rndArtefact1 = Random.Range(0, temp_artefactToChose.Count);
        int rndArtefact2 = Random.Range(0, temp_artefactToChose.Count);
        ArtefactsInfos[] artefactInfoTemps = new ArtefactsInfos[] { artefactToChose[rndArtefact1], artefactToChose[rndArtefact2] };

        m_fragmentChoose.SetFragmentInfo(artefactInfoTemps);

        //m_playerTransform.GetComponent<CharacterArtefact>().AddArtefact(artefactInfo);
        //m_playerTransform.GetComponent<DropInventory>().AddNewArtefact(artefactInfo);
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
