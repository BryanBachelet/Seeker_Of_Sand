using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltarHealth : MonoBehaviour
{
    [SerializeField] private int m_healthMax;
    [SerializeField] private int m_currentHealth;
    [SerializeField] private float m_timeInvulnerable;

    public float tempsEcoule;

    private MeshRenderer m_Renderer;
    private Animator m_Animator;
    // Start is called before the first frame update
    void Start()
    {
        InitComponent();
    }

    private void InitComponent()
    {
        m_Renderer = this.GetComponent<MeshRenderer>();
        m_Animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (tempsEcoule > 0)
        {
            tempsEcoule -= Time.deltaTime;
        }
    }

    public void TakeDamage()
    {
        if (tempsEcoule > 0) return;
        else
        {
            tempsEcoule = m_timeInvulnerable;
            m_currentHealth--;
            StartCoroutine(DamageCorroutine(1));
        }
    }

    IEnumerator DamageCorroutine(float time)
    {
        m_Animator.SetTrigger("TakeHit");
        yield return new WaitForSeconds(time / 2);
        m_Animator.ResetTrigger("TakeHit");
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.tag == "Enemy" && tempsEcoule <= 0)
        {
            Debug.Log("Collide !!!");
            TakeDamage();
        }
    }
}
