using BorsalinoTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DissonanceHeartBehavior;

public class DissonanceHeartBehavior : InteractionInterface
{
    public enum DissonanceHeartState
    {
        PROTECTED = 0,
        UNPROTECTED = 1,
        BROKEN = 2,
    }

    [Header("Dissonance Heart Variablle")]
    public DissonanceHeartState dissonanceHeartState = DissonanceHeartState.PROTECTED;

    [Header("Debug  Variables")]
    [SerializeField] private bool m_activeDissonanceHeartDebug;

    [HideInInspector] public RoomManager roomManager;

    public void Start()
    {
        isInteractable = false;
    }

    public void RemoveProtection()
    {
        if (m_activeDissonanceHeartDebug)
            ScreenDebuggerTool.AddMessage("Dissonance heart is unprotected");
        dissonanceHeartState = DissonanceHeartState.UNPROTECTED;
        isInteractable = true;
    }

    public override void OnInteractionStart(GameObject player)
    {
        dissonanceHeartState = DissonanceHeartState.BROKEN ;
        if (m_activeDissonanceHeartDebug)
            ScreenDebuggerTool.AddMessage("Dissonance heart is broken");
        roomManager.ValidateRoom();
        isInteractable = false;
    }

    public override void OnInteractionEnd(GameObject player)
    {
        return;
    }


}
