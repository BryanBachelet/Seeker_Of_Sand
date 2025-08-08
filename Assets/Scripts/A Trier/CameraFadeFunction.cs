using Render.Camera;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFadeFunction : MonoBehaviour
{
    [SerializeField] private MeshRenderer m_fadeObject;
    [HideInInspector] private Material fadeMat;

    [HideInInspector] private float fadeProgress = 0;
    [Range(1,5)] [SerializeField] private float m_fadeInSecond = 1; //1 = 1 seconde ; 2 = 2 secondes
    [HideInInspector] private bool fadeInActive = false;
    [HideInInspector] private bool fadeInActivation = false;

    [HideInInspector] private bool fadeOutActive = false;
    [HideInInspector] private bool fadeOutActivation = true;
    public bool manuelFade = false;

    [SerializeField] public TeleporterBehavior tpBehavior;
    [SerializeField] public Animator bandeNoir;

    [SerializeField] public Animator dayTextAnimator;
    [SerializeField] public GameObject dayTextObj;

    [SerializeField] public GameTutorialView gameTutorialView;
    private bool m_isFirstTime = false;

    public CameraBehavior cameraBehavior;
    public Camera renderCam;
    // Start is called before the first frame update
    void Start()
    {
        if(cameraBehavior == null) this.gameObject.GetComponent<CameraBehavior>();
        fadeMat = m_fadeObject.material;
        LaunchFadeOut(true, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (manuelFade)
        {
            ChangeFadeAlpha(fadeProgress);
            return;
        }
        if (fadeInActivation)
        {
            fadeInActive = true;
            fadeInActivation = false;
            fadeProgress = 0;
        }
        if (fadeInActive)
        {
            if (fadeProgress < 0.99f)
            {
                fadeProgress += Time.deltaTime * m_fadeInSecond;
            }
            else
            {
                fadeProgress = 1;
                fadeInActive = false;
                tpBehavior.ActivationTeleportation();
                dayTextObj.SetActive(false);
                //LaunchFadeOut(true, 1);


            }
            ChangeFadeAlpha(fadeProgress);
        }
        if (fadeOutActivation)
        {
            fadeOutActive = true;
            fadeOutActivation = false;
            fadeProgress = 1;
        }
        if (fadeOutActive)
        {
            if (fadeProgress > 0.01f)
            {
                fadeProgress -= Time.deltaTime * m_fadeInSecond;
            }
            else
            {
                m_fadeObject.gameObject.SetActive(false);
                renderCam.gameObject.SetActive(false);
                fadeProgress = 0;
                fadeOutActive = false;
                dayTextObj.SetActive(false);
                if (m_isFirstTime)
                {
                    if (GameManager.instance.generalSaveData.IsFirstTime)
                    {
                        gameTutorialView.StartTutoriel();
                    }
                    else
                    {
                        GameState.ChangeState();
                    }
                    m_isFirstTime = false;
                }
                //dayTextAnimator.ResetTrigger("NewDay");
            }
            ChangeFadeAlpha(fadeProgress);
        }
    }

    public void LaunchGame()
    {
        //GameState.ChangeState();
        //GameManager.instance.generalSaveData.IsFirstTime = false;
    }

    private void ChangeFadeAlpha(float alphaValue)
    {
        fadeMat.SetColor("_UnlitColor", new Color(0, 0, 0, alphaValue));
        if (fadeInActive)
        {
            cameraBehavior.m_distanceToTarget = Mathf.Lerp(150, 15, alphaValue);
        }
        else if (fadeOutActive)
        {
            cameraBehavior.m_distanceToTarget = Mathf.Lerp(150, 450, alphaValue);
        }

    }
    public void LaunchFadeIn(bool stateFade, float speedFade)
    {

        fadeInActivation = stateFade;
        m_fadeInSecond = speedFade;
        Debug.Log("Launch Fade in " + stateFade);
    }

    public void LaunchFadeOut(bool stateFade, float speedFade)
    {
        Debug.Log("Launch Fade out " + stateFade);
        renderCam.gameObject.SetActive(true);
        m_fadeObject.gameObject.SetActive(true);
        fadeOutActivation = stateFade;
        m_fadeInSecond = speedFade;
    }
}
