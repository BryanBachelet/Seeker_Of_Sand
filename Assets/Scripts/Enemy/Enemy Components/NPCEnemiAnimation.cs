using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCEnemiAnimation : MonoBehaviour
{
    public string prepAttackNameTrigger = "Attacking";
    public string moveTriggerName;
    public List<int> indexAttackTrigger = new List<int>();
    public string closeNameTrigger = "Close";
    public Animator m_animator;


    public void CallAnimMovement()
    {
        m_animator.SetTrigger(moveTriggerName);
    }

    public void ResetAnimMouvement()
    {
        m_animator.ResetTrigger(moveTriggerName);
    }

    public void CallAnimPrepAttack(int index)
    {
        m_animator.SetTrigger(prepAttackNameTrigger + index.ToString());
        
    }
    public void CallAnimPrepAttackByIndex(int index)
    {
        m_animator.SetTrigger(prepAttackNameTrigger + index);
       
    }

    public void ResetAnimAttack(int index)
    {
        m_animator.ResetTrigger(prepAttackNameTrigger + index.ToString());
    }

    public void CallCloseAnimation(int index)
    {
        m_animator.SetBool(closeNameTrigger, true);
    }

    public void ResetCloseAnimation(int index)
    {
        m_animator.SetBool(closeNameTrigger, false);
    }

    
}
