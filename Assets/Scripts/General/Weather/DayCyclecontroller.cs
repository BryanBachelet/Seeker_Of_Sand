using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;
using TMPro;
using Character;

public class DayCyclecontroller : MonoBehaviour
{
    [SerializeField] public VolumeProfile volumeProfile;
    [SerializeField] public Volume volumePP;
    [SerializeField] private CubemapParameter[] cubemapForSky;
    [SerializeField] private AnimationCurve m_cubeMapIntensityPerHour;
    [SerializeField] private AnimationCurve m_ShadowOpacityByHour;
    VolumetricClouds vClouds;
    CloudLayer vCloudLayer;
    Exposure vExposure;
    DepthOfField vDepthOfField;
    PhysicallyBasedSky vSpaceEmissionTexture;
    [SerializeField] private AnimationCurve m_OpacityRByHour;
    [SerializeField] private AnimationCurve m_RotationByHour;
    [SerializeField] private AnimationCurve m_ShadowMultiplierByHour;
    [SerializeField] private AnimationCurve m_colorTemperatureOverTime;
    [SerializeField] private AnimationCurve m_ExposureCompensationByHour;
    [Range(0, 24)]
    [SerializeField] public float m_timeOfDay;
    static public float staticTimeOfTheDay;
    [SerializeField] private Light m_sun;
    [SerializeField] private Light m_moon;
    [SerializeField] private float m_SettingDurationDay = 10; // Correspond au nombre de minute IRL de la dur�e d'une journ�e in-game, de base 10 minutes
    [SerializeField] public float m_orbitSpeed = 1.0f; // Correponds � la vitesse d'�coulement du temps in-game. 1 reviens � avoir une journ�e de 24 secondes IRL
    [SerializeField] private GlobalSoundManager m_GSM;
    [SerializeField] static public float durationDay;
    [SerializeField] static public float durationNight;
    [SerializeField] private Volume m_LocalNightVolume;
    [SerializeField] private Enemies.EnemyManager m_EnemyManager;
    [SerializeField] private float time;
    [SerializeField] public TMP_Text m_DayPhases;
    [SerializeField] public TMP_Text m_Instruction;
    [SerializeField] public Animator m_instructionAnimator;
    [SerializeField] public Image m_daySlider;
    public bool isNight = false;
    public static bool isNightState = false;
    public float timescale;

    [SerializeField] float[] tempsChaquePhase;
    [SerializeField] string[] instructionPhase;
    [SerializeField] string[] nomChaquePhase;
    [SerializeField] float[] heureChaquePhase;
    [SerializeField] string[] nomHeureChaquePhase;
    [SerializeField] int currentPhase = 0;
    private int lastPhaseChecked = 0;
    [SerializeField] float m_TimeTransitionLastPhase;
    [SerializeField] float m_TimeProchainePhase;

    [SerializeField] private GameObject m_EndUI;
    [SerializeField] public int m_nightCount { get; private set; }
    public static int m_nightCountGlobal;
    private string dayprogress;
    private string phaseprogress;

    // Night Event
    public delegate void NightBeginning();
    public event NightBeginning nightStartEvent;
    // Day Event
    public delegate void DayBeginning();
    public event DayBeginning dayStartEvent;

    [SerializeField] public TMP_Text m_DayRemain;
    public Animator dayAnoncerAnimator;
    private bool dayChanged = false;
    bool checkNightSound = false;
    public static bool choosingArtefactStart = true;
    public bool choosingArtefactDisplay = true;
    public Chosereward choseReward;

    public bool menuMovement = false;

    public bool timeByGeneration = true;

    public float speed;
    public bool newDay = false;
    public TMPro.TMP_Text dayText;

    public Texture2D[] moonTexture = new Texture2D[3];
    // Start is called before the first frame update

    public GameObject AiguilleRotation;
    public GameObject horlogeDay;
    public GameObject horlogeNight;

    public Vector3[] positionAiguille;
    public Vector3[] rotationAiguille;

    public Vector3[] positionDay;
    public Vector3[] positionNight;

    public Vector3[] rotationDay;
    public Vector3[] rotationNight;
    void Start()
    {
        //newDay = true;

        m_nightCount = 0;
        dayText.text = "Day " + (m_nightCount + 1); 
        m_nightCountGlobal = 0;
        time = 0;
        m_orbitSpeed = 24 / (m_SettingDurationDay * 60); //on divise 24 (nombre d'heure) par le nombre de secondes qui vont s'�couler IRL.  On multiplie le nombre de minutes r�gl�e dans l'inspector par 60 pour le convertire en seconde.
        durationNight = m_SettingDurationDay / 3;
        durationDay = durationNight * 2;
        Time.timeScale = timescale;
        if (currentPhase < 0) currentPhase = 0;
        m_TimeProchainePhase = time + tempsChaquePhase[currentPhase];
        //AiguilleRotation.transform.Rotate(new Vector3(0, 0, 75));

    }

    // Update is called once per frame
    void Update()
    {
        //choosingArtefactStart = choosingArtefactDisplay;
        m_nightCountGlobal = m_nightCount;
        if (menuMovement)
        {
            staticTimeOfTheDay = m_timeOfDay;
            UpdateTime();
        }
        //Debug.Log("Test Phase 1");
        if (choosingArtefactStart) return;
        //Debug.Log("Test Phase 2");
        if (!GameState.IsPlaying()) return;
        //Debug.Log("Test Phase 3");

        if (timeByGeneration)
        {
            //UpdateTimeByStep();
        }
        else
        {
            UpdateTimeBySecond();
        }

    }

    private void OnValidate()
    {
        UpdateTime();

    }

    private void UpdateTimeBySecond()
    {
        time += Time.deltaTime;
        CheckPhase(m_timeOfDay);
        staticTimeOfTheDay = m_timeOfDay;
        UpdateTime();
    }


    public void UpdateTimeByStep()
    {
        time += 600;
        CheckPhase(m_timeOfDay);
        staticTimeOfTheDay = m_timeOfDay;
        UpdateTime();
    }
    private void UpdateTime()
    {
        float alpha = m_timeOfDay / 24.0f;
        float sunRotation = Mathf.Lerp(-90, 270, alpha);


        float moonRotation = sunRotation - 180;
        if (Application.isPlaying)
        {
            AiguilleRotation.transform.Rotate(rotationAiguille[currentPhase]);
        }

        m_sun.transform.rotation = Quaternion.Euler(sunRotation, -149, 0);
        m_moon.transform.rotation = Quaternion.Euler(moonRotation, -149, 0);

        AdjustPostProcessByHour(m_timeOfDay);
        //UpdatePostProcess();
        if (!Application.isPlaying) return;
        if (m_timeOfDay > 5.12f && m_timeOfDay < 18.5f)
        {
            if (m_moon.isActiveAndEnabled)
            {
                m_moon.enabled = false;
                m_DayRemain.text = "Day " + (1 + m_nightCount) + " ...";
                dayChanged = true;
                dayAnoncerAnimator.SetBool("ActiveDay", true);
                choseReward.activeGeneration = true;
            }

        }
        else
        {
            if (!m_moon.isActiveAndEnabled)
            {
                m_moon.GetComponent<HDAdditionalLightData>().surfaceTexture = moonTexture[m_nightCount];
                m_moon.enabled = true;
                dayChanged = false;
                dayAnoncerAnimator.SetBool("ActiveDay", false);
            }
        }
        CheckingNightDayTransition();
    }

    private void UpdatePostProcess()
    {
        ShadowOpacityAdjustByHour(m_timeOfDay);
        OpacityRAdjustByHour(m_timeOfDay);
        RotationAdjustByHour(m_timeOfDay);
        ShadowMultiplierAdjustByHour(m_timeOfDay);
    }

    public bool IsNextRoomIsDay()
    {
        return m_timeOfDay == 0 || m_timeOfDay == 24;
    }

    public float GetNextHour()
    {
        if (currentPhase < tempsChaquePhase.Length)
            return heureChaquePhase[currentPhase + 1];
        else
            return heureChaquePhase[0];
    }
    private void CheckingNightDayTransition()
    {
        if (isNight)
        {
            if (m_moon.transform.rotation.eulerAngles.x > 180)
            {
                StartDay();
                if (m_nightCount == 3)
                {
                    GameState.LaunchEndMenu();
                    m_EndUI.SetActive(true);
                }
            }

        }
        else
        {
            if (m_sun.transform.rotation.eulerAngles.x > 180)
            {
                StartNight();


            }
            else
            {
                newDay = false;
            }
        }
    }

    private void StartDay()
    {
        m_sun.gameObject.SetActive(true);
        horlogeDay.transform.position = positionDay[0];
        horlogeDay.transform.Rotate(rotationDay[0]);
        horlogeNight.transform.position = positionDay[1];
        horlogeNight.transform.Rotate(rotationDay[1]);
        isNight = false;
        isNightState = isNight;
        if (dayStartEvent != null) dayStartEvent.Invoke();
        m_sun.shadows = LightShadows.Soft;
        m_moon.shadows = LightShadows.None;
        m_GSM.UpdateParameter(0, "DayOrNight");
        //m_LocalNightVolume.enabled = false;
        m_moon.gameObject.SetActive(false);
        checkNightSound = false;
        if (cubemapForSky.Length >= 1)
        {
            //vSpaceEmissionTexture.spaceEmissionTexture = cubemapForSky[0];
            //vSpaceEmissionTexture.Override(vSpaceEmissionTexture, 1);
        }
        //vSpaceEmissionTexture.
        m_nightCount++;
        m_nightCountGlobal++;
        m_EnemyManager.m_playerTranform.GetComponent<CharacterDash>().gainDash(1, true);
        dayText.text = "Day " + (m_nightCount + 1);
        newDay = true;
    }

    private void StartNight()
    {
        m_moon.gameObject.SetActive(true);
        horlogeDay.transform.position = positionNight[0];
        horlogeDay.transform.Rotate(rotationNight[0]);
        horlogeNight.transform.position = positionNight[1];
        horlogeNight.transform.Rotate(rotationNight[1]);
        isNight = true;
        isNightState = isNight;
        if (nightStartEvent != null) nightStartEvent.Invoke();
        m_GSM.UpdateParameter(1, "DayOrNight");
        //m_GSM.globalMusicInstance.setParameterByName("Repos", 1);
        StartCoroutine(DisplayInstruction("Night fall", 2, Color.white, ""));
        //  GlobalSoundManager.PlayOneShot(34, transform.position);
        //m_LocalNightVolume.enabled = true;
        m_sun.shadows = LightShadows.None;
        m_moon.shadows = LightShadows.Soft;
        m_sun.gameObject.SetActive(false);
        //if (volumePP.profile.TryGet<PhysicallyBasedSky>(out vSpaceEmissionTexture))
        //{
        //    if (cubemapForSky.Length >= 2)
        //    {
        //        //vSpaceEmissionTexture.spaceEmissionTexture = cubemapForSky[1];
        //        //vSpaceEmissionTexture.Override(vSpaceEmissionTexture, 1);
        //    }
        //}
    }

    public void CheckPhase(float hour)
    {
        UpdatePhaseInfo();

        if (currentPhase != lastPhaseChecked)
        {
            m_DayPhases.text = dayprogress + "" + phaseprogress;
            StartCoroutine(DisplayInstruction(instructionPhase[currentPhase], 2, Color.white, ""));
            lastPhaseChecked = currentPhase;
        }
        //AiguilleRotation.transform.position = positionAiguille[currentPhase];

    }

    public void UpdatePhaseInfo()
    {
        if (time > m_TimeProchainePhase)
        {
            currentPhase += 1;
            if (currentPhase > tempsChaquePhase.Length - 1)
            {
                currentPhase = 0;
            }
            m_TimeProchainePhase = time + tempsChaquePhase[currentPhase];
            m_TimeTransitionLastPhase = time;
            phaseprogress = nomChaquePhase[currentPhase];
            dayprogress = nomHeureChaquePhase[currentPhase];
            m_DayPhases.text = dayprogress + "" + phaseprogress;

        }
        float sliderValue = 1 - ((m_TimeProchainePhase - time) / tempsChaquePhase[currentPhase]);
        if (sliderValue <= 1 && sliderValue >= 0)
        {
            m_daySlider.fillAmount = sliderValue;
        }
        if (currentPhase < tempsChaquePhase.Length)
        {
            m_timeOfDay = Mathf.Lerp(heureChaquePhase[currentPhase], heureChaquePhase[currentPhase + 1], sliderValue);
        }
        else
        {
            m_timeOfDay = Mathf.Lerp(heureChaquePhase[currentPhase], heureChaquePhase[0], sliderValue);
        }

        if (m_timeOfDay >= 24)
        {
            m_timeOfDay = 0;
        }
        else if (m_timeOfDay > 5.5f && m_timeOfDay <= 8f)
        {
            //StartDay();
            //m_timeOfDay = 6.1f;
        }
        else if (m_timeOfDay > 17f && m_timeOfDay < 20f)
        {
            //m_timeOfDay = 18.5f;
            if (!checkNightSound)
            {
                checkNightSound = true;

            }

        }

        //if (currentPhase == 1 || currentPhase == 4 || currentPhase == 7) { m_EnemyManager.ChangeSpawningPhase(true); }
        //else { m_EnemyManager.ChangeSpawningPhase(false); }


    }

    public IEnumerator DisplayInstruction(string instruction, float time, Color colorText, string locationName)
    {
        m_Instruction.color = colorText;
        m_Instruction.text = "" + locationName + "" + instruction;
        m_instructionAnimator.SetTrigger("DisplayInstruction");
        yield return new WaitForSeconds(time);
        m_instructionAnimator.ResetTrigger("DisplayInstruction");
    }

    //public void Sparse()
    //{
    //    if (volumeProfile.TryGet<VolumetricClouds>(out vClouds))
    //    {
    //        vClouds.cloudPreset.value = VolumetricClouds.CloudPresets.Sparse;
    //        Debug.Log(vClouds.cloudPreset);
    //    }
    //
    //}
    //public void Cloudy()
    //{
    //    if (volumeProfile.TryGet<VolumetricClouds>(out vClouds))
    //    {
    //        vClouds.cloudPreset.value = VolumetricClouds.CloudPresets.Cloudy;
    //        Debug.Log(vClouds.cloudPreset);
    //    }
    //
    //}
    //public void Overcast()
    //{
    //    if (volumeProfile.TryGet<VolumetricClouds>(out vClouds))
    //    {
    //        vClouds.cloudPreset.value = VolumetricClouds.CloudPresets.Overcast;
    //        Debug.Log(vClouds.cloudPreset);
    //    }
    //
    //}
    //public void Stormy()
    //{
    //    if (volumeProfile.TryGet<VolumetricClouds>(out vClouds))
    //    {
    //        vClouds.cloudPreset.value = VolumetricClouds.CloudPresets.Stormy;
    //        Debug.Log(vClouds.cloudPreset);
    //    }
    //
    //}
    public void ShadowOpacityAdjustByHour(float hour)
    {
        if (volumeProfile.TryGet<VolumetricClouds>(out vClouds))
        {
            vClouds.shadowOpacity.value = m_ShadowOpacityByHour.Evaluate(hour);
        }

    }
    public void OpacityRAdjustByHour(float hour)
    {
        if (volumePP.profile.TryGet<CloudLayer>(out vCloudLayer))
        {
            vCloudLayer.layerA.opacityR.value = m_OpacityRByHour.Evaluate(hour);
        }

    }


    public void RotationAdjustByHour(float hour)
    {
        if (volumePP.profile.TryGet<CloudLayer>(out vCloudLayer))
        {
            vCloudLayer.layerA.rotation.value = m_RotationByHour.Evaluate(hour);
        }

    }

    public void ShadowMultiplierAdjustByHour(float hour)
    {
        if (volumePP.profile.TryGet<CloudLayer>(out vCloudLayer))
        {
            vCloudLayer.shadowMultiplier.value = m_ShadowMultiplierByHour.Evaluate(hour);
        }

    }

    public void AdjustPostProcessByHour(float hour)
    {
        if (volumePP)
        {
            if (volumePP.profile.TryGet<CloudLayer>(out vCloudLayer))
            {
                //vClouds.shadowOpacity.value = m_ShadowOpacityByHour.Evaluate(hour);
                vCloudLayer.layerA.opacityR.value = m_OpacityRByHour.Evaluate(hour);
                vCloudLayer.layerA.rotation.value = m_RotationByHour.Evaluate(hour);
                vCloudLayer.shadowMultiplier.value = m_ShadowMultiplierByHour.Evaluate(hour);

                //Debug.Log("Cloud Layer Debug : (" + vCloudLayer.layerA.opacityR.value + ") opacity || (" + vCloudLayer.layerA.rotation.value + ") rotation || (" + vCloudLayer.shadowMultiplier.value + ") shadowMultiplier");
            }
            if (m_ExposureCompensationByHour != null && volumePP.profile.TryGet<Exposure>(out vExposure))
            {
                vExposure.compensation.value = m_ExposureCompensationByHour.Evaluate(hour);
            }
            if (m_cubeMapIntensityPerHour != null && volumePP.profile.TryGet<PhysicallyBasedSky>(out vSpaceEmissionTexture))
            {
                vSpaceEmissionTexture.spaceEmissionMultiplier.value = m_cubeMapIntensityPerHour.Evaluate(hour);
            }
        }
        if (m_sun && m_colorTemperatureOverTime != null)
        {
            m_sun.colorTemperature = m_colorTemperatureOverTime.Evaluate(hour);
        }
    }

    public void UpdateDepthOfField(bool stateEffet)
    {
        if (volumePP.profile.TryGet<DepthOfField>(out vDepthOfField))
        {
            vDepthOfField.active = stateEffet;
        }
    }
}


