using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using GuerhoubaGames.UI;
using GuerhoubaGames.GameEnum;
using SeekerOfSand.Tools;

public class AltarBehaviorComponent : InteractionInterface
{
    #region Variable
    [Header("Event Parameters")]
    [SerializeField] public GameElement eventElementType = 0;
    [SerializeField] private Color[] colorEvent;
    [SerializeField] private Material[] materialEvent;
    [SerializeField] private Sprite[] spriteEventCompass;
    [ColorUsage(showAlpha: true, hdr: true)]
    [SerializeField] private Color[] colorEventTab;
    [SerializeField] private float m_TimeInvulnerability;
    [SerializeField] private float m_MaxKillEnemies;
    [SerializeField] private int m_CurrentKillCount;
    [SerializeField] private GameObject[] DangerAddition;
    [HideInInspector] public Chosereward rewardManagerReference;
    [HideInInspector] public float radiusEventActivePlayer = 300;
    [HideInInspector] public float radiusEjection;
    public int rangeEvent = 600;

    [Header("Reward Parameters")]

    [SerializeField] private GameObject[] rewardObject;
    private float m_ImpusleForceXp = 20;


    private bool m_hasEventActivate = true;
    private bool m_isEventOccuring;


    private Text displayTextDescription1;
    private Text displayTextDescription2;
    private Text eventTextName;

    [Header("Event UI Parameters")]
    public Animator m_myAnimator;

    static int altarCount = 0;
    private int ownNumber = 0;

    private Transform m_playerTransform;

    public VisualEffect m_visualEffectActivation;
    int resetNumber = 0;

    public SkinnedMeshRenderer[] altarAllMesh;
    public MeshRenderer socleMesh;

    [HideInInspector] public bool isAltarDestroy = false;

    private ObjectHealthSystem m_objectHealthSystem;
    [HideInInspector] public Image m_eventProgressionSlider;

    [SerializeField] private string instructionOnActivation;
    private int nextReward;
    private int nextRewardTypologie = 0;

    private Enemies.EnemyManager m_enemyManager;
    private Vector3 m_DropAreaPosition;

    private GameObject lastItemInstantiate;
    private GameObject nextRewardObject;
    [SerializeField] private LayerMask m_groundLayer;
    private Vector3 raycastdirection;

    private float progression = 0;

    private int m_idSpellReward;
    public int m_enemiesCountConditionToWin = 0;
    public Sprite instructionImage;
    private InteractionEvent m_interactionEvent;

    [HideInInspector] public EventHolder eventHolder;

    [HideInInspector] public RoomInfoUI roomInfoUI;

    public bool hasBeenActivate = false;
    private Selection_Feedback m_selectionFeedback;

    [SerializeField] private Light eventLight;
    [SerializeField] private MeshRenderer meshPointLight;

    public Transform piedestalTranformPosition;

    #endregion Variable

    #region Unity Functions
    void Start()
    {
        InitComponent();
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
        if (eventHolder == null)
        {
            eventHolder = EventHolder.GetInstance();
            eventHolder.GetNewAltar(this);
        }

    }

    void Update()
    {

        if (!m_isEventOccuring) return;

        if (m_enemiesCountConditionToWin <= m_CurrentKillCount && m_objectHealthSystem.IsEventActive())
        {
            SucceedEvent();
            return;
        }
        

        progression = (float)m_CurrentKillCount / (float)m_enemiesCountConditionToWin;
        //m_eventProgressionSlider.fillAmount = progression;
        roomInfoUI.ActualizeMajorGoalProgress(progression);
        roomInfoUI.UpdateTextProgression(m_enemiesCountConditionToWin - m_CurrentKillCount, m_enemiesCountConditionToWin);
        //Debug.Log("Progression : " + progression + "(" + this.name + ")");

        //m_eventProgressionSlider.fillAmount = progression; // Update event UI

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
        m_enemyManager = GameObject.FindAnyObjectByType<Enemies.EnemyManager>();
        m_selectionFeedback = this.GetComponent<Selection_Feedback>();
    }


    private void InitVisualElements()
    {
        Light[] lightToSwap = GetComponentsInChildren<Light>();
        for (int i = 0; i < lightToSwap.Length; i++)
        {
            lightToSwap[i].color = colorEvent[(int)eventElementType];
        }
        eventLight.color = colorEvent[(int)eventElementType];
        meshPointLight.material.SetColor("_MainColor", colorEventTab[(int)eventElementType]);

        socleMesh.material = materialEvent[(int)eventElementType];

        m_visualEffectActivation.GetComponentInChildren<VisualEffect>();
        m_visualEffectActivation.SetVector4("ColorEvent", colorEvent[(int)eventElementType]);


        socleMesh.material.SetColor("_SelfLitColor", colorEventTab[(int)eventElementType]);
        for (int i = 0; i < altarAllMesh.Length; i++)
        {
            altarAllMesh[i].material.SetColor("_SelfLitColor", colorEventTab[(int)eventElementType]);
        }
    }


    public IEnumerator LastStart()
    {
        yield return new WaitForSeconds(2.0f);
        m_idSpellReward = SpellManager.GetRandomSpellIndex();
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
        m_enemyManager.SendInstruction("Altar protection fail...", Color.red, instructionImage);
        m_myAnimator.SetBool("ActiveEvent", false);
        m_hasEventActivate = true;
        m_isEventOccuring = false;
        isAltarDestroy = true;
        m_interactionEvent.currentInteractibleObjectActive = null;
        m_enemyManager.DeactiveEvent(transform);
        GuerhoubaTools.LogSystem.LogMsg("Destroy Altar");
    }
    #endregion


    public void IncreaseKillCount()
    {
        m_CurrentKillCount++;
    }

    #region State Altar Functions

    public void ResetAltar()
    {
        resetNumber = 0;
        m_objectHealthSystem.ChangeState(EventObjectState.Deactive);
        m_myAnimator.ResetTrigger("Activation");
        m_myAnimator.Play("New State");
    }

    // Need to set active
    public void ActiveEvent()
    {
        if (hasBeenActivate) return;

        hasBeenActivate = true;

        m_interactionEvent = GameState.s_playerGo.GetComponent<InteractionEvent>();


        //m_enemiesCountConditionToWin = (int)(25 * (resetNumber + 1) + (m_enemyManager.m_maxUnittotal * 0.25f));
        m_enemyManager.ActiveEvent(transform);

        m_enemyManager.SendInstruction(instructionOnActivation + " [Repeat(+" + resetNumber + ")]", Color.white, instructionImage);
        progression = 0;

        m_myAnimator.SetBool("ActiveEvent", true);
        m_selectionFeedback.ChangeLayerToDefault();
        if (resetNumber == 0)
        {
            m_myAnimator.SetTrigger("Activation");
        }
        int nightCount = m_enemyManager.m_dayController.m_nightCount;
        if (nightCount >= 0)
        {
            lastItemInstantiate = Instantiate(eventHolder.DangerAddition[nightCount], transform.position, transform.rotation);
            TrainingArea area = lastItemInstantiate.GetComponent<TrainingArea>();
            area.altarAssociated = this.gameObject;
            area.element = eventElementType;
            lastItemInstantiate.SetActive(true);
        }


        SetMeshesEventIntensity(0.33f * (1 + 1));
        m_visualEffectActivation.Play();

        GlobalSoundManager.PlayOneShot(13, transform.position);

        roomInfoUI.ActiveMajorGoalInterface();
        m_objectHealthSystem.ChangeState(EventObjectState.Active);

        m_hasEventActivate = false;
        m_isEventOccuring = true;

    }


    private void SucceedEvent()
    {
        m_isEventOccuring = false;
        m_myAnimator.SetTrigger("FinishOnce");
        m_myAnimator.SetTrigger("Repetition");
        progression = 0;
        //ObjectifAndReward_Ui_Function.StopEventDisplay();
        m_myAnimator.SetBool("ActiveEvent", false);
        m_myAnimator.SetBool("IsDone", true);
        m_interactionEvent.currentInteractibleObjectActive = null;
        // Update Enemies Manager  ==> Create One functions from the enemyManager
        m_enemyManager.altarSuccessed++;
        m_enemyManager.SendInstruction("Event succeed ! Gain [" + nextReward + "] (" + nextRewardTypologie + ")", Color.green, instructionImage);
        m_enemyManager.DeactiveEvent(transform);
        // Update altar mesh color
        SetMeshesEventIntensity(.32f * (resetNumber + 1));

        GlobalSoundManager.PlayOneShot(14, transform.position);

        SpawnAltarReward();

        StartCoroutine(ResetEventWithDelay(1.5f));

        m_objectHealthSystem.ChangeState(EventObjectState.Deactive);

        transform.parent.GetComponentInChildren<RoomManager>().ValidateRoom();

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
        float maxHealth = 100;

        m_objectHealthSystem.SetMaxHealth((int)maxHealth);
        m_objectHealthSystem.ResetCurrentHealth();

    }

    #endregion 

    #region Reward Functions

    public void SpawnAltarReward()
    {
        RewardDistribution rewardDistributionComponent =  m_playerTransform.GetComponent<RewardDistribution>();
        int indexReward = Random.Range(0, 4);
        rewardDistributionComponent.GiveReward((RewardType)(indexReward), piedestalTranformPosition, HealthReward.QUARTER, eventElementType);

        return;
        //for (int i = 0; i < nextReward; i++)
        //{
        //    Vector3 randomRadiusPosition = new Vector3(Random.Range(-radiusEjection, radiusEjection), 0, Random.Range(-radiusEjection, radiusEjection));

        //    GameObject rewardObject = Instantiate(nextRewardObject, transform.position, Quaternion.identity, this.transform);

        //    if (nextRewardTypologie == 2)
        //    {
        //        //rewardObject.GetComponent<CapsuleContainer>().capsuleIndex = m_idSpellReward;
        //        if (rewardManagerReference) rewardManagerReference.GenerateNewArtefactReward(this.transform);

        //    }

        //    ExperienceMouvement expMouvementComponent = rewardObject.GetComponent<ExperienceMouvement>();
        //    expMouvementComponent.ActiveExperienceParticule(m_playerTransform);
        //    expMouvementComponent.GroundPosition = m_DropAreaPosition + randomRadiusPosition;
        //    StartCoroutine(expMouvementComponent.MoveToGround());
        //}

    }

    public string[] GetAltarData()
    {
        int RewardTypologie = nextRewardTypologie; // 0 = Cristal element; 1 = Experience quantit�; 2 = Specific spell; 3 = Health quarter
        string[] dataToSend = new string[5];
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
            dataToSend[3] = "123"; //1234 = Aucun element car la r�compense est de l'experience
        }
        else if (RewardTypologie == 2)
        {
            dataToSend[2] = "-1"; // -1 = Aucune quantit� particuli�re car la r�compense est unique
            dataToSend[3] = m_idSpellReward.ToString(); // A remplac� par une variable randomis� au d�marrage
        }
        /*  else if (RewardTypologie == 3)
          {
              dataToSend[2] = "-1"; // -1 = valeur par defaut signifiant 1 quarter de vie. Pourrait etre augment� sous condition sp�cifique
              dataToSend[3] = "ID de la ressource (vie, mana, autre ?) associ� au gain"; 
          }*/
        dataToSend[4] = "" + progression;

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
            nextRewardObject = rewardObject[(int)eventElementType];
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
            nextRewardObject = rewardObject[(int)eventElementType];
        }

    }

    public void GetDamage(int damage)
    {

    }

    #endregion

    #region Visual Functions

    private void SetMeshesEventIntensity(float intensity)
    {
        socleMesh.material.SetFloat("_SelfLitIntensity", intensity);
        for (int i = 0; i < altarAllMesh.Length; i++)
        {
            altarAllMesh[i].material.SetColor("_SelfLitColor", colorEventTab[GeneralTools.GetElementalArrayIndex(eventElementType,true)]);
            altarAllMesh[i].material.SetFloat("_SelfLitIntensity", intensity);
        }
        eventLight.color = colorEvent[GeneralTools.GetElementalArrayIndex(eventElementType, true)];
        meshPointLight.material.SetColor("_MainColor", colorEvent[GeneralTools.GetElementalArrayIndex(eventElementType, true)]);
    }
    #endregion


    public override void OnInteractionStart(GameObject player)
    {
        ActiveEvent();

    }

    public override void OnInteractionEnd(GameObject player)
    {

    }

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
