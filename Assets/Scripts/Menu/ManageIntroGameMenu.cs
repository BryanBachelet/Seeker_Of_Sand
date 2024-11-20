using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;
using UnityEngine.Rendering;
using TMPro;

public class ManageIntroGameMenu : MonoBehaviour
{
    public GameObject m_activeUI;
    public VideoPlayer m_videoPlayer;

    public Animator dayControllerAnimator;
    private bool startPlaying = false;

    public GameObject playMenu;
    public LensFlareDataSRP lensFlaresun;

    [SerializeField] private TMP_Text m_textName;

    public void Start()
    {
        // Set Resolution
        Screen.SetResolution(1920, 1080, true);


        if (lensFlaresun)
        {
            lensFlaresun.elements[0].localIntensity = 0;
            lensFlaresun.elements[1].localIntensity = 0;
            lensFlaresun.elements[2].localIntensity = 0;
            lensFlaresun.elements[3].localIntensity = 0;

        }
        GlobalSoundManager.PlayOneShot(0, transform.position);
        StartCoroutine(ActiveMenuDelay(10f));
    }
    public void Update()
    {
        if (m_videoPlayer.isPlaying)
        {
            startPlaying = true;
        }

        if (!m_videoPlayer.isPlaying && startPlaying && !m_activeUI.activeSelf)
        {
            ActiveMenu();

            startPlaying = false;
        }
    }

    public IEnumerator ActiveMenuDelay(float delay)
    {

        yield return new WaitForSeconds(delay - 0.4f);
        if (lensFlaresun)
        {
            lensFlaresun.elements[3].localIntensity = 10;

        }
        yield return new WaitForSeconds(0.1f);
        if (lensFlaresun)
        {
            lensFlaresun.elements[3].localIntensity = 20;

        }
        yield return new WaitForSeconds(0.1f);
        if (lensFlaresun)
        {
            lensFlaresun.elements[3].localIntensity = 30;

        }
        yield return new WaitForSeconds(0.1f);
        if (lensFlaresun)
        {
            lensFlaresun.elements[3].localIntensity = 40;

        }
        yield return new WaitForSeconds(0.1f);
        if (lensFlaresun)
        {
            lensFlaresun.elements[0].localIntensity = 0;
            lensFlaresun.elements[1].localIntensity = 0;
            lensFlaresun.elements[2].localIntensity = 0;
            lensFlaresun.elements[3].localIntensity = 50;

        }

        yield return new WaitForSeconds(1);
        if (lensFlaresun)
        {
            lensFlaresun.elements[0].localIntensity = 1;
            lensFlaresun.elements[1].localIntensity = 0.03f;
            lensFlaresun.elements[2].localIntensity = 1;
            lensFlaresun.elements[3].localIntensity = 50;

        }
        dayControllerAnimator.SetTrigger("ActivationMenu");
        m_activeUI.SetActive(true);
        m_videoPlayer.gameObject.SetActive(false);
    }
    public void ActiveMenu()
    {




        //
    }

    public void SkipIntro(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            m_videoPlayer.Stop();
            //StartCoroutine(ActiveMenuDelay());
            ActiveMenu();
        }
    }

    public void SetInactive()
    {
        dayControllerAnimator.gameObject.SetActive(true);
        //dayControllerAnimator.SetTrigger("ActivationMenu");
        this.gameObject.SetActive(false);
        playMenu.SetActive(true);
        m_textName.text = GameManager.instance.profileName;
    }
}
