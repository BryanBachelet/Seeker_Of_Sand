using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlatarHealthSysteme : MonoBehaviour
{
    [SerializeField] private float m_TimeInvulnerability;
    [SerializeField] private int m_MaxHealth;
    [SerializeField] private int m_CurrentHealth;
    [SerializeField] private int m_MaxKillEnemys;
    [SerializeField] private int m_CurrentKillCount;
    [SerializeField] private int XpQuantity = 100;
    [SerializeField] private GameObject[] xpObject;
    [SerializeField] private float m_ImpusleForceXp;
    public float tempsEcouleInvulnerability;
    private bool bool_Invulnerrable;
    public bool bool_ActiveEvent;
    private bool bool_Activable = true;
    private bool bool_EventEnCour;
    private Animator m_myAnimator;

    public EventDisplay displayAnimator;

    private Text displayTextDescription1;
    private Text displayTextDescription2;

    public float radiusEjection;
    public GameObject displayEventDetail;
    private GameObject ownDisplayEventDetail;

    private RectTransform canvasPrincipal;
    private Enemies.EnemyManager m_EnemyManagerScript;
    
    // Start is called before the first frame update
    void Start()
    {
        InitComponent();
        m_CurrentHealth = m_MaxHealth;
    }

    private void InitComponent()
    {
        m_EnemyManagerScript = GameObject.Find("Enemy Manager").GetComponent<Enemies.EnemyManager>();
        canvasPrincipal = GameObject.Find("MainUI_EventDisplayHolder").GetComponent<RectTransform>();
        ownDisplayEventDetail = Instantiate(displayEventDetail, canvasPrincipal.position, canvasPrincipal.rotation, canvasPrincipal);
        displayAnimator = ownDisplayEventDetail.GetComponent<EventDisplay>();
        m_myAnimator = this.GetComponent<Animator>();
        displayTextDescription1 = displayAnimator.m_textDescription1;
        displayTextDescription2 = displayAnimator.m_textDescription2;

    }
    // Update is called once per frame
    void Update()
    {
        
        if(bool_ActiveEvent) { ActiveEvent(); }
        if(m_MaxKillEnemys <= m_CurrentKillCount && bool_EventEnCour)
        {
            m_myAnimator.SetBool("ActiveEvent", false);
            GiveRewardXp();
            displayAnimator.InvertDisplayStatus(2);
        }
        else
        {
            displayTextDescription1.text = m_CurrentHealth + "/" + m_MaxHealth;
            displayTextDescription2.text = m_MaxKillEnemys - m_CurrentKillCount + " Remaining";
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
        m_myAnimator.SetTrigger("TakeHit");
        yield return new WaitForSeconds(time / 2);
        m_myAnimator.ResetTrigger("TakeHit");
        bool_Invulnerrable = false;
    }

    public void ActiveEvent()
    {
        if(bool_Activable)
        {
            m_EnemyManagerScript.AddAltarEvent(this.transform);
            this.transform.GetChild(0).gameObject.SetActive(true);
            //Enemies.EnemyManager.EnemyTargetPlayer = false;
            m_myAnimator.SetBool("ActiveEvent", true);
            GlobalSoundManager.PlayOneShot(13, transform.position);
            displayAnimator.InvertDisplayStatus(1);
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
        //Enemies.EnemyManager.EnemyTargetPlayer = true;
        GlobalSoundManager.PlayOneShot(14, transform.position);
        for (int i = 0; i < XpQuantity; i++)
        {
            int rnd = Random.Range(0, 100);
            Vector2 rndVariant = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
            GameObject xpGenerated;
            if (rnd < 95)
            {
                xpGenerated = Instantiate(xpObject[0], transform.position + new Vector3(rndVariant.x * radiusEjection, transform.position.y, rndVariant.y * radiusEjection), Quaternion.identity);
            }
            else
            {
                xpGenerated = Instantiate(xpObject[1], transform.position + new Vector3(rndVariant.x * radiusEjection, transform.position.y, rndVariant.y * radiusEjection), Quaternion.identity);
            }
            //xpGenerated.GetComponent<Rigidbody>().AddForce(new Vector3(rndVariant.x, 1 * m_ImpusleForceXp, rndVariant.y) , ForceMode.Impulse);
        }
        StartCoroutine(ResetEventWithDelay(3));
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radiusEjection);
    }

    public void ResetAltarEvent()
    {
        this.transform.GetChild(0).gameObject.SetActive(false);
        //Enemies.EnemyManager.EnemyTargetPlayer = true;
        m_myAnimator.SetBool("ActiveEvent", false);
        displayAnimator.InvertDisplayStatus(2);
        bool_Invulnerrable = false;
        bool_ActiveEvent = false;
        bool_Activable = true;
        bool_EventEnCour = false;
        m_CurrentKillCount = 0;
        m_CurrentHealth = m_MaxHealth;

    }

    public IEnumerator ResetEventWithDelay(float time)
    {
        yield return new WaitForSeconds(time);
        ResetAltarEvent();
    }
}
