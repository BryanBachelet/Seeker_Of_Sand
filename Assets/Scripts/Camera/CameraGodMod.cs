
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraGodMod : MonoBehaviour
{
    public float speedCameraMvt = 10;
    public float rotationMvt = 10;
    public float deadZoneMouse = 10;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            Vector2 axis = GetMouvementAxis();

            transform.position += ((transform.forward * axis.y) + (transform.right * axis.x)) * speedCameraMvt * Time.deltaTime;
        }
        Vector2 mousedelta = Mouse.current.delta.value;
        if (Input.GetMouseButton(1))
        {
            if (Mathf.Abs(mousedelta.y) > deadZoneMouse) transform.rotation *= Quaternion.AngleAxis(mousedelta.y * rotationMvt * Time.deltaTime, Vector3.right);
            if (Mathf.Abs(mousedelta.x) > deadZoneMouse) transform.rotation *= Quaternion.AngleAxis(mousedelta.x * rotationMvt * Time.deltaTime, Vector3.up);
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
        }

    }

    public Vector2 GetMouvementAxis()
    {
        Vector2 axis = Vector2.zero;
        if (Input.GetKey(KeyCode.Z))
        {
            axis.y++;
        }

        if (Input.GetKey(KeyCode.S))
        {
            axis.y--;
        }

        if (Input.GetKey(KeyCode.D))
        {
            axis.x++;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            axis.x--;
        }


        return axis;
    }
}
