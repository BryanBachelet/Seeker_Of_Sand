using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCEnemiAnimation : MonoBehaviour
{
    public string attackNameTrigger = "Attacking";
    public string closeNameTrigger = "Close";
    public Animator m_animator;

    public void CallAnimAttack()
    {
        m_animator.SetTrigger("ActiveAttack");
        Debug.Log("Call anim attack");
    }

    public void ResetAnimAttack()
    {
        m_animator.ResetTrigger("ActiveAttack");
    }

    public void CallCloseAnimation()
    {
        m_animator.SetBool("Close", true);
    }

    public void ResetCloseAnimation()
    {
        m_animator.SetBool("Close", false);
    }

    
}
