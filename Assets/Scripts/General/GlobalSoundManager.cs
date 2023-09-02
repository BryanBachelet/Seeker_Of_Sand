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

    public bool changing = false;

    public float delayMusic = 0;
    // Start is called before the first frame update
    void Awake()
    {
        everyEvent = everyEvent_Attribution;
        OneS_Sound = OneS_Sound_Attribution;
        StartCoroutine(StartAmbiant(delayMusic));

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
    }

    public IEnumerator StartAmbiant(float delay)
    {
        yield return new WaitForSeconds(delay);
        globalinstance = RuntimeManager.CreateInstance(Globalsound);
        globalinstance.start();
        globalMusicInstance = RuntimeManager.CreateInstance(GlobalMusic);
        globalMusicInstance.start();
    }

}
