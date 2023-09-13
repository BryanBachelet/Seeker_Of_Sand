using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;

public class ManageIntroGameMenu : MonoBehaviour
{
    public GameObject m_activeUI;
    public VideoPlayer m_videoPlayer;

    private bool startPlaying = false;
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
        if (m_activeUI.activeSelf) return;
        m_activeUI.SetActive(true);
    }

    public void SkipIntro(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            m_videoPlayer.Stop();
            ActiveMenu();
        }
    }
}
