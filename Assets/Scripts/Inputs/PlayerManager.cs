using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerManager : MonoBehaviour
{
    public PlayerInputManager playerInputManager;

    public void Start()
    {
        Debug.Log(" Devices number is :  " + InputSystem.devices.Count);
        for (int i = 0; i < InputSystem.devices.Count; i++)
        {
            Debug.Log(" Devices name is :  "+ InputSystem.devices[i].name);
        }
    }

    public void AddPlayer()
    {
        Debug.Log("Add a player");
    }
}
    