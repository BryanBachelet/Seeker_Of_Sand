using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;


[Serializable]
public struct CameraStep
{
    public float duration;
    public Vector3 position;
    public Vector3 rotation;
}

public class CameraIntroMouvement : MonoBehaviour
{
    [SerializeField] private CameraStep[] m_cameraSteps = new CameraStep[0];
    private int m_prevIndex = 0;
    private int m_nextIndex = 1;

    private bool m_isActivate;
    private float m_timerCamera;
    private Render.Camera.CameraBehavior m_cameraScript;
    [SerializeField] private GameObject m_fixInterface;

    public void Start()
    {
        m_isActivate = true;
        m_cameraScript = GetComponent<Render.Camera.CameraBehavior>();

        GameState.ChangeState();
    }
    public void Update()
    {
        if (m_isActivate)
        {
            UpdateCameraStatus();
        }
    }

    public void InputSkipCinematic(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            SkipIntro();
        }
    }

    private void SkipIntro()
    {
        transform.rotation = Quaternion.Euler(m_cameraSteps[m_cameraSteps.Length - 1].rotation);
        transform.position = m_cameraSteps[m_cameraSteps.Length - 1].position;
        m_cameraScript.enabled = true;
        m_isActivate = false;
        m_fixInterface.SetActive(true);
        GameState.ChangeState();
    }

    private void UpdateCameraStatus()
    {
        if (m_timerCamera > m_cameraSteps[m_nextIndex].duration)
        {
            if (m_nextIndex != m_cameraSteps.Length - 1)
            {
                m_prevIndex++;
                m_nextIndex++;

            }
            else
            {
                m_cameraScript.enabled = true;
                m_isActivate = false;
                m_fixInterface.SetActive(true);
                GameState.ChangeState();
                return;
            }
            m_timerCamera = 0.0f;
        }

        m_timerCamera += Time.deltaTime;
        float ratio = m_timerCamera / m_cameraSteps[m_nextIndex].duration;

        transform.position = Vector3.Lerp(m_cameraSteps[m_prevIndex].position, m_cameraSteps[m_nextIndex].position, ratio);
        transform.rotation = Quaternion.Lerp(Quaternion.Euler(m_cameraSteps[m_prevIndex].rotation), Quaternion.Euler(m_cameraSteps[m_nextIndex].rotation), ratio);
    }
}
