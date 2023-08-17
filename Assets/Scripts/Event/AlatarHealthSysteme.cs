using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlatarHealthSysteme : MonoBehaviour
{
    [Range(0, 3)]
    [SerializeField] int eventElementType = 0;
    [SerializeField] private Color[] colorEvent;
    [SerializeField] private float m_TimeInvulnerability;
    [SerializeField] private float m_MaxHealth;
    [SerializeField] private int m_CurrentHealth;
    [SerializeField] private float m_MaxKillEnemys;
    [SerializeField] private int m_CurrentKillCount;
    [SerializeField] private int XpQuantity = 100;
    [SerializeField] private GameObject[] xpObject;
    [SerializeField] private float m_ImpusleForceXp;
    public float tempsEcouleInvulnerability;
    private bool bool_Invulnerrable;
    public bool bool_ActiveEvent;
    private bool bool_Activable = true;
    private bool bool_EventEnCour;
    public Animator m_myAnimator;

    public EventDisplay displayAnimator;

    private Text displayTextDescription1;
    private Text displayTextDescription2;
    private Text eventTextName;

    public float radiusEjection;
    public GameObject displayEventDetail;
    public GameObject displayArrowEvent;
    private GameObject ownDisplayEventDetail;
    private GameObject ownArrowDisplayEventDetail;

    private RectTransform canvasPrincipal;
    private Enemies.EnemyManager m_EnemyManagerScript;

    public int rangeEvent = 100;
    static int altarCount = 0;
    private int ownNumber = 0;
    public Color myColor;

    public string txt_EventName;
    int resetNumber = 0;
    // Start is called before the first frame update
    void Start()
    {
        eventElementType = Random.Range(0, 4);
        GetComponentInChildren<Light>().color = colorEvent[eventElementType];
        ownNumber = altarCount;
        altarCount++;
        myColor = GetColorByID(ownNumber);
        InitComponent();
        m_CurrentHealth = (int)m_MaxHealth;
        //DisableColor();
    }

    private void InitComponent()
    {
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
        
        if(bool_ActiveEvent) { ActiveEvent(); }
        if(m_MaxKillEnemys * (1 + 0.1f * (resetNumber + 1)) <= m_CurrentKillCount && bool_EventEnCour)
        {
            m_myAnimator.SetBool("ActiveEvent", false);
            GiveRewardXp();
            //displayAnimator.InvertDisplayStatus(2);
        }
        else
        {
            //displayTextDescription1.text = m_CurrentHealth + "/" + m_MaxHealth;
            //displayTextDescription2.text = (m_MaxKillEnemys * (1 + 0.1f * (resetNumber + 1))) - m_CurrentKillCount + " Remaining";
        }
    }

    public void OnCollisionStay(Collision collision)
    {
        if (!bool_EventEnCour) return;
        if (bool_Invulnerrable) return;
        if (collision.gameObject.tag != "Enemy") return;

        m_CurrentHealth--;
        StartCoroutine(TakeDamage(m_TimeInvulnerability));
    }
    public IEnumerator TakeDamage(float time)
    {
        bool_Invulnerrable = true;
        //m_myAnimator.SetTrigger("TakeHit");
        yield return new WaitForSeconds(time);
        //m_myAnimator.ResetTrigger("TakeHit");
        bool_Invulnerrable = false;
    }

    public void ActiveEvent()
    {
        if(bool_Activable)
        {
            m_EnemyManagerScript.AddAltarEvent(this.transform);
            //this.transform.GetChild(0).gameObject.SetActive(true);
            //Enemies.EnemyManager.EnemyTargetPlayer = false;
            m_myAnimator.SetBool("ActiveEvent", true);
            //ActiveColor();
            GlobalSoundManager.PlayOneShot(13, transform.position);
            //displayAnimator.InvertDisplayStatus(1);
            //eventTextName.text = txt_EventName + " (+" + resetNumber + ")";
            bool_Invulnerrable = false;
            bool_ActiveEvent = false;
            bool_Activable = false;
            bool_EventEnCour = true;
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
        m_EnemyManagerScript.RemoveAltarEvent(this.transform);
        bool_EventEnCour = false;
        m_myAnimator.SetBool("IsDone", true);
        //Enemies.EnemyManager.EnemyTargetPlayer = true;
        GlobalSoundManager.PlayOneShot(14, transform.position);
        for (int i = 0; i < XpQuantity + 25 * resetNumber; i++)
        {
            Vector2 rndVariant = new Vector2((float)Random.Range(-2, 2), (float)Random.Range(-2, 2));
            GameObject xpGenerated;
                xpGenerated = Instantiate(xpObject[0], transform.position + new Vector3(rndVariant.x * radiusEjection, 0, rndVariant.y * radiusEjection), Quaternion.identity);
            //xpGenerated.GetComponent<Rigidbody>().AddForce(new Vector3(rndVariant.x, 1 * m_ImpusleForceXp, rndVariant.y) , ForceMode.Impulse);
        }
        StartCoroutine(ResetEventWithDelay(3));
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radiusEjection);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangeEvent);
    }

    public void ResetAltarEvent()
    {
        //this.transform.GetChild(0).gameObject.SetActive(false);
        //Enemies.EnemyManager.EnemyTargetPlayer = true;
        m_myAnimator.SetBool("ActiveEvent", false);
        //eventTextName.text = "Ready !";
        resetNumber++;
        //DisableColor();
        //displayAnimator.InvertDisplayStatus(2);
        bool_Invulnerrable = false;
        bool_ActiveEvent = false;
        bool_Activable = true;
        bool_EventEnCour = false;
        m_CurrentKillCount = 0;
        m_MaxHealth = 100 * (1 - 0.1f * (resetNumber + 1));
        m_CurrentHealth = (int)m_MaxHealth;
    }

    public IEnumerator ResetEventWithDelay(float time)
    {
        //eventTextName.text = "Finish !";
        yield return new WaitForSeconds(time);
        ResetAltarEvent();
    }

    public Color GetColorByID(int ID)
    {
        if(ID == 0) { return Color.red; }
        else if (ID == 1) { return Color.blue; }
        else if (ID == 2) { return Color.green; }
        else if (ID == 3) { return Color.cyan; }
        else if (ID == 4) { return Color.yellow; }
        else if (ID == 5) { return Color.magenta; }
        else if (ID == 6) { return Color.grey; }
        else { return Color.white; }
    }


    public void ActiveColor()
    {
        ownDisplayEventDetail.GetComponent<EventDisplay>().Buttonimage.color = myColor;
        ownArrowDisplayEventDetail.GetComponent<Image>().color = myColor;
    }

    public void DisableColor()
    {
        ownDisplayEventDetail.GetComponent<EventDisplay>().Buttonimage.color = Color.gray;
        ownArrowDisplayEventDetail.GetComponent<Image>().color = Color.gray;
    }
}
