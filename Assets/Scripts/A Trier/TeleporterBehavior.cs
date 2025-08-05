using Render.Camera;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class TeleporterBehavior : MonoBehaviour
{
    [HideInInspector] private Teleporter nextTeleporter;
    [HideInInspector] private Vector3 nextTpPosition;
    [HideInInspector] public int nextTerrainNumber = 0;


    [HideInInspector] private CameraFadeFunction cameraFadeFunction;
    [HideInInspector] private CameraBehavior m_cameraBehavior;
    public TerrainGenerator terrainGen;

    [SerializeField] private VisualEffect apparitionVFX;
    public VisualEffect disparitionVFX;

    [HideInInspector] public bool isTimePassing;
    [SerializeField] public int specialRoomID = -1;
    public EventHolder eventHolder;

    public DayTimeController dayTimeController;

    public static Transform socleReferencePosition = null;
    // Start is called before the first frame update
    void Start()
    {
        Camera cameraMain = Camera.main;
        if (cameraFadeFunction == null) { cameraFadeFunction = cameraMain.GetComponent<CameraFadeFunction>(); }
        if(m_cameraBehavior == null) { m_cameraBehavior = cameraMain.GetComponent<CameraBehavior>();}
    }

    private void OnEnable()
    {
        Camera cameraMain = Camera.main;
        if (cameraFadeFunction == null) { cameraFadeFunction = cameraMain.GetComponent<CameraFadeFunction>(); }
        if (m_cameraBehavior == null) { m_cameraBehavior = cameraMain.GetComponent<CameraBehavior>(); }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GetTeleportorData(Teleporter tpObject)
    {
        nextTeleporter = tpObject;
        nextTpPosition = tpObject.transform.position;
        socleReferencePosition = tpObject.transform;
        //dayController.UpdateTimeByStep();
        //  nextTerrainNumber = tpObject.TeleporterNumber;
        // eventHolder.GetNewAltar(tpObject.altarBehavior);
    }

    public void ActivationTeleportation()
    {

        this.gameObject.transform.position = nextTpPosition + new Vector3(0, 10, 0);
        apparitionVFX.Play();
        if (isTimePassing)
        {
            //dayController.UpdateTimeByStep();
            dayTimeController.UpdateNextPhase();

        }
        if(specialRoomID >= 0)
        {
            dayTimeController.ChangeNextRoomSpecialStatut(specialRoomID, false);
        }
        terrainGen.ActiveGenerationTerrain(nextTerrainNumber);
        cameraFadeFunction.LaunchFadeOut(true, 0.38f);
        //if (dayController.newDay)
        //{
        //    cameraFadeFunction.LaunchFadeOut(true, 0.25f);
        //    GlobalSoundManager.PlayOneShot(34, this.gameObject.transform.position);
        //    StartCoroutine(LaunchNewDay());
        //}
        //else 
        //{ 
        //    cameraFadeFunction.LaunchFadeOut(true, 1); 
        //    m_cameraBehavior.ChangeLerpForTP(); 
        //}


    }

    public IEnumerator LaunchNewDay()
    {
        cameraFadeFunction.dayTextObj.SetActive(true);
        cameraFadeFunction.dayTextAnimator.SetTrigger("NewDay");
        yield return new WaitForSeconds(3f);
        nextTeleporter.transform.parent.GetComponentInChildren<RoomManager>().ActivateRoom(null);
        cameraFadeFunction.dayTextAnimator.ResetTrigger("NewDay");
        cameraFadeFunction.dayTextObj.SetActive(false);
    }
    public void ActivationDebugTeleportation()
    {
        this.gameObject.transform.position = nextTpPosition + new Vector3(0, 10, 0);
    }
}
