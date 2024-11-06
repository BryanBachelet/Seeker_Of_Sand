using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class GlobalSoundManager : MonoBehaviour
{
    static EventInstance[] everyEvent;
    public EventInstance[] everyEvent_Attribution;


    static EventReference[] OneS_Sound;

    public EventReference[] OneS_Sound_Attribution;

    public EventReference Globalsound;
    public EventInstance globalinstance;


    public EventReference GlobalMusic;
    public EventInstance globalMusicInstance;

    public EventReference musicIntensity;
    public EventInstance musicIntensityInstance;

    public EventReference musicIntensityTransition;
    public EventInstance musicIntensityTransitionInstance;

    public EventReference marchandMusic;
    public EventInstance marchandMusicInstance;

    public EventReference canalisationReference;
    public EventInstance canalisationInstance;

    public bool changing = false;

    public float delayMusic = 0;

    private static GlobalSoundManager thisGSM;
    // Start is called before the first frame update
    void Awake()
    {
        thisGSM = this.gameObject.GetComponent<GlobalSoundManager>();
        everyEvent = everyEvent_Attribution;
        OneS_Sound = OneS_Sound_Attribution;
        StartCoroutine(StartAmbiant(delayMusic));

    }

    public void ReceiveButtonInput(int sNumber)
    {
        PlayOneShot(sNumber, this.transform.position);
    }
    public static void PlayOneShot(int sNumber, Vector3 position)
    {

        if(sNumber>= OneS_Sound.Length)
        {
          //  Debug.LogError("[FMOD] Event not found because the number calls is out of  bound ");
            return;
        } 
            
        try
        {   
            RuntimeManager.PlayOneShot(OneS_Sound[sNumber], position);
        }
        catch (EventNotFoundException)
        {
            Debug.LogWarning("[FMOD] Event not found: " + OneS_Sound[sNumber]);
        }
    }

    public void UpdateParameter(float parameterValue, string parameterName)
    {
        globalinstance.setParameterByName(parameterName, parameterValue);
        globalMusicInstance.setParameterByName(parameterName, parameterValue);
        musicIntensityInstance.setParameterByName(parameterName, parameterValue);
    }

    //public IEnumerator UpdateParameterWithDelay(float parameterValue, string parameterName, float delay, float beforeDelayParameterValue, string beforeDelayParameterName)
    //{
    //    globalinstance.setParameterByName(parameterName, parameterValue);
    //    globalMusicInstance.setParameterByName(parameterName, parameterValue);
    //    musicIntensityInstance.setParameterByName(parameterName, parameterValue);
    //    musicIntensityTransitionInstance.setParameterByName(beforeDelayParameterName, 0);
    //}

    public IEnumerator StartAmbiant(float delay)
    {
        yield return new WaitForSeconds(delay);
        globalinstance = RuntimeManager.CreateInstance(Globalsound); 
        //globalinstance.start();
        //globalinstance.setVolume(1);
        //globalMusicInstance = RuntimeManager.CreateInstance(GlobalMusic);
        ////globalMusicInstance.start();
        //globalMusicInstance.setVolume(1);
        musicIntensityInstance = RuntimeManager.CreateInstance(musicIntensity);
        musicIntensityInstance.setVolume(1);
        musicIntensityTransitionInstance = RuntimeManager.CreateInstance(musicIntensityTransition);
        musicIntensityTransitionInstance.setVolume(1);
        marchandMusicInstance = RuntimeManager.CreateInstance(marchandMusic);
        //marchandMusicInstance.start();
        marchandMusicInstance.setVolume(0);
        StartAmbiantNoDelay();
    }

    public void OnDisable()
    {
        //globalinstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        globalMusicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        musicIntensityInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        musicIntensityTransitionInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        canalisationInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    public void CanalisationParameterLaunch(float canalisationState, float element)
    {
        canalisationInstance = RuntimeManager.CreateInstance(canalisationReference);
        canalisationInstance.setParameterByName("CanalisationState", canalisationState);
        //Debug.Log(element);
        canalisationInstance.setParameterByName("Element", element);
        canalisationInstance.start();
    }

    public void CanalisationParameterStop()
    {
        canalisationInstance.setParameterByName("CanalisationState", 0);
        canalisationInstance.setParameterByName("Element", 0);
        globalinstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    static public void SwitchAmbiantToMarchand(bool activeMarchand)
    {
        if(activeMarchand)
        {
            thisGSM.ActiveMarchand();
        }
        else
        {
            thisGSM.DisactiveMarchand();
        }
    }

    private void ActiveMarchand()
    {
        StopAmbiantNoDelay();
    }
    private void DisactiveMarchand()
    {
        StartAmbiantNoDelay();
    }

    public void StartAmbiantNoDelay()
    {
        //globalinstance = RuntimeManager.CreateInstance(Globalsound);
        globalinstance.start();
        globalinstance.setVolume(1);
        //globalMusicInstance = RuntimeManager.CreateInstance(GlobalMusic);
        //globalMusicInstance.start();
        //globalMusicInstance.setVolume(1);
        musicIntensityInstance.start();
        musicIntensityInstance.setVolume(1);
        UpdateParameter(0.1f, "Intensity");
        musicIntensityTransitionInstance.start();
        musicIntensityTransitionInstance.setVolume(1);
        //marchandMusicInstance = RuntimeManager.CreateInstance(marchandMusic);
        marchandMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        marchandMusicInstance.setVolume(0);
    }

    public void StopAmbiantNoDelay()
    {
        //globalinstance = RuntimeManager.CreateInstance(Globalsound);
        globalinstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        globalinstance.setVolume(0);
        //globalMusicInstance = RuntimeManager.CreateInstance(GlobalMusic);
        //globalMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        //globalMusicInstance.setVolume(0);
        musicIntensityInstance.start();
        musicIntensityInstance.setVolume(0);
        UpdateParameter(0f, "Intensity");
        //marchandMusicInstance = RuntimeManager.CreateInstance(marchandMusic);
        marchandMusicInstance.start();
        marchandMusicInstance.setVolume(1);
    }
}
