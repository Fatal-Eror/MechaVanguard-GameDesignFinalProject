using Mirror;
using Org.BouncyCastle.Security;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ShootGunBehaviour : WeaponBehaviour
{
    [SerializeField] private int defaultCurrentBullets = 28;
    [SerializeField] private int defaultmaxBullets = 28;
    [SerializeField] private int defaultSpareBullets = 600;
    [SerializeField] private float defaultDamage = 15;
    [SerializeField] private float defaultBPS = 14;
    [SerializeField] private float defaultShootingRange = 100;

    // ParticalSystem Sets
    [SerializeField] GameObject shootingVFXManager;
    [SerializeField] GameObject flamePosition;

    // GunShooting Audio Clip
    public AudioClip shootAudio;

    private Coroutine _fireCoroutine;
    [SyncVar] private bool isAbleToFire = false;

    private Camera _mainCamera;

    protected override void Start()
    {
        base.Start();

        currentBullets = defaultCurrentBullets;
        maxBullets = defaultmaxBullets;
        spareBullets = defaultSpareBullets;
        damage = defaultDamage;
        bulletPerSec = defaultBPS;
        shootingRange = defaultShootingRange;

        _input.Gameplay.Fire.started += CallLeftClickedEvent;
        _input.Gameplay.Fire.canceled += CallLeftReleasedEvent;
        _input.Gameplay.Reload.started += CallReloadPressedEvent;

        _mainCamera = Camera.main;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (_input != null)
        {
            _input.Gameplay.Fire.started += CallLeftClickedEvent;
            _input.Gameplay.Fire.canceled += CallLeftReleasedEvent;
            _input.Gameplay.Reload.started += CallReloadPressedEvent;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (_input != null)
        {
            _input.Gameplay.Fire.started -= CallLeftClickedEvent;
            _input.Gameplay.Fire.canceled -= CallLeftReleasedEvent;
            _input.Gameplay.Reload.started -= CallReloadPressedEvent;
        }
    }

    protected override void Update()
    {
        base.Update();
        // Debug.Log($"{GameState.localPlayer.playerName} bullets {currentBullets}");
    }
    protected override void CallLeftClickedEvent(InputAction.CallbackContext obj)
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (GameNetwork.condition == LevelCondition.Battle)
        {
            OnLeftClicked?.Invoke();
            print("left click");
        }
    }

    protected override void CallLeftReleasedEvent(InputAction.CallbackContext obj)
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (GameNetwork.condition == LevelCondition.Battle)
        {
            OnLeftReleased?.Invoke();
            print("left release");
        }
    }

    protected override void CallReloadPressedEvent(InputAction.CallbackContext obj)
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (GameNetwork.condition == LevelCondition.Battle)
        {
            OnReloadPressed?.Invoke();
            print("Reload press in shoot gun behaviour");
        }
    }

    public override void OpenFire()
    {
        // 不知道为什么，本地玩家一按左键所有角色都会进入没有动画播放的射击动画机状态
        // 而射击状态的脚本设置为，进入状态调用open fire，所有角色都会开始射击
        // 只好设置一个变量，确保只有按下左键的玩家才能启动射击协程
        UpdateFireState();
        UpdateFireStateOnServer();

        if (!isAbleToFire) { return; }

        _fireCoroutine = StartCoroutine(FireCoroutine());
    }

    public override void StopFire()
    {
        if(_fireCoroutine!= null)
        {
            StopCoroutine(_fireCoroutine);
        }        
        _fireCoroutine = null;
    }

    private IEnumerator FireCoroutine()
    {
        while (true)
        {
            // Debug.Log("Fire");

            PlayShootingVFX();
            PlayShootingSFX();
            CallServerToPlayShootingFX();
            RayDetect(_mainCamera.transform.position, _mainCamera.transform.forward, shootingRange, GameState.localPlayer);

            currentBullets--;

            if (currentBullets <= 0)
            {
                yield break;
            }

            yield return new WaitForSeconds(1.0f / bulletPerSec);
        }
    }

    private void PlayShootingVFX()
    {
        if (flamePosition != null)
        {
            GameObject visual = Instantiate(shootingVFXManager, flamePosition.transform.position, Quaternion.Euler(flamePosition.transform.eulerAngles - new Vector3(90, 45, 0)));
            Destroy(visual, 2);
        }
    }

    [Command]
    private void CallServerToPlayShootingFX()
    {
        PlayShootingFXOnAllClients();
    }

    [ClientRpc]
    private void PlayShootingFXOnAllClients()
    {
        PlayShootingVFX();
        PlayShootingSFX();
    }

    [Command]
    private void UpdateFireStateOnServer()
    {
        UpdateFireState();
    }

    private void UpdateFireState()
    {
        isAbleToFire = _input.Gameplay.Fire.IsPressed();
    }

    private void PlayShootingSFX()
    {
        if (shootAudio != null)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = shootAudio;
            source.Play();

            Destroy(source, 2);
        }
    }

    public override void ReloadWeapon()
    {
        int requiredBullets = (maxBullets - currentBullets) <= spareBullets ? (maxBullets - currentBullets) : spareBullets;
        currentBullets += requiredBullets;
        spareBullets -= requiredBullets;
    }

    [Command]
    private void RayDetect(Vector3 start, Vector3 direction, float length, PlayerAttributes instigator)
    {
        Physics.Raycast(start, direction, out RaycastHit hit, length, LayerMask.GetMask("Player"));
        if(hit.collider != null)
        {
            hit.transform.GetComponent<PlayerAttributes>().GetDamage(instigator, damage);
        }
        else
        {
            print("Didnt hit");
        }
    }

}
