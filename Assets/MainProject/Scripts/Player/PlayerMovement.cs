using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerMovement : NetworkBehaviour
{
    private PlayerControls _input;
    private CharacterController _controller;
    private Animator[] _animators;
    private Camera _camera;    

    private Vector3 _presentSpeed = new Vector3(0,0,0);
    [SerializeField] private float moveSpeed = 20.0f;
    [SerializeField] private float virtualGravity = -9.81f;
    [SerializeField] private float maxFallSpeed = -0.1f;

    [SerializeField] private GameObject cameraPosition;
    private GameObject _photograher;

    [SerializeField] private int jumpTimes = 1;
    [SerializeField] private float jumpForce = 0.02f;
    private int _currentJumpTimes = 0;

    private bool hasMoved = false;  


    private void Start()
    {
        _input = new PlayerControls();
        _controller = GetComponent<CharacterController>();
        _animators = GetComponentsInChildren<Animator>();
        _camera = Camera.main;     

        if (GameNetwork.condition == LevelCondition.Battle && isLocalPlayer)
        {            
            _input.Enable();
            _photograher = FindAnyObjectByType<CameraController>().gameObject;
            _input.Gameplay.Jump.started += Jump;
        }
    }

    private void OnEnable()
    {
        if (GameNetwork.condition == LevelCondition.Battle && isLocalPlayer)
        {
            _input.Enable();
            _input.Gameplay.Jump.started += Jump;
        }
    }

    private void OnDisable()
    {
        if(_input!= null)
        {
            _input.Disable();
            _input.Gameplay.Jump.started -= Jump;
        }        
    }

    private void Update()
    {
        if (GameNetwork.condition == LevelCondition.Battle && isLocalPlayer)
        {
            UpdateMovement();
            _controller.Move(_presentSpeed);
            if(!hasMoved)
            {
                if (GameState.belongingTeam != PlayerTeam.None && GameState.belongingIndex != -1)
                {
                    GameState.currentLevelState.MovePlayerToSpawnPosition(this.gameObject);
                }
                else
                {
                    Debug.LogWarning($"Player does not have team and index in Player Movement, Update");
                }
                hasMoved = true;
            }
            UpdateRotation();

            if(_currentJumpTimes >= 1 && _controller.isGrounded)
            {
                foreach (var animator in _animators)
                {
                    animator.SetBool("IsGrounded", true);
                }
            }
           
        }
        UpdateGravityInfluenceOnY();

        if (_controller.isGrounded)
        {
            _currentJumpTimes = 0;
        }
    }

    private void UpdateMovement()
    {
        Vector2 input = _input.Gameplay.Move.ReadValue<Vector2>();
        Vector3 forward = input.y * moveSpeed * Time.deltaTime * _photograher.transform.forward;
        Vector3 right=  input.x * moveSpeed* Time.deltaTime * _photograher.transform.right;

        _presentSpeed.x = forward.x + right.x;  
        _presentSpeed.z = forward.z + right.z;

        foreach(var animator in _animators) 
        {
            animator.SetFloat("MoveForward", input.y);
            animator.SetFloat("MoveRight", input.x);
        }
    }

    private void UpdateGravityInfluenceOnY()
    {
        if (_controller.isGrounded)
        {
            _presentSpeed.y = 0;
        }

        _presentSpeed.y += Time.deltaTime * virtualGravity;
        _presentSpeed.y = Mathf.Max(maxFallSpeed, _presentSpeed.y);
    }

    private void UpdateRotation()
    {
        // Only change rotation when player press wasd
        if (!_input.Gameplay.Move.IsPressed() && !_input.Gameplay.Fire.IsPressed())
        {
            return;
        }

        transform.eulerAngles = new Vector3(0, _photograher.transform.eulerAngles.y, 0);        
    }

    private void Jump(InputAction.CallbackContext obj)
    {
        if(_currentJumpTimes < jumpTimes)
        {
            _presentSpeed.y += jumpForce;
            _currentJumpTimes++;

            foreach (var animator in _animators)
            {
                animator.SetBool("IsGrounded", false);
            }
        }
    }
}
