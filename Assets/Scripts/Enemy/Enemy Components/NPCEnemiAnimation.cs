using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCEnemiAnimation : MonoBehaviour
{
    public string prepAttackNameTrigger = "Attacking";
    public string closeNameTrigger = "Close";
    public Animator m_animator;

    public void CallAnimPrepAttack()
    {
        m_animator.SetTrigger(prepAttackNameTrigger);
        Debug.Log("Call anim attack");
    }

    public void ResetAnimAttack()
    {
        m_animator.ResetTrigger(prepAttackNameTrigger);
    }

    public void CallCloseAnimation()
    {
        m_animator.SetBool(closeNameTrigger, true);
    }

    public void ResetCloseAnimation()
    {
        m_animator.SetBool(closeNameTrigger, false);
    }

    
}
