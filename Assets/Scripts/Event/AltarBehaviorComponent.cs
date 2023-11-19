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

    [SerializeField] private Enemies.EnemyManager m_enemeyManager;
    public Vector3 m_DropAreaPosition;

    private GameObject lastItemInstantiate;
    private GameObject nextRewardObject;
    [SerializeField] private LayerMask m_groundLayer;
    private Vector3 raycastdirection;

    private float progression = 0;

    private int m_idSpellReward;

    #region Unity Functions
    void Start()
    {
        InitComponent();

        eventElementType = Random.Range(0, 4); 

        ownNumber = altarCount;
        altarCount++;

        m_playerTransform = m_enemeyManager.m_playerTranform;

        // Setup the altar health
        float maxHealth = 50 + m_enemeyManager.m_maxUnittotal;
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

        float ennemyTokill = 25 * (resetNumber + 1) + (m_enemeyManager.m_maxUnittotal * 0.25f);

        if (ennemyTokill <= m_CurrentKillCount && m_objectHealthSystem.IsEventActive())
        {
            m_myAnimator.SetBool("ActiveEvent", false);
            GiveRewardXp();
            m_enemeyManager.altarSuccessed++;
            m_objectHealthSystem.ChangeState(EventObjectState.Deactive);
            if (lastItemInstantiate != null)
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

    #endregion

    #region Init Functions
    private void InitComponent()
    {
        m_objectHealthSystem = GetComponent<ObjectHealthSystem>();
        m_enemeyManager = GameObject.Find("Enemy Manager").GetComponent<Enemies.EnemyManager>();
    }


    private void InitVisualElements()
    {
        Light[] lightToSwap = GetComponentsInChildren<Light>();
        for (int i = 0; i < lightToSwap.Length; i++)
        {
            lightToSwap[i].color = colorEvent[eventElementType];
        }

        socleMesh.material = materialEvent[eventElementType];
        socleMesh.material.SetColor("_SelfLitColor", colorEventTab[eventElementType]);

        m_visualEffectActivation.GetComponentInChildren<VisualEffect>();
        m_visualEffectActivation.SetVector4("ColorEvent", colorEvent[eventElementType]);

        for (int i = 0; i < altarAllMesh.Length; i++)
        {
            altarAllMesh[i].material.SetColor("_SelfLitColor", colorEventTab[eventElementType]);
        }
    }
    #endregion

    // Update is called once per frame
   


    private void DestroyAltar()
    {
        m_objectHealthSystem.ChangeState(EventObjectState.Deactive);
        m_enemeyManager.SendInstruction("Altar protection fail...", Color.red, TerrainLocationID.currentLocationName);
        m_myAnimator.SetBool("ActiveEvent", false);
        m_hasEventActivate = true;
        m_isEventOccuring = false;
        isAltarDestroy = true;
        m_enemeyManager.RemoveTarget(transform);
        m_enemeyManager.RemoveAltar(transform);
        Debug.Log("Destroy event");
    }


    // Need to set active
    public void ActiveEvent()
    {
        if (!m_objectHealthSystem.IsEventActive())
        {
            m_enemeyManager.AddTarget(this.transform);
            m_enemeyManager.AddAltar(transform);
            m_enemeyManager.SendInstruction(instructionOnActivation + " [Repeat(+" + resetNumber + ")]", Color.white, TerrainLocationID.currentLocationName);
            if(resetNumber == 0)
            {
                m_myAnimator.SetTrigger("Activation");
            }
            if(resetNumber == 2)
            {
                lastItemInstantiate = Instantiate(DangerAddition[0], transform.position, transform.rotation);
            }
            else if (resetNumber == 3)
            {
                lastItemInstantiate = Instantiate(DangerAddition[1], transform.position, transform.rotation);
            }
            else if (resetNumber == 4)
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
        }
        else
        {
            Debug.Log("Cette objet [" + this.name + "] ne peut pas �tre activ�");
        }
    }

    public void IncreaseKillCount()
    {
        m_CurrentKillCount++;
    }

    public void GiveRewardXp()
    {

        m_enemeyManager.RemoveTarget(transform);
        m_enemeyManager.RemoveAltar(transform);
        m_enemeyManager.SendInstruction("Event succeed ! Gain [" + nextReward + "] (" +nextRewardTypologie + ")", Color.green, TerrainLocationID.currentLocationName);
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

            if (nextRewardTypologie == 2)
                xpGenerated.GetComponent<CapsuleContainer>().capsuleIndex = m_idSpellReward;

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
        int RewardTypologie = nextRewardTypologie; // 0 = Cristal element; 1 = Experience quantit�; 2 = Specific spell; 3 = Health quarter
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
            dataToSend[3] = "123"; //1234 = Aucun element car la r�compense est de l'experience
        }
        else if(RewardTypologie == 2)
        {
            dataToSend[2] = "-1"; // -1 = Aucune quantit� particuli�re car la r�compense est unique
            dataToSend[3] = m_idSpellReward.ToString(); // A remplac� par une variable randomis� au d�marrage
        }
      /*  else if (RewardTypologie == 3)
        {
            dataToSend[2] = "-1"; // -1 = valeur par defaut signifiant 1 quarter de vie. Pourrait etre augment� sous condition sp�cifique
            dataToSend[3] = "ID de la ressource (vie, mana, autre ?) associ� au gain"; 
        }*/
        dataToSend[4] = ""+ progression;

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

    public IEnumerator LastStart()
    {
       yield return new WaitForSeconds(2.0f);
        m_idSpellReward = CapsuleManager.GetRandomCapsuleIndex();
        GenerateNextReward(resetNumber);
    }

}
