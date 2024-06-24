using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using UnityEngine.UI;

public class ShadowFunction : MonoBehaviour
{
    public bool onShadow = false;
    public bool outShadow = false;

    static public bool onShadowStatic = false;
    static public bool outShadowStatic = false;

    [SerializeField] private float m_TimeOnShadow = 0;
    [SerializeField] private float m_TimeBeforeDetection = 2;
    static public bool onShadowSpawnStatic = false;
    private bool onShadowSpawn = false;

    [SerializeField] private float m_TimeOutShadow = 0;
    [SerializeField] private float m_TimeBeforeStopDetection = 2;
    static public bool outShadowSpawnStatic = false;
    private bool outShadowSpawn = false;

    public Animator detectionAnimator;
    public UnityEngine.VFX.VisualEffect vfxDetection;

    public EventReference Globalsound;
    public EventInstance shadowDetection;

    public EventReference activationShadowDetection_Attribution;

    public bool IsDebugActive = false;
    public Enemies.EnemyManager enemyManager;

    public Sprite[] spritesEyes;
    public AnimationCurve spriteAnimation;

    public Sprite currentSpriteDetection;
    public Image[] spriteUsed = new Image[2];
    public Image[] spriteProgress = new Image[4]; //Progression fill effect image
    private float progressTimeShadow = 0;
    private int signedProgress = 0;

    public Animator newAnimatorEyes;
    // Start is called before the first frame update
    void Start()
    {
        shadowDetection = RuntimeManager.CreateInstance(Globalsound);
    }
    //ShadowDetection
    // Update is called once per frame
    void Update()
    {
        if (onShadow)
        {
            if (m_TimeOnShadow < m_TimeBeforeDetection)
            {
                m_TimeOnShadow += Time.deltaTime;
                progressTimeShadow = m_TimeOnShadow / m_TimeBeforeDetection;
                if (vfxDetection.HasInt("Rate")) vfxDetection.SetInt("Rate", (int)(progressTimeShadow * 5));
                currentSpriteDetection = spritesEyes[(int)spriteAnimation.Evaluate(progressTimeShadow)];
                newAnimatorEyes.SetBool("OnShadowEnter", false);
                newAnimatorEyes.SetBool("OnShadowExit", false);
            }
            else
            {
                if (!onShadowSpawn)
                {
                    m_TimeOutShadow = 0;
                    StopDetectionSoundFeedback();
                    if (vfxDetection.HasFloat("Rate")) vfxDetection.SetInt("Rate", 0);
                    RuntimeManager.PlayOneShot(activationShadowDetection_Attribution, transform.position);
                    GlobalSoundManager.PlayOneShot(40, transform.position);
                    onShadowSpawn = true;
                    onShadowSpawnStatic = true;
                    outShadowSpawn = false;
                    outShadowSpawnStatic = false;
                    newAnimatorEyes.SetBool("OnShadowEnter", true);
                    newAnimatorEyes.SetTrigger("Enter");
                    //enemyManager.ActiveSpawnPhase(true,Enemies.EnemySpawnCause.SHADOW);
                }

            }

        }
        else
        {
            if (m_TimeOutShadow < m_TimeBeforeStopDetection)
            {
              if(vfxDetection.HasInt("Rate"))  vfxDetection.SetInt("Rate", 0);
                newAnimatorEyes.SetBool("OnShadowExit", false);
                newAnimatorEyes.SetBool("OnShadowEnter", false);
                m_TimeOutShadow += Time.deltaTime;
                progressTimeShadow = 1 - (m_TimeOutShadow / m_TimeBeforeStopDetection);
                currentSpriteDetection = spritesEyes[(int)spriteAnimation.Evaluate(progressTimeShadow)];
            }
            else
            {
                if (!outShadowSpawn && !DayCyclecontroller.isNightState)
                {
                    m_TimeOnShadow = 0;
                    StopDetectionSoundFeedback();
                    outShadowSpawn = true;
                    outShadowSpawnStatic = true;
                    onShadowSpawn = false;
                    onShadowSpawnStatic = false;
                    newAnimatorEyes.SetTrigger("Exit");
                    enemyManager.ActiveSpawnPhase(false,Enemies.EnemySpawnCause.SHADOW);
                }

            }
        }
        ActualiseSprites(currentSpriteDetection,  progressTimeShadow);


    }

    public void OnShadow()
    {
        onShadow = true;
        outShadow = false;

        if(!onShadowSpawn) { StartDetectionSoundFeedback(); }
        //onShadowStatic = onShadow;
        //outShadowStatic = outShadow;
     if(IsDebugActive)   Debug.Log("On Shadow enter !");
    }

    public void OutShadow()
    {
        StopDetectionSoundFeedback();
        onShadow = false;
        outShadow = true;
        //onShadowStatic = onShadow;
        //outShadowStatic = outShadow;
        if (IsDebugActive) Debug.Log("On Shadow out !");
    }

    public void OnEnterShadow()
    {
        m_TimeOnShadow = 0;
    }
    public void OnExitShadow()
    {
        m_TimeOutShadow = 0;
    }
    public void StartDetectionSoundFeedback()
    {
        shadowDetection.start();
    }

    public void StopDetectionSoundFeedback()
    {
        shadowDetection.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void ActualiseSprites(Sprite sprite, float progress)
    {
        spriteUsed[0].sprite = currentSpriteDetection;
        spriteProgress[0].fillAmount = progress;
        spriteProgress[1].fillAmount = progress;
        spriteProgress[2].fillAmount = progress;
        spriteProgress[3].fillAmount = progress;
    }

    public void ResetPlayerShadowStatus()
    {
        m_TimeOnShadow = 0;
        StopDetectionSoundFeedback();
        outShadowSpawn = true;
        outShadowSpawnStatic = true;
        onShadowSpawn = false;
        onShadowSpawnStatic = false;
        newAnimatorEyes.SetTrigger("Exit");
        enemyManager.ActiveSpawnPhase(false, Enemies.EnemySpawnCause.SHADOW);
    }

}
