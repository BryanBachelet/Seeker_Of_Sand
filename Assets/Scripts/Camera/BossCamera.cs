using Render.Camera;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCamera : MonoBehaviour
{
    public Camera camera;
    private float timeCinematic;
    public float timeP1;
    public float timeMvt;
    public float timeP2;

    private Vector3 startPosition;
    public Transform positionCam1;

    private bool isActive = false;
    private float timer;
    private float halfTimer;
    private BossRoom BossRoom;
    private Transform targetCam;
    private bool activeMusique = false;

    public void StartCamera(Camera cameraPara, Transform target)
    {
        BossRoom = GetComponent<BossRoom>();
        camera = cameraPara;
        startPosition = cameraPara.transform.position;
        isActive = true;
        timer = 0;
        halfTimer = timeCinematic / 2.0f;
        camera.GetComponent<CameraBehavior>().enabled = false;
        targetCam = target;

        timeCinematic = timeP1 + timeP2;
    }

    public void Update()
    {
        if (!isActive) return;
        if (!activeMusique) BossRoom.roomManager.m_enemyManager.gsm.UpdateParameter(2.5f, "Intensity"); activeMusique = true;
        camera.transform.LookAt(targetCam);
        if (timer < timeP1)
        {
            float ratio = timer / timeMvt;
            camera.transform.position = Vector3.Lerp(startPosition, positionCam1.position, ratio);
        }
        else
        {
            float ratio = (timer - timeP1) / timeP2;
            camera.transform.position = Vector3.Lerp(positionCam1.position, startPosition, ratio);
            if (timer > timeCinematic)
            {
                BossRoom.LaunchBoss();
                isActive = false;
                camera.GetComponent<CameraBehavior>().enabled = true;
            }
        }

        timer += Time.deltaTime;


    }





}
