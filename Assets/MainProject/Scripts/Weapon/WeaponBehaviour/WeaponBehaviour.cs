using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using UnityEngine.UIElements;

public abstract class WeaponBehaviour : NetworkBehaviour
{
    public WeaponManager weaponManager;

    // Observation mode, it will be binded by animator's state behaviour
    public Action OnLeftClicked;
    public Action OnLeftReleased;
    public Action OnReloadPressed;

    // Attributes
    public int currentBullets;
    public int maxBullets;
    public int spareBullets;
    public float damage;
    public float bulletPerSec; // How many bullets are shot in 1 sec
    public float shootingRange;


    // UI Relevant
    private UIDocument _shootGunUI;
    private VisualElement _root;
    private VisualElement _topCrosshair;
    private VisualElement _bottomCrosshair;
    private VisualElement _leftCrosshair;
    private VisualElement _rightCrosshair;
    private Label _bulletsInfo;

    public PlayerControls _input;

    protected virtual void Start()
    {
        if (GameState.localPlayer != null)
        {
            weaponManager = GameState.localPlayer.gameObject.GetComponent<WeaponManager>();
        }

        _input = new PlayerControls();

        _shootGunUI = GetComponent<UIDocument>();

        _root = GetComponent<UIDocument>().rootVisualElement;
        _topCrosshair = _root.Q<VisualElement>("TopCrosshair");
        _bottomCrosshair = _root.Q<VisualElement>("BottomCrosshair");
        _rightCrosshair = _root.Q<VisualElement>("RightCrosshair");
        _leftCrosshair = _root.Q<VisualElement>("LeftCrosshair");
        _bulletsInfo = _root.Q<Label>("BulletsInfo");
    }

    protected virtual void OnEnable()
    {
        if (GameState.localPlayer != null)
        {
            weaponManager = GameState.localPlayer.gameObject.GetComponent<WeaponManager>();
            weaponManager._currentGun = this;
        }
    }

    protected virtual void OnDisable()
    {
        /*if(weaponManager != null )
        {
            weaponManager._currentGun = null;
        } */

        if(_shootGunUI!= null)
        {
            _shootGunUI.enabled = false;
        }
    }

    protected virtual void Update()
    {
        if (gameObject.active == true && isLocalPlayer)
        {
            
            if (weaponManager != null)
            {
                weaponManager._currentGun = this;
            }
            _input.Enable();            
        }
        else
        {
            _input.Disable();
        }

        if (GameNetwork.condition != LevelCondition.Battle || !isLocalPlayer)
        {
            _shootGunUI.enabled = false;

        }
        else
        {
            _shootGunUI.enabled = true;
            _bulletsInfo.text = $"{currentBullets} / {maxBullets} / {spareBullets}";
        }        
    }

    protected abstract void CallLeftClickedEvent(InputAction.CallbackContext obj);
    protected abstract void CallLeftReleasedEvent(InputAction.CallbackContext obj);
    protected abstract void CallReloadPressedEvent(InputAction.CallbackContext obj);

    public abstract void ReloadWeapon();
    public abstract void OpenFire();
    public abstract void StopFire();

}
