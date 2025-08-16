using GuerhoubaGames;
using GuerhoubaGames.Enemies;
using GuerhoubaGames.GameEnum;
using GuerhoubaGames.UI;
using SeekerOfSand.Tools;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

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
    public VisualEffect m_distortionWave;
    int resetNumber = 0;

    public SkinnedMeshRenderer[] altarAllMesh;
    public MeshRenderer socleMesh;
    private Material matEvent;

    [HideInInspector] public bool isAltarDestroy = false;

    private ObjectHealthSystem m_objectHealthSystem;
    [HideInInspector] public Image m_eventProgressionSlider;

    [SerializeField] private string instructionOnActivation;
    private int nextReward;
    private int nextRewardTypologie = 0;

    private EnemyManager m_enemyManager;
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
    public Collider sphereCollider;

    [Header("Wave Variables")]
    public int groupSize;
    public int lostLifeToSpawnWave = 25;
    private bool m_isWaveCanSpawn = true;
    #endregion Variable

    public int indexEvent = 0;
    private int currentQuarter;
    public GameObject corruptedRoot;
    public AnimationCurve corruptedRootScale;

    public Action OnEventEnd;

    [Header("Debug Altar")]
    public bool isFastEvent = false;

    #region Unity Functions
    void Start()
    {

    }

    public void LaunchInit()
    {
        InitComponent();
        this.enabled = true;
        ownNumber = altarCount;
        altarCount++;


        m_playerTransform = m_enemyManager.m_playerTranform;

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

        if (!m_objectHealthSystem.IsEventActive())
        {
            SucceedEvent();
            return;
        }
        if ((int)(m_objectHealthSystem.healthSystem.health) / lostLifeToSpawnWave != currentQuarter)
        {
            if (m_isWaveCanSpawn)
            {
                Vector2 rndPosition = UnityEngine.Random.insideUnitSphere * 350;
                m_enemyManager.SpawEnemiesGroupCustom(transform.position + new Vector3(rndPosition.x, 0, rndPosition.y), groupSize);
                m_enemyManager.ActiveMobAggro();
                m_distortionWave.SendEvent("Activation");
                m_isWaveCanSpawn = false;
                currentQuarter--;
            }
        }
        else
        {
            m_isWaveCanSpawn = true;
        }



        progression = 1.0f - m_objectHealthSystem.healthSystem.percentHealth;
        //m_eventProgressionSlider.fillAmount = progression;
        roomInfoUI.ActualizeMajorGoalProgress(progression);
        //corruptedRoot.transform.localScale = Vector3.one * (corruptedRootScale.Evaluate(progression));
        roomInfoUI.UpdateTextProgression((int)m_objectHealthSystem.healthSystem.maxHealth - (int)m_objectHealthSystem.healthSystem.health, (int)m_objectHealthSystem.healthSystem.maxHealth);

    }

    #endregion

    #region Init Functions
    private void InitComponent()
    {
        m_objectHealthSystem = GetComponent<ObjectHealthSystem>();
        m_objectHealthSystem.animatorAssociated = m_myAnimator;
        m_enemyManager = GameObject.FindAnyObjectByType<EnemyManager>();
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

        //socleMesh.material = materialEvent[(int)eventElementType];

        m_visualEffectActivation.GetComponentInChildren<VisualEffect>();
        m_visualEffectActivation.SetVector4("ColorEvent", colorEvent[(int)eventElementType]);


        //socleMesh.material.SetColor("_SelfLitColor", colorEventTab[(int)eventElementType]);
        for (int i = 0; i < altarAllMesh.Length; i++)
        {
            if (i == 1)
            {
                matEvent = altarAllMesh[i].material;
            }

            altarAllMesh[i].material.SetColor("_SelfLitColor", colorEventTab[(int)eventElementType]);
        }
        m_objectHealthSystem._matAssociated = matEvent;
    }


    public IEnumerator LastStart()
    {
        yield return new WaitForSeconds(2.0f);
        m_idSpellReward = SpellManager.GetRandomSpellIndex();
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

        isInteractable = false;
        hasBeenActivate = true;

        m_interactionEvent = GameState.s_playerGo.GetComponent<InteractionEvent>();
        m_objectHealthSystem.Setup((int)m_objectHealthSystem.maxHealthEvolution.Evaluate(m_enemyManager.m_characterUpgrade.avatarUpgradeList.Count));
        lostLifeToSpawnWave = (int)(m_objectHealthSystem.healthSystem.maxHealth / 3);


        m_enemyManager.ActiveEvent(transform);


        sphereCollider.enabled = true;


        if (!isFastEvent)
        {
            m_enemyManager.SpawEnemiesGroupCustom(transform.position, groupSize);
        }
        m_isWaveCanSpawn = false;
        currentQuarter = (int)(m_objectHealthSystem.healthSystem.health) / lostLifeToSpawnWave;
        progression = 0;

        m_myAnimator.SetBool("ActiveEvent", true);
        m_myAnimator.SetBool("Reset", false);
        m_selectionFeedback.ChangeLayerToDefault();
        if (resetNumber == 0)
        {
            m_myAnimator.SetTrigger("Activation");
        }
        int altarDone = transform.parent.GetComponentInChildren<RoomManager>().CheckEventNumber();
        int nightCount = m_enemyManager.m_dayController.m_nightCount;
        if (altarDone >= 0)
        {
            //lastItemInstantiate = Instantiate(eventHolder.DangerAddition[alatarDone], transform.position, transform.rotation);
            //TrainingArea area = lastItemInstantiate.GetComponent<TrainingArea>();
            //area.altarAssociated = this.gameObject;
            //area.element = eventElementType;
            //lastItemInstantiate.SetActive(true);
        }
        AltarAttackComponent altarAttackComponent = GetComponent<AltarAttackComponent>();
        altarAttackComponent.ActivateAltarAttack(eventHolder.GetAltarAttackData(eventElementType, altarDone), m_myAnimator.transform.position);
        eventHolder.SpawnAreaVFX(eventElementType, transform.position);

        SetMeshesEventIntensity(0.33f * (1 + 1));
        //  m_visualEffectActivation.Play();
        m_distortionWave.SendEvent("Activation");

        GlobalSoundManager.PlayOneShot(13, transform.position);

        roomInfoUI.ActiveMajorGoalInterface();
        m_objectHealthSystem.ChangeState(EventObjectState.Active);

        m_hasEventActivate = false;
        m_isEventOccuring = true;

        OnEventEnd += RunManager.instance.RemoveHourPoint;

        if (isFastEvent)
        {
            SucceedEvent();
        }

    }


    private void SucceedEvent()
    {
        sphereCollider.enabled = false;
        roomInfoUI.DeactivateMajorGoalInterface();

        m_isEventOccuring = false;
        m_myAnimator.SetTrigger("FinishOnce");
        m_myAnimator.SetTrigger("Repetition");
        progression = 0;


        m_myAnimator.SetBool("ActiveEvent", false);
        m_myAnimator.SetBool("IsDone", true);
        m_interactionEvent.currentInteractibleObjectActive = null;

        // Update Enemies Manager  ==> Create One functions from the enemyManager
        m_enemyManager.altarSuccessed++;
        m_enemyManager.DeactiveEvent(transform);
        // Update altar mesh color
        SetMeshesEventIntensity(.32f * (resetNumber + 1));

        GlobalSoundManager.PlayOneShot(14, transform.position);
        AltarAttackComponent altarAttackComponent = GetComponent<AltarAttackComponent>();
        altarAttackComponent.DeactivateAltarAttack();
        eventHolder.ActiveEndEvent();

        OnEventEnd?.Invoke();
        OnEventEnd -= RunManager.instance.RemoveHourPoint;

        int rewardIndex = transform.parent.GetComponentInChildren<RoomManager>().EventValidate();
        SpawnAltarReward(rewardIndex);

        m_objectHealthSystem.ChangeState(EventObjectState.Deactive);



        if (lastItemInstantiate != null)
            Destroy(lastItemInstantiate);
    }


    public void ResetAltarEvent()
    {
        resetNumber++;

        isInteractable = true;
        m_hasEventActivate = false;
        m_isEventOccuring = false;
        hasBeenActivate = false;
        m_myAnimator.SetBool("ActiveEvent", false);
        m_myAnimator.SetBool("IsDone", false);
        m_myAnimator.SetBool("Reset", true);
        isOpen = false;

        m_objectHealthSystem.ResetCurrentHealth();
    }

    #endregion 

    #region Reward Functions

    public void SpawnAltarReward(int indexReward)
    {
        RunManager.SpawnReward((RewardType)(indexReward), piedestalTranformPosition.position, eventElementType);
    }

    #endregion

    #region Visual Functions

    private void SetMeshesEventIntensity(float intensity)
    {
        //socleMesh.material.SetFloat("_SelfLitIntensity", intensity);
        for (int i = 0; i < altarAllMesh.Length; i++)
        {
            altarAllMesh[i].material.SetColor("_SelfLitColor", colorEventTab[GeneralTools.GetElementalArrayIndex(eventElementType, true)]);
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
        return;
    }




}
