using Render.Camera;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class TeleporterBehavior : MonoBehaviour
{
    public Teleporter lastTeleportor;
    public Vector3 lastTpPosition;
    public bool activationTP;
    public Teleporter nextTeleporter;
    public Vector3 nextTpPosition;
    public int nextTerrainNumber = 0;

    public CameraFadeFunction cameraFadeFunction;
    private CameraBehavior m_cameraBehavior;
    public TerrainGenerator terrainGen;
    public AltarBehaviorComponent altarBehavior;

    public VisualEffect apparitionVFX;
    public VisualEffect disparitionVFX;

    [HideInInspector] public bool isTimePassing;
    public EventHolder eventHolder;
    public DayCyclecontroller dayController;
    public DayTimeController dayTimeController;
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
            dayController.UpdateTimeByStep();
            dayTimeController.UpdateNextPhase();

        }
        terrainGen.ActiveGenerationTerrain(nextTerrainNumber);
        cameraFadeFunction.LaunchFadeOut(true, 1);
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
        nextTeleporter.transform.parent.GetComponentInChildren<RoomManager>().ActivateRoom();
        cameraFadeFunction.dayTextAnimator.ResetTrigger("NewDay");
        cameraFadeFunction.dayTextObj.SetActive(false);
    }
    public void ActivationDebugTeleportation()
    {
        this.gameObject.transform.position = nextTpPosition + new Vector3(0, 10, 0);
    }
}
