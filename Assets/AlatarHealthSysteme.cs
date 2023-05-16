using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlatarHealthSysteme : MonoBehaviour
{
    [SerializeField] private float m_TimeInvulnerability;
    [SerializeField] private int m_MaxHealth;
    [SerializeField] private int m_CurrentHealth;
    [SerializeField] private int m_MaxKillEnemys;
    [SerializeField] private int m_CurrentKillCount;
    public float tempsEcouleInvulnerability;
    public bool bool_Invulnerrable;
    public bool bool_ActiveEvent;
    private Animator m_myAnimator;

    public Animator displayAnimator;

    // Start is called before the first frame update
    void Start()
    {
        InitComponent();
        m_CurrentHealth = m_MaxHealth;
    }

    private void InitComponent()
    {
        m_myAnimator = this.GetComponent<Animator>();

    }
    // Update is called once per frame
    void Update()
    {
        if(bool_ActiveEvent) { ActiveEvent(); }
        if(m_MaxKillEnemys <= m_CurrentKillCount)
        {
            m_myAnimator.SetBool("ActiveEvent", false);
            displayAnimator.SetBool("DisplayEvent", false);
        }
    }

    public void OnCollisionStay(Collision collision)
    {
        if (Enemies.EnemyManager.EnemyTargetPlayer) return;
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
        this.transform.GetChild(0).gameObject.SetActive(true);
        Enemies.EnemyManager.EnemyTargetPlayer = false;
        m_myAnimator.SetBool("ActiveEvent", true);
        displayAnimator.SetBool("DisplayEvent", true);
        bool_Invulnerrable = false;
        bool_ActiveEvent = false;
    }

    public void IncreaseKillCount()
    {
        m_CurrentKillCount++;
    }
}
