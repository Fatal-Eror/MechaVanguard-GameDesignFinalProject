using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootGunReload : StateMachineBehaviour
{
    private Animator[] _animators;
    private WeaponBehaviour _shootGun;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (GameState.localPlayer != null)
        {
            _shootGun = GameState.localPlayer.GetComponent<WeaponManager>()._currentGun;
        }

        _animators = GameState.localPlayer.GetComponentsInChildren<Animator>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _shootGun.ReloadWeapon();

        foreach (var anime in _animators)
        {
            anime.SetBool("IsReloading", false);
            Debug.Log("Stop loading");
        }
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
