using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;

public class ManageIntroGameMenu : MonoBehaviour
{
    public GameObject m_activeUI;
    public VideoPlayer m_videoPlayer;

    public Animator dayControllerAnimator;
    private bool startPlaying = false;

    public GameObject playMenu;
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

    public void ActiveMenu()
    {
        m_activeUI.SetActive(true);
        m_videoPlayer.gameObject.SetActive(false);
        if (m_activeUI.activeSelf) return;

        //
    }

    public void SkipIntro(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            m_videoPlayer.Stop();
            ActiveMenu();
        }
    }

    public void SetInactive()
    {
        dayControllerAnimator.gameObject.SetActive(true);
        dayControllerAnimator.SetTrigger("ActivationMenu");
        this.gameObject.SetActive(false);
        playMenu.SetActive(true);
    }
}
