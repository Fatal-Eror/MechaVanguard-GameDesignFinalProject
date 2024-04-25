using Org.BouncyCastle.Asn1;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShootGunIdle : StateMachineBehaviour
{
    private WeaponBehaviour _shootGun;
    private Animator[] _animators;
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (GameState.localPlayer != null)
        {
            _shootGun = GameState.localPlayer.GetComponent<WeaponManager>()._currentGun;
        }

        _animators = GameState.localPlayer.GetComponentsInChildren<Animator>();
        
        _shootGun.OnLeftClicked += LeftClickedEvent;
        _shootGun.OnLeftReleased += LeftReleasedEvent;
        _shootGun.OnReloadPressed += ReloadPressedEvent;

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(_shootGun!= null)
        {
            _shootGun.OnLeftClicked -= LeftClickedEvent;
            _shootGun.OnLeftReleased -= LeftReleasedEvent;
            _shootGun.OnReloadPressed -= ReloadPressedEvent;
        }        
    }

    private void LeftClickedEvent()
    {

        Debug.Log("LeftClicked ");
        if(_shootGun.currentBullets != 0)
        {
            foreach(var animator in _animators)
            {
                animator.SetBool("IsFire", true);
            }
        }
        else
        {
            Debug.Log("Empty");
        }
    }

    private void LeftReleasedEvent()
    {
        Debug.Log("LeftReleased in idle");
        foreach (var animator in _animators)
        {
            animator.SetBool("IsFire", false);
        }
    }

    private void ReloadPressedEvent()
    {
        if (_shootGun.currentBullets != _shootGun.maxBullets && _shootGun.spareBullets > 0)
        {
            foreach (var animator in _animators)
            {
                animator.SetBool("IsReloading", true);
            }            
        }        
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //
    // }

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
