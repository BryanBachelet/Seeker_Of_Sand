using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testBehaviorAnimation : StateMachineBehaviour
{
    public float timeEnterState = 0;
    public int attaqueNumber;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //animator.transform.parent.gameObject.GetComponent<attaqueSister>().GenerateSlashAttack();
        animator.transform.parent.gameObject.GetComponent<attaqueSister>().SetGlow(attaqueNumber);
        timeEnterState = Time.time;
    }
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float progress = (timeEnterState + 1 - Time.time);
        Debug.Log("Progress :" + progress);
        animator.transform.parent.gameObject.GetComponent<attaqueSister>().ModifyGlow(progress);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.parent.gameObject.GetComponent<attaqueSister>().ExitGlow(attaqueNumber);
        //animator.gameObject.SetActive(false);

    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
