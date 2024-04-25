using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootGunFire : StateMachineBehaviour
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

        if(_shootGun != null)
        {
            Debug.Log(GameState.localPlayer.playerName + "Entre fire");
            _shootGun.OpenFire();
        }


    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_shootGun != null)
        {
            _shootGun.StopFire();
        }

        // Ensure the var is changed to false
        foreach (var anime in _animators)
        {
            anime.SetBool("IsFire", false);
        }

        _shootGun.OnLeftClicked -= LeftClickedEvent;
        _shootGun.OnLeftReleased -= LeftReleasedEvent;
        _shootGun.OnReloadPressed -= ReloadPressedEvent;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(_shootGun.currentBullets <= 0)
        {
            foreach (var anime in _animators)
            {
                anime.SetBool("IsFire", false);
            }
        }
    }

    private void LeftClickedEvent()
    {
        Debug.Log("LeftClicked In fire anime ");
    }

    private void LeftReleasedEvent()
    {
        Debug.Log("LeftReleased in fire anime");
        foreach (var animator in _animators)
        {
            animator.SetBool("IsFire", false);
        }
    }

    private void ReloadPressedEvent()
    {
        if(_shootGun.currentBullets != _shootGun.maxBullets && _shootGun.spareBullets > 0)
        {
            foreach (var animator in _animators)
            {
                animator.SetBool("IsFire", false);
                animator.SetBool("IsReloading", true);
            }
        }        
    }
}
