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
    [SerializeField] public Image m_SliderCurrentHealthHigh;
    [SerializeField] public Image m_SliderCurrentHealthHighBuffer;
    [SerializeField] public Image m_SliderCurrentQuarterHigh;
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
    public GameObject vignetteEffectWithPlane;
    private Material m_vignetteObjectMat;
    private Color m_materialVignetteColor;

    private bool feedbackHit = false;
    private float timeLastHit;
    public AnimationCurve evolutionVignetteOverTime;
    public float tempsEffetHit = 0.25f;
    private bool healthBuffer;
    private float lastHealth;
    // Start is called before the first frame update
    void Start()
    {
        InitializedHealthData();
        m_characterMouvement = GetComponent<Character.CharacterMouvement>();
        m_vignetteObjectMat = vignetteEffectWithPlane.GetComponent<Renderer>().material;
        m_materialVignetteColor = m_vignetteObjectMat.color;
        //volume.profile.TryGet(out vignette);
    }

    // Update is called once per frame
    void Update()
    {
        if (activeDeath && m_CurrentHealth <= 0 && !isActivate)
        {
            GameState.LaunchEndMenu();
           
            isActivate = true;
            return;
        }
        if (updateHealthValues)
        {
        //    GetDamageLeger(damageSend);

        }
        if (healthBuffer)
        {
            BufferXpDisplay();
        }
        if (feedbackHit)
        {
            if (Time.time - timeLastHit < tempsEffetHit)
            {
                m_materialVignetteColor.a = ((0.35f + (0.05f * m_CurrentQuarter)) * evolutionVignetteOverTime.Evaluate(Time.time - timeLastHit));
                //vignette.intensity.value = ;
                m_vignetteObjectMat.color = m_materialVignetteColor;
            }
            else
            {
                m_materialVignetteColor.a = 0;
                //vignette.intensity.value = ;
                m_vignetteObjectMat.color = m_materialVignetteColor;
                //vignette.intensity.value = 0;
                feedbackHit = false;
            }
            if(Time.time - timeLastHit > 1)
            {
                healthBuffer = false;
                m_SliderCurrentHealthHigh.fillAmount = m_CurrentHealth / m_MaxHealthQuantity;

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

            feedbackHit = true;
            m_materialVignetteColor.a = 0.35f;
            //vignette.intensity.value = ;
            m_vignetteObjectMat.color = m_materialVignetteColor;
            //vignette.intensity.value = 0.35f;
            if (m_CurrentQuarter - 1 >= 0 && m_CurrentHealth - damage < m_CurrentQuarterMinHealth[m_CurrentQuarter - 1])
            {
                m_CurrentQuarter -= 1;
                m_CurrentHealth = m_CurrentQuarterMinHealth[m_CurrentQuarter];
            }
            else
            {
                m_CurrentHealth -= damage;
            }


            m_SliderCurrentHealthHighBuffer.fillAmount = m_CurrentHealth / m_MaxHealthQuantity;
            m_SliderCurrentQuarterHigh.fillAmount = 1 / m_QuarterNumber * (m_QuarterNumber - m_CurrentQuarter);
            m_characterMouvement.SetKnockback(position);
            ActiveBufferHealth(Time.time, m_CurrentHealth);

            if (m_CurrentHealth <= 0)
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
            feedbackHit = true;
            m_materialVignetteColor.a = 0.35f;
            //vignette.intensity.value = ;
            m_vignetteObjectMat.color = m_materialVignetteColor;
            //vignette.intensity.value = 0.35f;
            if (m_CurrentHealth - damage < m_CurrentQuarterMinHealth[m_CurrentQuarter - 1])
            {
                m_CurrentQuarter -= 1;
                m_CurrentHealth = m_CurrentQuarterMinHealth[m_CurrentQuarter];
            }
            else
            {
                m_CurrentHealth -= damage;
            }

            m_SliderCurrentHealthHighBuffer.fillAmount = m_CurrentHealth / m_MaxHealthQuantity;
            m_SliderCurrentQuarterHigh.fillAmount = 1 / m_QuarterNumber * (m_QuarterNumber - m_CurrentQuarter);
            ActiveBufferHealth(Time.time, m_CurrentHealth);

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

        m_SliderCurrentHealthHighBuffer.fillAmount = m_CurrentHealth / m_MaxHealthQuantity;
        m_SliderCurrentQuarterHigh.fillAmount = 1 / m_QuarterNumber * (m_QuarterNumber - m_CurrentQuarter);
        m_CurrentHealth = m_MaxHealthQuantity;
        ActiveBufferHealth(Time.time, m_CurrentHealth);
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

    public void RestoreHealQuarter(int quantity)
    {
        m_CurrentHealth += m_QuarterHealthQuantity;
        if(m_CurrentHealth > m_MaxHealthQuantity)
        {

            m_CurrentHealth = m_MaxHealthQuantity;
            ActiveBufferHealth(Time.time, m_CurrentHealth);
            //m_CurrentQuarter = (int)m_QuarterNumber;
        }
        else
        {
            //m_CurrentQuarter += 1;
        }
        InitializedHealthData();
    }
    private void OnCollisionStay(Collision collision)
    {
        //  Debug.Log("Hit an Object !");
        if (collision.transform.tag != "Enemy" || !GameState.IsPlaying()  ) return;
       // Debug.Log("Object was an Enemy !");
        GetDamageLeger(2,collision.transform.position);
    }

    private void ActiveBufferHealth(float time, float health)
    {
        timeLastHit = time;
        healthBuffer = true;
        lastHealth = health;
    }
    private void BufferXpDisplay()
    {
       m_SliderCurrentHealthHigh.fillAmount = Mathf.Lerp(lastHealth / m_MaxHealthQuantity, m_CurrentHealth / m_MaxHealthQuantity, (timeLastHit - Time.time - 1) /1 );

    }

}
