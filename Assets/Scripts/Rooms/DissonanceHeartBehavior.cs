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
    [SerializeField] private Animator m_animator;
    [SerializeField] private Transform cristalPartHolder;
    private Rigidbody[] cristalPartRB;
    public void Start()
    {
        isInteractable = false;
        int childNumber = cristalPartHolder.childCount;
        cristalPartRB = new Rigidbody[childNumber];
        for(int i = 0; i < childNumber; i++)
        {
            cristalPartRB[i] = cristalPartHolder.GetChild(i).GetComponent<Rigidbody>();
        }
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
        m_animator.enabled = false;
        GlobalSoundManager.PlayOneShot(60, transform.position);
        roomManager.ValidateRoom();
        isInteractable = false;
    }

    public void DestroyHeart()
    {
        for (int i = 0; i < cristalPartRB.Length; i++)
        {
            cristalPartRB[i].AddExplosionForce(25, cristalPartHolder.position, 50);
        }
    }
    public override void OnInteractionEnd(GameObject player)
    {
        return;
    }


}
