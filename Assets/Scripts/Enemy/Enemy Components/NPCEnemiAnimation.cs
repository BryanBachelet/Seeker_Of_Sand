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
        if (!HasParameter(moveTriggerName, m_animator)) return;
        m_animator.SetTrigger(moveTriggerName);
    }

    public void ResetAnimMouvement()
    {
        if (!HasParameter(moveTriggerName, m_animator)) return;
        m_animator.ResetTrigger(moveTriggerName);
    }

    public void CallAnimPrepAttack(int index)
    {
        if (!HasParameter(prepAttackNameTrigger, m_animator)) return;
        m_animator.SetTrigger(prepAttackNameTrigger + index.ToString());
        
    }
    public void CallAnimPrepAttackByIndex(int index)
    {
        if (!HasParameter(prepAttackNameTrigger, m_animator)) return;
        m_animator.SetTrigger(prepAttackNameTrigger + index);
       
    }

    public void ResetAnimAttack(int index)
    {
        if (!HasParameter(prepAttackNameTrigger, m_animator)) return;
        m_animator.ResetTrigger(prepAttackNameTrigger + index.ToString());
    }

    public void CallCloseAnimation(int index)
    {
        if (!HasParameter(closeNameTrigger, m_animator)) return;

        m_animator.SetBool(closeNameTrigger, true);
    }

    public void ResetCloseAnimation(int index)
    {
        if (!HasParameter(closeNameTrigger, m_animator)) return;
        m_animator.SetBool(closeNameTrigger, false);
    }

    public static bool HasParameter(string paramName, Animator animator)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName) return true;
        }
        return false;
    }


}
