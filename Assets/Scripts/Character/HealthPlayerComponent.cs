using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public struct AttackDamageInfo
{
    public string attackName;
    public int damage;
    public Vector3 position;
}

public class HealthPlayerComponent : MonoBehaviour
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
    [SerializeField] private AnimationCurve m_SlowdownEffectCurve;
    [SerializeField] private AnimationCurve m_SlowdownEffectCurveLastQuarter;
    private AnimationCurve m_currentAnimationCurveToUse;
    [SerializeField] private float m_DurationSlowdownEffect = 1;
    private float tempsEcouleSlowEffect = 0;
    private bool slowDownActive = false;

    private float m_healthLost;

    private bool m_isInvulnerableLeger = false;
    private bool m_isInvulnerableLourd = false;

    public bool updateHealthValues = false;

    private Character.CharacterMouvement m_characterMouvement;

    public float damageSend;

    public Volume volume;
    private Vignette vignette;
    private ColorAdjustments colorAdjustments;

    public bool feedbackHit = false;
    private float timeLastHit;
    public AnimationCurve evolutionVignetteOverTime;
    public float tempsEffetHit = 0.25f;
    private bool healthBuffer;
    private float lastHealth;


    public System.Action<AttackDamageInfo> OnDamage;

    public delegate void OnContact(Vector3 position, EntitiesTrigger tag, GameObject objectHit);
    public event OnContact OnContactEvent = delegate { };


    public GameState gameStateObject;
    private Camera m_cameraUsed;
    [SerializeField] private AnimationCurve m_fieldOfViewEdit;
    [SerializeField] private AnimationCurve m_ColorAdjustementSaturation;
    // Start is called before the first frame update
    void Start()
    {
        InitializedHealthData();
        m_characterMouvement = GetComponent<Character.CharacterMouvement>();
        m_cameraUsed = Camera.main;
        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out colorAdjustments);
    }

    // Update is called once per frame
    void Update()
    {
        if (activeDeath && m_CurrentHealth <= 0 && !isActivate)
        {
            GuerhoubaGames.SaveData.GameData.UpdateFarestRoom(TerrainGenerator.roomGeneration_Static);
            GameState.LaunchEndMenu();
            gameStateObject.HideGlobalUI();

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
                vignette.opacity.value = /*((0.35f + (0.05f * m_CurrentQuarter)) * */evolutionVignetteOverTime.Evaluate(Time.time - timeLastHit);
            }
            else
            {
                vignette.opacity.value = 0;
                feedbackHit = false;
            }
        }
        if (slowDownActive)
        {
            float progress = tempsEcouleSlowEffect / m_DurationSlowdownEffect;
            float newTimeScale = m_currentAnimationCurveToUse.Evaluate(progress);
            m_cameraUsed.fieldOfView = m_fieldOfViewEdit.Evaluate(progress);
            colorAdjustments.saturation.value = m_ColorAdjustementSaturation.Evaluate(progress);
            Time.timeScale = newTimeScale;
            tempsEcouleSlowEffect += (Time.deltaTime * (1 + (1 - newTimeScale)));
            if(tempsEcouleSlowEffect >= m_DurationSlowdownEffect)
            {
                newTimeScale = 1;
                tempsEcouleSlowEffect = 0;
                Time.timeScale = 1;
                m_cameraUsed.fieldOfView = 65;
                colorAdjustments.saturation.value = 3.8f;
                slowDownActive = false;
            }
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("SlowDownEffect", newTimeScale);
        }


    }

    public void GetLightDamage(AttackDamageInfo attackDamageInfo)
    {
        if (m_isInvulnerableLourd) return;
        if (m_isInvulnerableLeger) return;
        else
        {
            GlobalSoundManager.PlayOneShot(29, transform.position);
            StartCoroutine(GetInvulnerableLeger(m_invulerableLegerTime));
            ActiveBufferHealth(Time.time, m_CurrentHealth);
            feedbackHit = true;
            vignette.intensity.value = 0.35f;
            if (m_CurrentQuarter - 1 >= 0 && m_CurrentHealth - attackDamageInfo.damage < m_CurrentQuarterMinHealth[m_CurrentQuarter - 1])
            {
                ActiveSlowEffect(1.5f, m_CurrentQuarter);
                m_CurrentQuarter -= 1;
                m_CurrentHealth = m_CurrentQuarterMinHealth[m_CurrentQuarter];

            }
            else
            {
                m_CurrentHealth -= attackDamageInfo.damage; 
            }

            if (OnDamage != null) OnDamage.Invoke(attackDamageInfo);

            m_SliderCurrentHealthHighBuffer.fillAmount = m_CurrentHealth / m_MaxHealthQuantity;
            BufferXpDisplay();
            m_SliderCurrentQuarterHigh.fillAmount = 1 / m_QuarterNumber * (m_QuarterNumber - m_CurrentQuarter);
            m_characterMouvement.SetKnockback(attackDamageInfo.position);

            if (m_CurrentHealth <= 0)
            {
                activeDeath = true;
            }
        }
        updateHealthValues = false;

    }

    public void GetHeavyDamage(AttackDamageInfo attackDamageInfo)
    {
        if (m_isInvulnerableLourd) return;
        else
        {
            GlobalSoundManager.PlayOneShot(29, transform.position);
            StartCoroutine(GetInvulnerableLourd(m_invulerableLourdTime));
            ActiveBufferHealth(Time.time, m_CurrentHealth);
            feedbackHit = true;
            vignette.intensity.value = 0.35f;
            if (m_CurrentHealth - attackDamageInfo.damage < m_CurrentQuarterMinHealth[m_CurrentQuarter - 1])
            {
                ActiveSlowEffect(1.5f, m_CurrentQuarter);
                m_CurrentQuarter -= 1;
                m_CurrentHealth = m_CurrentQuarterMinHealth[m_CurrentQuarter];
            }
            else
            {
                m_CurrentHealth -= attackDamageInfo.damage;
            }
            if (OnDamage != null) OnDamage.Invoke(attackDamageInfo);
            m_SliderCurrentHealthHighBuffer.fillAmount = m_CurrentHealth / m_MaxHealthQuantity;
            m_SliderCurrentQuarterHigh.fillAmount = 1 / m_QuarterNumber * (m_QuarterNumber - m_CurrentQuarter);
            BufferXpDisplay();

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
        ActiveBufferHealth(Time.time, m_CurrentHealth);
        BufferXpDisplay();
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

    public void RestoreQuarter()
    {
        int indexQuarter = (int)(m_CurrentHealth) / (int)(m_QuarterHealthQuantity);
        m_CurrentHealth = Mathf.Clamp((indexQuarter + 1) * m_QuarterHealthQuantity, 0, m_MaxHealthQuantity);
        m_CurrentQuarter = Mathf.Clamp((indexQuarter + 1), 0, 4);
        UpdateUILifebar();
    }

    public void UpdateUILifebar()
    {
        ActiveBufferHealth(Time.time, m_CurrentHealth);
        m_SliderCurrentHealthHighBuffer.fillAmount = m_CurrentHealth / m_MaxHealthQuantity;
        m_SliderCurrentQuarterHigh.fillAmount = 1 / m_QuarterNumber * (m_QuarterNumber - m_CurrentQuarter);
    }

    public void RestoreFullLife()
    {

        m_CurrentHealth = m_MaxHealthQuantity;
        m_CurrentQuarter = 4;
        UpdateUILifebar();
    }

    public void RestoreHealQuarter()
    {
        m_CurrentHealth += m_QuarterHealthQuantity;
        if (m_CurrentHealth > m_MaxHealthQuantity)
        {
            ActiveBufferHealth(Time.time, m_CurrentHealth);
            m_CurrentHealth = m_MaxHealthQuantity;
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
        if (collision.transform.tag != "Enemy" || !GameState.IsPlaying()) return;
        // Debug.Log("Object was an Enemy !");
        //   GetLightDamage(2,collision.transform.position);
    }

    private void ActiveBufferHealth(float time, float health)
    {
        timeLastHit = time;
        healthBuffer = true;
        lastHealth = health;
    }

    private void BufferXpDisplay()
    {
        m_SliderCurrentHealthHigh.fillAmount = Mathf.Lerp(lastHealth / m_MaxHealthQuantity, m_CurrentHealth / m_MaxHealthQuantity, (Time.time -timeLastHit) / 0.5f);
    }

    public void OnCollisionEnter(Collision collision)
    { 
        if (collision.collider.tag == "Enemy")
        {
            OnContactEvent(collision.transform.position, EntitiesTrigger.Enemies, collision.gameObject);
        }
    }

    public void ActiveSlowEffect(float time, int quarter)
    {
        slowDownActive = true;
        m_DurationSlowdownEffect = time;
        tempsEcouleSlowEffect = 0;
        GlobalSoundManager.PlayOneShot(50, transform.position);
        if (quarter > 1)
        {
            m_currentAnimationCurveToUse = m_SlowdownEffectCurve;
        }
        else
        {
            m_DurationSlowdownEffect += 1;
            m_currentAnimationCurveToUse = m_SlowdownEffectCurveLastQuarter;
        }
    }

}
