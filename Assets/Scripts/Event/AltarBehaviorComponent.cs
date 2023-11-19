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
    [SerializeField] private float m_MaxHealth;
    [SerializeField] private int m_CurrentHealth;
    [SerializeField] private float m_MaxKillEnemys;
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
    private Enemies.EnemyManager m_EnemyManagerScript;

    static int altarCount = 0;
    private int ownNumber = 0;
    private Color myColor;
    private Transform m_playerTransform;

    public VisualEffect m_visualEffectActivation;
    public string txt_EventName;
    int resetNumber = 1;

    public SkinnedMeshRenderer[] altarAllMesh;
    public MeshRenderer socleMesh;

    public Material socleMaterial;
    [HideInInspector] public bool isAltarDestroy = false;

    private ObjectHealthSystem m_objectHealthSystem;
    public Image m_eventProgressionSlider;

    [SerializeField] private string instructionOnActivation;
    [SerializeField] private int nextReward;
    [SerializeField] private int nextRewardTypologie = 0;

    [SerializeField] private Enemies.EnemyManager m_enemeyManager;
    public Vector3 m_DropAreaPosition;

    private GameObject lastItemInstantiate;
    private GameObject nextRewardObject;
    [SerializeField] private LayerMask m_groundLayer;
    private Vector3 raycastdirection;

    private float progression = 0;
    // Start is called before the first frame update
    void Start()
    {
        raycastdirection = new Vector3(0, -10, 0);
        eventElementType = Random.Range(0, 4);

        GetComponentInChildren<Light>().color = colorEvent[eventElementType];
        Light[] lightToSwap = GetComponentsInChildren<Light>();
        for (int i = 0; i < lightToSwap.Length; i++)
        {
            lightToSwap[i].color = colorEvent[eventElementType];
        }
        ownNumber = altarCount;
        altarCount++;
        InitComponent();
        m_CurrentHealth = (int)m_MaxHealth;
        m_enemeyManager = GameObject.Find("Enemy Manager").GetComponent<Enemies.EnemyManager>();
        socleMesh.material.shader = Shader.Find("Intensity");
        socleMesh.material.shader = Shader.Find("Color");
        socleMesh.material = materialEvent[eventElementType];
        m_visualEffectActivation.GetComponentInChildren<VisualEffect>();
        m_visualEffectActivation.SetVector4("ColorEvent", colorEvent[eventElementType]);
        for (int i = 0; i < altarAllMesh.Length; i++)
        {

            altarAllMesh[i].material.SetColor("_SelfLitColor", colorEventTab[eventElementType]);
        }
        socleMesh.material.SetColor("_SelfLitColor", colorEventTab[eventElementType]);
        //DisableColor();
        m_playerTransform = m_EnemyManagerScript.m_playerTranform;
        float maxHealth = 50 + m_enemeyManager.m_maxUnittotal;
        m_objectHealthSystem.SetMaxHealth((int)maxHealth);
        m_objectHealthSystem.ResetCurrentHealth();
        RaycastHit hit;
        if (Physics.Raycast(transform.position + new Vector3(0, 25, 0), raycastdirection, out hit, Mathf.Infinity, m_groundLayer))
        {
            Debug.DrawRay(transform.position + new Vector3(0, 25, 0), raycastdirection * hit.distance, Color.cyan);
            m_DropAreaPosition = hit.point;
            Debug.Log("Did Hit");
        }
        else
        {
            Debug.DrawRay(transform.position + new Vector3(0, 25, 0), raycastdirection * 1000, Color.white);
            Debug.Log("Did not Hit");
        }
        nextRewardObject = rewardObject[0];
        // Does the ray intersect any objects excluding the player layer

    }

    private void InitComponent()
    {
        m_objectHealthSystem = GetComponent<ObjectHealthSystem>();
        m_EnemyManagerScript = GameObject.Find("Enemy Manager").GetComponent<Enemies.EnemyManager>();
        //canvasPrincipal = GameObject.Find("MainUI_EventDisplayHolder").GetComponent<RectTransform>();
        //ownDisplayEventDetail = Instantiate(displayEventDetail, canvasPrincipal.position, canvasPrincipal.rotation, canvasPrincipal);
        //ownArrowDisplayEventDetail = Instantiate(displayArrowEvent, canvasPrincipal.position, canvasPrincipal.rotation, canvasPrincipal.parent);
        //ownArrowDisplayEventDetail.GetComponent<UI_ArrowPointingEvent>().refGo = this.gameObject;
        //displayAnimator = ownDisplayEventDetail.GetComponent<EventDisplay>();
        //m_myAnimator = this.GetComponentInChildren<Animator>();
        //displayTextDescription1 = displayAnimator.m_textDescription1;
        //displayTextDescription2 = displayAnimator.m_textDescription2;
        //eventTextName = displayAnimator.m_textEventName;

    }
    // Update is called once per frame
    void Update()
    {

        if (!m_isEventOccuring) return;
        float ennemyTokill = 25 * (resetNumber + 1) + (m_enemeyManager.m_maxUnittotal * 0.25f);

        if (ennemyTokill <= m_CurrentKillCount && m_objectHealthSystem.IsEventActive())
        {
            m_myAnimator.SetBool("ActiveEvent", false);
            GiveRewardXp();
            m_enemeyManager.altarSuccessed++;
            m_objectHealthSystem.ChangeState(EventObjectState.Deactive);
            if(lastItemInstantiate != null)
            {
                
               Destroy(lastItemInstantiate);
            }

            //displayAnimator.InvertDisplayStatus(2);
        }
        else
        {
            progression = m_CurrentKillCount / ennemyTokill;
            m_eventProgressionSlider.fillAmount = progression;
            //displayTextDescription1.text = m_CurrentHealth + "/" + m_MaxHealth;
            //displayTextDescription2.text = (m_MaxKillEnemys * (1 + 0.1f * (resetNumber + 1))) - m_CurrentKillCount + " Remaining";
        }


        if (m_objectHealthSystem.IsEventActive() && Vector3.Distance(m_playerTransform.position, transform.position) > radiusEventActivePlayer)
        {
            DestroyAltar();
        }

        if (m_objectHealthSystem.eventState == EventObjectState.Death)
        {
            DestroyAltar();
        }



    }


    private void DestroyAltar()
    {
        m_objectHealthSystem.ChangeState(EventObjectState.Deactive);
        m_EnemyManagerScript.SendInstruction("Altar protection fail...", Color.red, TerrainLocationID.currentLocationName);
        m_myAnimator.SetBool("ActiveEvent", false);
        m_hasEventActivate = true;
        m_isEventOccuring = false;
        isAltarDestroy = true;
        m_EnemyManagerScript.RemoveTarget(transform);
        m_EnemyManagerScript.RemoveAltar(transform);
        Debug.Log("Destroy event");
    }


    // Need to set active
    public void ActiveEvent()
    {
        if (!m_objectHealthSystem.IsEventActive())
        {
            m_EnemyManagerScript.AddTarget(this.transform);
            m_EnemyManagerScript.AddAltar(transform);
            m_EnemyManagerScript.SendInstruction(instructionOnActivation + " [Repeat(+" + resetNumber + ")]", Color.white, TerrainLocationID.currentLocationName);
            if(resetNumber == 1)
            {
                m_myAnimator.SetTrigger("Activation");
            }
            if(resetNumber == 3)
            {
                lastItemInstantiate = Instantiate(DangerAddition[0], transform.position, transform.rotation);
            }
            else if (resetNumber == 4)
            {
                lastItemInstantiate = Instantiate(DangerAddition[1], transform.position, transform.rotation);
            }
            else if (resetNumber == 5)
            {
                lastItemInstantiate = Instantiate(DangerAddition[2], transform.position, transform.rotation);
            }
            m_visualEffectActivation.Play();
            for (int i = 0; i < altarAllMesh.Length; i++)
            {
                altarAllMesh[i].material.SetFloat("_SelfLitIntensity", 0.15f * resetNumber);
            }
            socleMesh.material.SetFloat("_SelfLitIntensity", 0.15f * resetNumber);
            m_myAnimator.SetBool("ActiveEvent", true);
            GlobalSoundManager.PlayOneShot(13, transform.position);
            m_objectHealthSystem.ChangeState(EventObjectState.Active);
            m_hasEventActivate = false;
            m_isEventOccuring = true;

            //this.transform.GetChild(0).gameObject.SetActive(true);
            //Enemies.EnemyManager.EnemyTargetPlayer = false;
            //this.transform.GetChild(0).gameObject.SetActive(true);
            //Enemies.EnemyManager.EnemyTargetPlayer = false;
            //this.transform.GetChild(0).gameObject.SetActive(true);
            //Enemies.EnemyManager.EnemyTargetPlayer = false;
            //displayAnimator.InvertDisplayStatus(1);
            //eventTextName.text = txt_EventName + " (+" + resetNumber + ")";
            //ActiveColor();
        }
        else
        {
            Debug.Log("Cette objet [" + this.name + "] ne peut pas être activé");
        }
    }

    public void IncreaseKillCount()
    {
        m_CurrentKillCount++;
    }

    public void GiveRewardXp()
    {

        m_EnemyManagerScript.RemoveTarget(transform);
        m_EnemyManagerScript.RemoveAltar(transform);
        m_EnemyManagerScript.SendInstruction("Event succeed ! Gain [" + nextReward + "] (" +nextRewardTypologie + ")", Color.green, TerrainLocationID.currentLocationName);
        m_isEventOccuring = false;
        m_myAnimator.SetBool("IsDone", true);
        for (int i = 0; i < altarAllMesh.Length; i++)
        {
            altarAllMesh[i].material.SetFloat("_SelfLitIntensity", 0.32f * resetNumber);
        }
        socleMesh.material.SetFloat("_SelfLitIntensity", 0.32f * resetNumber);
        //Enemies.EnemyManager.EnemyTargetPlayer = true;
        GlobalSoundManager.PlayOneShot(14, transform.position);
        for (int i = 0; i < nextReward; i++)
        {
            Vector3 rndVariant = new Vector3((float)Random.Range(-radiusEjection, radiusEjection), 0, (float)Random.Range(-radiusEjection, radiusEjection));
            GameObject xpGenerated = Instantiate(nextRewardObject, transform.position, Quaternion.identity);
            ExperienceMouvement ExpMovementRef = xpGenerated.GetComponent<ExperienceMouvement>();
            ExpMovementRef.GroundPosition = m_DropAreaPosition + rndVariant;
            StartCoroutine(ExpMovementRef.MoveToGround());
        }
        //xpGenerated.GetComponent<Rigidbody>().AddForce(new Vector3(rndVariant.x, 1 * m_ImpusleForceXp, rndVariant.y) , ForceMode.Impulse);
        //Enemies.EnemyManager.EnemyTargetPlayer = true;
        StartCoroutine(ResetEventWithDelay(1.5f));
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radiusEjection);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangeEvent);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, radiusEventActivePlayer);
    }

    public void ResetAltarEvent()
    {
        m_myAnimator.SetBool("ActiveEvent", false);
        progression = 0;
        resetNumber++;
        GenerateNextReward(resetNumber);
        m_hasEventActivate = true;
        m_isEventOccuring = false;
        m_CurrentKillCount = 0;
        float maxHealth = 50 + m_enemeyManager.m_maxUnittotal;
        m_objectHealthSystem.SetMaxHealth((int)maxHealth);
        m_objectHealthSystem.ResetCurrentHealth();

        //this.transform.GetChild(0).gameObject.SetActive(false);
        //Enemies.EnemyManager.EnemyTargetPlayer = true;
        //eventTextName.text = "Ready !";
        //displayAnimator.InvertDisplayStatus(2);
    }

    public IEnumerator ResetEventWithDelay(float time)
    {
        //eventTextName.text = "Finish !";
        yield return new WaitForSeconds(time);
        ResetAltarEvent();
    }

    public string[] GetAltarData() 
    {
        int RewardTypologie = nextRewardTypologie; // 0 = Cristal element; 1 = Experience quantité; 2 = Specific spell; 3 = Health quarter
        string[] dataToSend = new string[5];
        dataToSend[0] = RewardTypologie.ToString();
        dataToSend[1] = instructionOnActivation;
        if ( RewardTypologie == 0)
        {
            dataToSend[2] = nextReward.ToString();
            dataToSend[3] = eventElementType.ToString();
        }
        else if (RewardTypologie == 1)
        {
            dataToSend[2] = nextReward.ToString();
            dataToSend[3] = "123"; //1234 = Aucun element car la récompense est de l'experience
        }
        /*else if(RewardTypologie == 2)
        {
            dataToSend[2] = "-1"; // -1 = Aucune quantité particulière car la récompense est unique
            dataToSend[3] = "ID du spell associé à l'autel"; // A remplacé par une variable randomisé au démarrage
        }
        else if (RewardTypologie == 3)
        {
            dataToSend[2] = "-1"; // -1 = valeur par defaut signifiant 1 quarter de vie. Pourrait etre augmenté sous condition spécifique
            dataToSend[3] = "ID de la ressource (vie, mana, autre ?) associé au gain"; 
        }*/
        dataToSend[4] = ""+ progression;

        return dataToSend;
    }

    public void GenerateNextReward(int repeatNumber)
    {
        if(repeatNumber == 1)
        {
            nextRewardTypologie = 2;
            nextRewardObject = rewardObject[0];
        }
        else if (repeatNumber == 2)
        {
            nextRewardTypologie = 1;
            nextReward = (int)(100 + Time.timeSinceLevelLoad);
            nextRewardObject = rewardObject[0];
            Debug.Log("Next reward : " + nextReward);
        }
        else if (repeatNumber == 3)
        {
            nextRewardTypologie = 0;
            nextReward = 30;
            nextRewardObject = rewardObject[1 + eventElementType];
        }
        else if (repeatNumber == 4) 
        {
            nextRewardTypologie = 3;
            nextReward = 1;
            nextRewardObject = rewardObject[5];
        }
        else if (repeatNumber == 5)
        {
            nextRewardTypologie = 0;
            nextReward = 70;
            nextRewardObject = rewardObject[1 + eventElementType];
        }

    }

}
