using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
public class AltarBehaviorComponent : MonoBehaviour
{
    [Header("Event Parameters")]
    [Range(0, 3)]
    [SerializeField] int eventElementType = 0;
    [SerializeField] private Color[] colorEvent;
    [SerializeField] private Material[] materialEvent;
    [ColorUsage(showAlpha: true, hdr: true)]
    [SerializeField] private Color[] colorEventTab;
    [SerializeField] private float m_TimeInvulnerability;
    [SerializeField] private float m_MaxKillEnemies;
    [SerializeField] private int m_CurrentKillCount;
    [SerializeField] private GameObject[] DangerAddition;
    public float radiusEventActivePlayer = 300;
    public float radiusEjection;
    public int rangeEvent = 100;

    [Header("Reward Parameters")]
    [SerializeField] private int XpQuantity = 100;
    [SerializeField] private GameObject[] rewardObject;
    [SerializeField] private float m_ImpusleForceXp;


    private bool m_hasEventActivate = true;
    private bool m_isEventOccuring;


    private Text displayTextDescription1;
    private Text displayTextDescription2;
    private Text eventTextName;

    [Header("Event UI Parameters")]
    public GameObject displayEventDetail;
    public GameObject displayArrowEvent;
    private GameObject ownDisplayEventDetail;
    private GameObject ownArrowDisplayEventDetail;

    public Animator m_myAnimator;
    public EventDisplay displayAnimator;

    private RectTransform canvasPrincipal;

    static int altarCount = 0;
    private int ownNumber = 0;
    private Color myColor;
    private Transform m_playerTransform;

    public VisualEffect m_visualEffectActivation;
    public string txt_EventName;
    int resetNumber = 0;

    public SkinnedMeshRenderer[] altarAllMesh;
    public MeshRenderer socleMesh;

    public Material socleMaterial;
    [HideInInspector] public bool isAltarDestroy = false;

    private ObjectHealthSystem m_objectHealthSystem;
    public Image m_eventProgressionSlider;

    [SerializeField] private string instructionOnActivation;
    [SerializeField] private int nextReward;
    [SerializeField] private int nextRewardTypologie = 0;

    [SerializeField] private Enemies.EnemyManager m_enemyManager;
    public Vector3 m_DropAreaPosition;

    private GameObject lastItemInstantiate;
    private GameObject nextRewardObject;
    [SerializeField] private LayerMask m_groundLayer;

    private int m_idSpellReward;
    private int m_enemiesCountConditionToWin = 0;

    #region Unity Functions
    void Start()
    {
        InitComponent();

        eventElementType = Random.Range(0, 4);

        ownNumber = altarCount;
        altarCount++;

        m_playerTransform = m_enemyManager.m_playerTranform;

        // Setup the altar health
        float maxHealth = 50 + m_enemyManager.m_maxUnittotal;
        m_objectHealthSystem.SetMaxHealth((int)maxHealth);
        m_objectHealthSystem.ResetCurrentHealth();

        // Setup the drop point position
        RaycastHit hit = new RaycastHit();
        Vector3 raycastdirection = new Vector3(0, -10, 0);
        if (Physics.Raycast(transform.position + new Vector3(0, 25, 0), raycastdirection, out hit, Mathf.Infinity, m_groundLayer))
        {
            m_DropAreaPosition = hit.point;
        }

        InitVisualElements();

        StartCoroutine(LastStart());
    }

    void Update()
    {

        if (!m_isEventOccuring) return;

        if (m_enemiesCountConditionToWin <= m_CurrentKillCount && m_objectHealthSystem.IsEventActive())
        {

            SucceedEvent();
            return;
        }

        m_eventProgressionSlider.fillAmount = m_CurrentKillCount / m_enemiesCountConditionToWin; // Update event UI

        if (DestroyCondition())
        {
            DestroyAltar();
        }
    }

    #endregion

    #region Init Functions
    private void InitComponent()
    {
        m_objectHealthSystem = GetComponent<ObjectHealthSystem>();
        m_enemyManager = GameObject.Find("Enemy Manager").GetComponent<Enemies.EnemyManager>();
    }


    private void InitVisualElements()
    {
        Light[] lightToSwap = GetComponentsInChildren<Light>();
        for (int i = 0; i < lightToSwap.Length; i++)
        {
            lightToSwap[i].color = colorEvent[eventElementType];
        }

        socleMesh.material = materialEvent[eventElementType];

        m_visualEffectActivation.GetComponentInChildren<VisualEffect>();
        m_visualEffectActivation.SetVector4("ColorEvent", colorEvent[eventElementType]);


        socleMesh.material.SetColor("_SelfLitColor", colorEventTab[eventElementType]);
        for (int i = 0; i < altarAllMesh.Length; i++)
        {
            altarAllMesh[i].material.SetColor("_SelfLitColor", colorEventTab[eventElementType]);
        }
    }


    public IEnumerator LastStart()
    {
        yield return new WaitForSeconds(2.0f);
        m_idSpellReward = CapsuleManager.GetRandomCapsuleIndex();
        GenerateNextReward(resetNumber);
    }
    #endregion

    // Update is called once per frame

    #region Destroy Functions

    private bool DestroyCondition()
    {
        bool distanceTest = m_objectHealthSystem.IsEventActive() && Vector3.Distance(m_playerTransform.position, transform.position) > radiusEventActivePlayer;
        bool stateTest = m_objectHealthSystem.eventState == EventObjectState.Death;

        if (distanceTest || stateTest)
            return true;

        return false;
    }

    private void DestroyAltar()
    {
        m_objectHealthSystem.ChangeState(EventObjectState.Deactive);
        m_enemyManager.SendInstruction("Altar protection fail...", Color.red, TerrainLocationID.currentLocationName);
        m_myAnimator.SetBool("ActiveEvent", false);
        m_hasEventActivate = true;
        m_isEventOccuring = false;
        isAltarDestroy = true;
        m_enemyManager.RemoveTarget(transform);
        m_enemyManager.RemoveAltar(transform);
        Debug.Log("Destroy event");
    }
    #endregion

 
    public void IncreaseKillCount()
    {
        m_CurrentKillCount++;
    }

    #region State Altar Functions

    // Need to set active
    public void ActiveEvent()
    {
        if (!m_objectHealthSystem.IsEventActive())
        {
            m_enemiesCountConditionToWin = (int)(25 * (resetNumber + 1) + (m_enemyManager.m_maxUnittotal * 0.25f));

            m_enemyManager.AddTarget(this.transform);
            m_enemyManager.AddAltar(transform);
            m_enemyManager.SendInstruction(instructionOnActivation + " [Repeat(+" + resetNumber + ")]", Color.white, TerrainLocationID.currentLocationName);

            m_myAnimator.SetBool("ActiveEvent", true);

            if (resetNumber == 0)
            {
                m_myAnimator.SetTrigger("Activation");
            }
            if (resetNumber - 2 >= 0)
            {
                lastItemInstantiate = Instantiate(DangerAddition[resetNumber -2], transform.position, transform.rotation);
            }
            

            SetMeshesEventIntensity(0.15f * (resetNumber + 1));
            m_visualEffectActivation.Play();
            
            GlobalSoundManager.PlayOneShot(13, transform.position);

            m_objectHealthSystem.ChangeState(EventObjectState.Active);

            m_hasEventActivate = false;
            m_isEventOccuring = true;
        }
        else
        {
            Debug.Log("Cette objet [" + this.name + "] ne peut pas être activé");
        }
    }


    private void SucceedEvent()
    {
        m_isEventOccuring = false;

        m_myAnimator.SetBool("ActiveEvent", false);
        m_myAnimator.SetBool("IsDone", true);

        // Update Enemies Manager  ==> Create One functions from the enemyManager
        m_enemyManager.altarSuccessed++;
        m_enemyManager.RemoveTarget(transform);
        m_enemyManager.RemoveAltar(transform);
        m_enemyManager.SendInstruction("Event succeed ! Gain [" + nextReward + "] (" + nextRewardTypologie + ")", Color.green, TerrainLocationID.currentLocationName);

        // Update altar mesh color
        SetMeshesEventIntensity(.32f * (resetNumber + 1));

        GlobalSoundManager.PlayOneShot(14, transform.position);

        SpawnAltarReward();

        StartCoroutine(ResetEventWithDelay(1.5f));

        m_objectHealthSystem.ChangeState(EventObjectState.Deactive);

        if (lastItemInstantiate != null)
            Destroy(lastItemInstantiate);
    }

    public IEnumerator ResetEventWithDelay(float time)
    {
        yield return new WaitForSeconds(time);
        ResetAltarEvent();
    }

    public void ResetAltarEvent()
    {
       
        resetNumber++;
        GenerateNextReward(resetNumber);

        m_hasEventActivate = true;
        m_isEventOccuring = false;

        m_CurrentKillCount = 0;
        
        float maxHealth = 50 + m_enemyManager.m_maxUnittotal;
        
        m_objectHealthSystem.SetMaxHealth((int)maxHealth);
        m_objectHealthSystem.ResetCurrentHealth();

    }

    #endregion 

    #region Reward Functions

    public void SpawnAltarReward()
    {
        for (int i = 0; i < nextReward; i++)
        {
            Vector3 randomRadiusPosition = new Vector3(Random.Range(-radiusEjection, radiusEjection), 0, Random.Range(-radiusEjection, radiusEjection));

            GameObject rewardObject = Instantiate(nextRewardObject, transform.position, Quaternion.identity);

            if (nextRewardTypologie == 2)
                rewardObject.GetComponent<CapsuleContainer>().capsuleIndex = m_idSpellReward;

            ExperienceMouvement expMouvementComponent = rewardObject.GetComponent<ExperienceMouvement>();
            expMouvementComponent.GroundPosition = m_DropAreaPosition + randomRadiusPosition;
            StartCoroutine(expMouvementComponent.MoveToGround());
        }

    }

    public string[] GetAltarData()
    {
        int RewardTypologie = nextRewardTypologie; // 0 = Cristal element; 1 = Experience quantité; 2 = Specific spell; 3 = Health quarter
        string[] dataToSend = new string[4];
        dataToSend[0] = RewardTypologie.ToString();
        dataToSend[1] = instructionOnActivation;
        if (RewardTypologie == 0)
        {
            dataToSend[2] = nextReward.ToString();
            dataToSend[3] = eventElementType.ToString();
        }
        else if (RewardTypologie == 1)
        {
            dataToSend[2] = nextReward.ToString();
            dataToSend[3] = "123"; //1234 = Aucun element car la récompense est de l'experience
        }
        else if (RewardTypologie == 2)
        {
            dataToSend[2] = "-1"; // -1 = Aucune quantité particulière car la récompense est unique
            dataToSend[3] = m_idSpellReward.ToString(); // A remplacé par une variable randomisé au démarrage
        }
        /*  else if (RewardTypologie == 3)
          {
              dataToSend[2] = "-1"; // -1 = valeur par defaut signifiant 1 quarter de vie. Pourrait etre augmenté sous condition spécifique
              dataToSend[3] = "ID de la ressource (vie, mana, autre ?) associé au gain"; 
          }*/


        return dataToSend;
    }

    public void GenerateNextReward(int repeatNumber)
    {
        if (repeatNumber == 0)
        {
            nextRewardTypologie = 2;
            nextRewardObject = rewardObject[6];
            nextReward = 1;
        }
        else if (repeatNumber == 1)
        {
            nextRewardTypologie = 1;
            nextReward = (int)(100 + Time.timeSinceLevelLoad);
            nextRewardObject = rewardObject[0];
        }
        else if (repeatNumber == 2)
        {
            nextRewardTypologie = 0;
            nextReward = 30;
            nextRewardObject = rewardObject[1 + eventElementType];
        }
        else if (repeatNumber == 3)
        {
            nextRewardTypologie = 3;
            nextReward = 1;
            nextRewardObject = rewardObject[5];
        }
        else if (repeatNumber == 4)
        {
            nextRewardTypologie = 0;
            nextReward = 70;
            nextRewardObject = rewardObject[1 + eventElementType];
        }

    }


    #endregion

    #region Visual Functions

    private void SetMeshesEventIntensity(float intensity)
    {
        socleMesh.material.SetFloat("_SelfLitIntensity", intensity);
        for (int i = 0; i < altarAllMesh.Length; i++)
        {
            altarAllMesh[i].material.SetFloat("_SelfLitIntensity", intensity);
        }
    }
    #endregion


    #region Debug Functions
    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radiusEjection);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangeEvent);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, radiusEventActivePlayer);
    }
    #endregion

}
