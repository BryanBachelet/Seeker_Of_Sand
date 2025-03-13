using Render.Camera;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.Rendering.CoreEditorDrawer<TData>;

public class BossCamera : MonoBehaviour
{
    #region old variable
    private Camera camera;
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
    private bool activeDisplay = false;
    private Animator animator;

    private Animator bandeNoir;
    private Quaternion initialRotation;
    public float xRot;

    #endregion


    #region new variable
    private Camera m_cameraPlayer;

    [SerializeField] private bool cinematiqueActive = false;
    [SerializeField] private float totalTime = 5;           //useless to change, equal to all other time value
    [SerializeField] private float timePhaseStart = 1;
    [SerializeField] private float timePhaseEnd = 1;
    private float currentTime = 0;                          //Timer

    private Vector3 playerPosition;                         //Player ref
    [SerializeField] private Transform positionCamera;      //Player ref

    private BossRoom m_bossRoom;                            //Boss ref
    private Transform m_IntroductionTransformReference;
    private Vector3 endMovementCamera;
    private Vector3 posTargetInitial;
    #endregion
    public void StartCamera(Camera cameraPara, Transform target, Vector3 posSpawn)
    {
        posTargetInitial = Vector3.zero;
        BossRoom = GetComponent<BossRoom>();
        camera = cameraPara;
        startPosition = cameraPara.transform.position;
        initialRotation = cameraPara.transform.rotation;
        isActive = true;
        timer = 0;
        halfTimer = timeCinematic / 2.0f;
        camera.GetComponent<CameraBehavior>().enabled = false;
        animator = camera.GetComponent<Animator>();
        bandeNoir = camera.GetComponent<CameraFadeFunction>().bandeNoir;
        endMovementCamera = posSpawn;
        targetCam = target;
        animator.enabled = true;
        animator.SetBool("LaunchBoss", true);
        bandeNoir.SetBool("BossBande", true);
        timeCinematic = timeP1 + timeP2;
        BossRoom.enemyManager.uiDispatcher.fixeGameplayUI.SetActive(false);

    }

    public void Update()
    {
        if (!isActive) return;

        //camera.transform.LookAt(BossRoom.enemyManager.AstrePositionReference);
        if (timer < timeP1)
        {
            float ratio = timer / timeMvt;
            //camera.transform.position = Vector3.Lerp(startPosition, positionCam1.position, ratio);
            camera.transform.LookAt(BossRoom.enemyManager.AstrePositionReference);
            if (timer / timeMvt >= 1)
            {
                //camera.transform.position = Vector3.Lerp(startPosition, positionCam1.position, 1);
                //camera.transform.eulerAngles = new Vector3(-90, 0, 0);
                animator.SetBool("LaunchBoss", false);
                bandeNoir.SetBool("BossBande", false);
            }
            targetCam.position = BossRoom.enemyManager.AstrePositionReference.position;
            if(posTargetInitial == Vector3.zero) { posTargetInitial = targetCam.position; }
        }
        else
        {
            float ratio = (timer - timeP1) / timeP2;
            //float ratioMvm = timer -  / timeP2);
            targetCam.position = Vector3.Lerp(posTargetInitial, camera.transform.position, ratio);

            //camera.transform.position = Vector3.Lerp(positionCam1.position, startPosition, ratio);
            if (timer + 2.2f > timeCinematic)
            {
                animator.enabled = false;
                if (!activeMusique) BossRoom.roomManager.m_enemyManager.gsm.UpdateParameter(2.5f, "Intensity"); activeMusique = true;

                camera.transform.LookAt(targetCam);

                camera.transform.position = startPosition;
            }
            if (timer + 1.8f > timeCinematic)
            {
                if (!activeDisplay) { BossRoom.enemyManager.uiDispatcher.fixeGameplayUI.SetActive(true); activeDisplay = true; BossRoom.DisplayBossHealth(); camera.transform.rotation = initialRotation; }
                else { }
            }
            if (timer > timeCinematic)
            {
                camera.transform.rotation = initialRotation;
                targetCam.position = endMovementCamera;
                camera.fieldOfView = 65;
                BossRoom.LaunchBoss();
                isActive = false;
                camera.GetComponent<CameraBehavior>().enabled = true;
            }
        }

        timer += Time.deltaTime;


    }





}
