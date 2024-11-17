using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFadeFunction : MonoBehaviour
{
    public MeshRenderer fadeObject;
    private Material fadeMat;

    public float fadeProgress = 0;
    public float fadeSpeed = 1; //1 = 1 seconde ; 2 = 2 secondes
    public bool fadeInActive;
    public bool fadeInActivation;

    public bool fadeOutActive;
    public bool fadeOutActivation;
    public bool manuelFade = false;

    public TeleporterBehavior tpBehavior;
    public Animator bandeNoir;
    // Start is called before the first frame update
    void Start()
    {
        fadeMat = fadeObject.material;
        LaunchFadeOut(true, 2);
    }

    // Update is called once per frame
    void Update()
    {
        if(manuelFade)
        {
            ChangeFadeAlpha(fadeProgress);
            return;
        }
        if(fadeInActivation)
        {
            fadeInActive = true;
            fadeInActivation = false;
            fadeProgress = 0;
        }
        if(fadeInActive)
        {
            if(fadeProgress < 0.99f)
            {
                fadeProgress += Time.deltaTime * (1 / fadeSpeed);
            }
            else
            {
                fadeProgress = 1;
                fadeInActive = false;
                tpBehavior.ActivationTeleportation();
                LaunchFadeOut(true, 1);


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
                fadeProgress -= Time.deltaTime * fadeSpeed;
            }
            else
            {
                fadeProgress = 0;
                fadeOutActive = false;
            }
            ChangeFadeAlpha(fadeProgress);
        }
    }

    private void ChangeFadeAlpha(float alphaValue)
    {
        fadeMat.SetColor("_UnlitColor", new Color(0,0,0,alphaValue));
    }

    public void LaunchFadeIn(bool stateFade, float speedFade)
    {
        fadeInActivation = stateFade;
        fadeSpeed = speedFade;
    }

    public void LaunchFadeOut(bool stateFade, float speedFade)
    {
        fadeOutActivation = stateFade;
        fadeSpeed = speedFade;
    }
}
