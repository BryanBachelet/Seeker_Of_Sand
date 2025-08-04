using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomStarter : MonoBehaviour
{

    public RoomManager roomManager;
    public bool validateRotation;
    public void Start()
    {
        Destroy(this.gameObject);
    }


    public void OnValidate()
    {
        if (validateRotation)
        {
          // if(roomManager) roomManager.spawnAngle = transform.rotation.eulerAngles.y;
        }
    }
}
