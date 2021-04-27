﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class AnimScript : StateMachineBehaviour {

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        
        animator.SetBool("Revive", false);
        animator.SetBool("Dead", false);
        animator.SetBool("Walk", false);
        animator.SetBool("Idle", true);
    }

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (Input.GetKeyDown(KeyCode.W))
        {
            animator.SetBool("Walk", true);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            animator.SetBool("Flex", true);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            animator.SetBool("Idle", true);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            animator.SetBool("Attack", true);
        }
        if(Input.GetKeyDown(KeyCode.D))
        {
            animator.SetBool("Dead", true);
        }
        
    }

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.SetBool("Revive", false);
    }

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
