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
    [SerializeField] private AnimationCurve[] m_cameraStepsSpeeds = new AnimationCurve[0];
    private int m_prevIndex = 0;
    private int m_nextIndex = 1;

    [SerializeField] private bool isSkipCinematics;
    private bool m_isActivate;
    private float m_timerCamera;
    private Render.Camera.CameraBehavior m_cameraScript;
    [SerializeField] private GameObject m_fixInterface;
    [SerializeField] private Animator m_BlackOpening;

    public GameObject[] objectToActiveAfterStart;
    public void Start()
    {


        if (!isSkipCinematics)
        {
            m_isActivate = true;
        }

        m_cameraScript = GetComponent<Render.Camera.CameraBehavior>();

        GameState.ChangeState();
        GlobalSoundManager.PlayOneShot(41, transform.position);

        if (isSkipCinematics)
        {
            m_cameraScript.enabled = true;
            m_isActivate = false;
          
            m_fixInterface.SetActive(true);
            GameState.ChangeState();
            this.enabled = false;
        }


    }
    public void Update()
    {
        if (m_isActivate )
        {
            UpdateCameraStatus();
        }
    }

    public void InputSkipCinematic(InputAction.CallbackContext ctx)
    {
        if (ctx.started && this.enabled)
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
        ActiveItemAfterStart();
        m_fixInterface.SetActive(true);
        GameState.ChangeState();
        this.enabled = false;

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
                ActiveItemAfterStart();
                m_fixInterface.SetActive(true);
                GameState.ChangeState();
                this.enabled = false;

                return;
            }
            GlobalSoundManager.PlayOneShot(41, transform.position);
            m_timerCamera = 0.0f;
        }

        m_timerCamera += Time.deltaTime;

        float ratio = m_timerCamera / m_cameraSteps[m_nextIndex].duration;
        ratio = m_cameraStepsSpeeds[m_nextIndex].Evaluate(ratio);
        transform.position = Vector3.Lerp(m_cameraSteps[m_prevIndex].position, m_cameraSteps[m_nextIndex].position, ratio);
        transform.rotation = Quaternion.Lerp(Quaternion.Euler(m_cameraSteps[m_prevIndex].rotation), Quaternion.Euler(m_cameraSteps[m_nextIndex].rotation), ratio);
    }

    public void ActiveItemAfterStart()
    {
        OpenBlackBorder();
        if (objectToActiveAfterStart.Length == 0) return;
        for (int i = 0; i < objectToActiveAfterStart.Length; i++)
        {
            objectToActiveAfterStart[i].SetActive(true);
        }
    }

    public void OpenBlackBorder()
    {
        m_BlackOpening = GameObject.Find("Black_limite_Holder").GetComponent<Animator>();
        m_BlackOpening.SetBool("Open", true);
    }
}
