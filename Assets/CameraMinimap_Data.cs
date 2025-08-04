using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMinimap_Data : MonoBehaviour
{
    [Header("Camera render terrain Setting")]
    public Vector3 Crt_position;
    public Quaternion Crt_rotation = new Quaternion(90,0,0,0);

    [Header("Camera Icon Setting")]
    public Vector3 Ci_position;
    public Quaternion Ci_rotation = new Quaternion(90, 0, 0, 0);

    private Vector3[] positionCamera = new Vector3[2];
    private Quaternion[] rotationCamera = new Quaternion[2];
    // Start is called before the first frame update


    public Vector3[] SettingCameraMinimap_Position()
    {
        positionCamera[0] = Crt_position;
        positionCamera[1] = Ci_position;
        return positionCamera;
    }

    public Quaternion[] SettingCameraMinimap_Rotation()
    {
        rotationCamera[0] = Crt_rotation;
        rotationCamera[1] = Ci_rotation;
        return rotationCamera;
    }
}
