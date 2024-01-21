using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class Camera_Hub_Move : MonoBehaviour
{
    public GameObject playerRef;
    private Character.CharacterMouvement m_characterMove;
    public Transform playPositionTrain;
    public bool activeFollow;
    public bool inputDebut = false;
    public Render.Camera.CameraBehavior m_cameraScript;

    public GameObject trainRef;
    private Animator m_animatorTrain;

    public Animator bandeNoirAnimator;
    private void Start()
    {
        m_characterMove = playerRef.GetComponent<Character.CharacterMouvement>();
        m_cameraScript = GetComponent<Render.Camera.CameraBehavior>();
        m_animatorTrain = trainRef.GetComponent<Animator>();
    }
    private void Update()
    {
        if (activeFollow)
        {
            transform.LookAt(playerRef.transform);

            playerRef.transform.position = playPositionTrain.position;

        }
        if(inputDebut)
        {
            StartCoroutine(StartGameActivation());
  
        }
    }

    public IEnumerator StartGameActivation()
    {
        inputDebut = false;
        m_cameraScript.enabled = false;
        m_characterMove.enabled = false;
        activeFollow = true;
        m_animatorTrain.SetBool("GameStart", true);
        bandeNoirAnimator.SetBool("active", true);
        yield return new WaitForSeconds(1.5f);
        bandeNoirAnimator.SetBool("Close", true);
    }
}
