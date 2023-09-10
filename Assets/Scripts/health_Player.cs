using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
public class health_Player : MonoBehaviour
{
    [SerializeField] private bool activeDeath = false;
    [SerializeField] private GameObject m_gameOverMenu;

    private bool isActivate = false;
    [SerializeField] private float m_MaxHealthQuantity;
    [SerializeField] private float[] m_CurrentQuarterMaxHealth;
    [SerializeField] private float[] m_CurrentQuarterMinHealth;
    [SerializeField] private float m_QuarterNumber;
    [SerializeField] private int m_CurrentQuarter;
    [SerializeField] private float m_QuarterHealthQuantity;
    [SerializeField] private float m_CurrentHealth;
    [SerializeField] private float m_invulerableLegerTime;
    [SerializeField] private float m_invulerableLourdTime;
    [SerializeField] private Image m_SliderCurrentHealthHigh;
    [SerializeField] private Image m_SliderCurrentQuarterHigh;
    [SerializeField] private Image m_SliderCurrentHealthLow;
    [SerializeField] private Image m_SliderCurrentQuarterLow;

    private bool m_isInvulnerableLeger = false;
    private bool m_isInvulnerableLourd = false;
    private float m_lastHitTime = 0;

    public bool updateHealthValues = false;

    private Character.CharacterMouvement m_characterMouvement;

    public float damageSend;

    public Volume volume;
    private Vignette vignette;

    private bool feedbackHit = false;
    private float timeLastHit;
    public AnimationCurve evolutionVignetteOverTime;
    public float tempsEffetHit = 0.25f;
    // Start is called before the first frame update
    void Start()
    {
        InitializedHealthData();
        m_characterMouvement = GetComponent<Character.CharacterMouvement>();
        volume.profile.TryGet(out vignette);
    }

    // Update is called once per frame
    void Update()
    {
        if (activeDeath && m_CurrentHealth <= 0 && !isActivate)
        {
            GameState.DeathActivation();
      //      m_gameOverMenu.SetActive(true);
            isActivate = true;
            return;
        }
        if (updateHealthValues)
        {
        //    GetDamageLeger(damageSend);

        }
        if(feedbackHit)
        {
            if (Time.time - timeLastHit < tempsEffetHit)
            {
                vignette.intensity.value = 0.35f * evolutionVignetteOverTime.Evaluate(Time.time - timeLastHit);
            }
            else
            {
                vignette.intensity.value = 0;
                feedbackHit = false;
            }
        }


    }

    public void GetDamageLeger(float damage, Vector3 position)
    {
        if (m_isInvulnerableLourd) return;
        if (m_isInvulnerableLeger) return;
        else
        {
            GlobalSoundManager.PlayOneShot(29, transform.position);
            StartCoroutine(GetInvulnerableLeger(m_invulerableLegerTime));
            timeLastHit = Time.time;
            feedbackHit = true;
            vignette.intensity.value = 0.35f;
            if (m_CurrentQuarter - 1 >= 0 && m_CurrentHealth - damage < m_CurrentQuarterMinHealth[m_CurrentQuarter - 1])
            {
                m_CurrentQuarter -= 1;
                m_CurrentHealth = m_CurrentQuarterMinHealth[m_CurrentQuarter];
            }
            else
            {
                m_CurrentHealth -= damage;
            }

                m_SliderCurrentHealthHigh.fillAmount = m_CurrentHealth / m_MaxHealthQuantity;
                m_SliderCurrentQuarterHigh.fillAmount = 1 / m_QuarterNumber * (m_QuarterNumber - m_CurrentQuarter);

            m_characterMouvement.SetKnockback(position);

            if(m_CurrentHealth <= 0)
            {
                activeDeath = true;
            }
        }
        updateHealthValues = false;

    }

    public void GetDamageLourd(float damage)
    {
        if (m_isInvulnerableLourd) return;
        else
        {
            GlobalSoundManager.PlayOneShot(29, transform.position);
            StartCoroutine(GetInvulnerableLourd(m_invulerableLourdTime));
            timeLastHit = Time.time;
            feedbackHit = true;
            vignette.intensity.value = 0.35f;
            if (m_CurrentHealth - damage < m_CurrentQuarterMinHealth[m_CurrentQuarter - 1])
            {
                m_CurrentQuarter -= 1;
                m_CurrentHealth = m_CurrentQuarterMinHealth[m_CurrentQuarter];
            }
            else
            {
                m_CurrentHealth -= damage;
            }

            m_SliderCurrentHealthHigh.fillAmount = m_CurrentHealth / m_MaxHealthQuantity;
            m_SliderCurrentQuarterHigh.fillAmount = 1 / m_QuarterNumber * (m_QuarterNumber - m_CurrentQuarter);

            if (m_CurrentHealth <= 0)
            {
                activeDeath = true;
            }
        }
        updateHealthValues = false;
    }

    IEnumerator GetInvulnerableLeger(float time)
    {
        m_isInvulnerableLeger = true;
        yield return new WaitForSeconds(time);
        m_isInvulnerableLeger = false;
    }
    IEnumerator GetInvulnerableLourd(float time)
    {
        m_isInvulnerableLourd = true;
        yield return new WaitForSeconds(time);
        m_isInvulnerableLourd = false;
    }
    public void InitializedHealthData()
    {

        //m_CurrentHealth = m_MaxHealthQuantity;
        m_QuarterHealthQuantity = m_MaxHealthQuantity / m_QuarterNumber;
        m_CurrentQuarter = (int)Mathf.Ceil(m_CurrentHealth / m_MaxHealthQuantity * m_QuarterNumber);
        for (int i = 0; i < m_CurrentQuarterMaxHealth.Length; i++)
        {
            m_CurrentQuarterMinHealth[i] = m_QuarterHealthQuantity * i;
            m_CurrentQuarterMaxHealth[i] = m_QuarterHealthQuantity * i + m_QuarterHealthQuantity;
        }
        //Calculer la valeur d'un quart de vie
        //Actualiser m_CurrentQuarterMaxHealth && m_CurrentQuarterMinHealth
        //Actualiser m_CurrentQuarter
        m_SliderCurrentHealthHigh.fillAmount = m_CurrentHealth / m_MaxHealthQuantity;
        m_SliderCurrentQuarterHigh.fillAmount = 1 / m_QuarterNumber * (m_QuarterNumber - m_CurrentQuarter);
        m_CurrentHealth = m_MaxHealthQuantity;
        updateHealthValues = false;
        m_isInvulnerableLeger = false;
        m_isInvulnerableLourd = false;
    }

    public void AugmenteMaxHealth(int quantity)
    {
        m_MaxHealthQuantity += quantity;
        m_CurrentHealth += m_CurrentHealth;
        InitializedHealthData();
    }

    private void OnCollisionStay(Collision collision)
    {
      //  Debug.Log("Hit an Object !");
        if (collision.transform.tag != "Enemy") return;
       // Debug.Log("Object was an Enemy !");
        GetDamageLeger(2,collision.transform.position);
    }

}
