using Character;
using Enemies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class DayTimeController : MonoBehaviour
{
    public bool debugTime = false;

    public int currentPhase = 0;
    public float currentHour = 7;
    public float[] hourPerPhase = new float[2];
    private bool isNight = false;
    private bool isDay = true;
    public int dayCount = 0;
    public bool newDay = true;

    [SerializeField] private Light m_sun;
    [SerializeField] private Light m_moon;

    #region Post Processing
    [SerializeField] private CubemapParameter[] cubemapForSky;
    [SerializeField] private AnimationCurve m_cubeMapIntensityPerHour;
    [SerializeField] private AnimationCurve m_ShadowOpacityByHour;
    [SerializeField] public VolumeProfile volumeProfile;
    [SerializeField] public Volume volumePP;
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
    #endregion

    public TMPro.TMP_Text dayText;

    [HideInInspector] public GameObject m_Player;

    // Night Event
    public delegate void NightBeginning();
    public event NightBeginning nightStartEvent;
    // Day Event
    public delegate void DayBeginning();
    public event DayBeginning dayStartEvent;
    // Start is called before the first frame update
    void Start()
    {
        isNight = false;
        isDay = true;
        dayCount = 1;
        currentPhase = 0;
        currentHour = hourPerPhase[currentPhase];
        m_Player = GameObject.Find("Player");
        //UpdateNextPhase();
        UpdateTime(currentHour);
    }

    // Update is called once per frame
    void Update()
    {
        if(debugTime)
        {
            debugTime =false;
            UpdateNextPhase();
        }
    }

    public void UpdateNextPhase()
    {
        if (currentPhase + 1 > hourPerPhase.Length -1)
        {
            currentPhase = 0;
            currentHour = hourPerPhase[currentPhase];
            //Launch boss in next room
        }
        else
        {
            currentPhase = currentPhase + 1;
            currentHour = hourPerPhase[currentPhase];
            if (isNight)
            {
                if (currentHour > 5 && currentHour < 18)
                {
                    isNight = false;
                    isDay = true;
                    StartDay();
                }
            }
            else if (isDay)
            {
                if (currentHour < 5 || currentHour > 18)
                {
                    isNight = true;
                    isDay = false;
                    StartNight();
                }
            }
        }
        UpdateTime(currentHour);
    }

    public void UpdateTime(float Hour)
    {
        float dayProgress = Hour / 24.0f; //Valeur ramené entre 0 et 1. 0.5 = 12H
        float sunRotation = Mathf.Lerp(-90, 270, dayProgress); //Valeur de la rotation X du gameobject qui contient le component Light du soleil.
        float moonRotation = sunRotation - 180; //Valeur de la rotation X du gameobject qui contient le component Light de la lune. A l'opposé du soleil
        //if (Application.isPlaying)
        //{
        //    AiguilleRotation.transform.Rotate(rotationAiguille[currentPhase]);
        //}

        m_sun.transform.rotation = Quaternion.Euler(sunRotation, -149, 0);
        m_moon.transform.rotation = Quaternion.Euler(moonRotation, -149, 0);

        AdjustPostProcessByHour(Hour);
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

    private void StartDay()
    {
        m_sun.gameObject.SetActive(true);
        if (dayStartEvent != null) dayStartEvent.Invoke();
        m_sun.shadows = LightShadows.Soft;
        m_moon.shadows = LightShadows.None;
        m_moon.gameObject.SetActive(false);
        dayCount++;
        m_Player.GetComponent<CharacterDash>().gainDash(1, true);
        dayText.text = "Day " + dayCount;
        newDay = true;
        m_moon.enabled = false;
        m_sun.enabled = true;

    }

    private void StartNight()
    {
        m_moon.gameObject.SetActive(true);
        if (nightStartEvent != null) nightStartEvent.Invoke();
        m_sun.shadows = LightShadows.None;
        m_moon.shadows = LightShadows.Soft;
        m_sun.gameObject.SetActive(false);
        m_sun.enabled = false;
        m_moon.enabled = true;

    }

}
