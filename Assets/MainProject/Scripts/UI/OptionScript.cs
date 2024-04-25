using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Mirror;
using UnityEngine.InputSystem;
using System;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(UIDocument))]
public class OptionScript : NetworkBehaviour
{
    private PlayerControls _input;
    private VisualElement _optionsRoot;
    private Slider _pitchSlider;
    private Slider _yawSlider;
    public static GameObject instance;

    private bool isOptionOpen = false;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);            
        }
        instance = gameObject;
        GameObject.DontDestroyOnLoad(this);

        _input = new PlayerControls();
        _input.Enable();

        _optionsRoot = GetComponent<UIDocument>().rootVisualElement;
        _pitchSlider = _optionsRoot.Q<Slider>("YSlider");
        _yawSlider = _optionsRoot.Q<Slider>("XSlider");

        SytleInitialization();

        _input.UI.CallOptions.started += OptionsMenuToggle;

        _optionsRoot.Q<Button>("Leave").clicked += LeaveRoom;
        _optionsRoot.Q<Button>("Exit").clicked += Exit;
        _optionsRoot.Q<Button>("Back").clicked += BackToLobby;
        _pitchSlider.RegisterValueChangedCallback<float>(UpdatePitchSensitivity);
        _yawSlider.RegisterValueChangedCallback<float>(UpdateYawSensitivity);
    }    

    private void OnDestroy()
    {
        _input.UI.CallOptions.started -= OptionsMenuToggle;
        _input.Disable();

        _optionsRoot.Q<Button>("Leave").clicked -= LeaveRoom;
        _optionsRoot.Q<Button>("Exit").clicked -= Exit;
        _optionsRoot.Q<Button>("Back").clicked -= BackToLobby;
    }
    private void Update()
    {
        /*print(GameNetwork.condition);*/
    }

    private void SytleInitialization()
    {
        _optionsRoot.visible = isOptionOpen;
        _optionsRoot.style.display = isOptionOpen ? DisplayStyle.Flex : DisplayStyle.None;
        _optionsRoot.SetEnabled(isOptionOpen);

        if (PlayerPrefs.HasKey(nameof(GameState.PitchSensitivity)))
        {
            _pitchSlider.value = PlayerPrefs.GetFloat(nameof(GameState.PitchSensitivity));
        }
        else
        {
            _pitchSlider.value = GameState.defaultPitchSensitivity;
        }

        if (PlayerPrefs.HasKey(nameof(GameState.YawSensitivity)))
        {
            _yawSlider.value = PlayerPrefs.GetFloat(nameof(GameState.YawSensitivity));
        }
        else
        {
            _yawSlider.value = GameState.defaultYawSensitivity;
        }     
    }

    private void OptionsMenuToggle(InputAction.CallbackContext obj)
    {
        if (GameNetwork.condition == LevelCondition.Login)
        {
            return;
        }

        List<UIDocument> otherUI = FindObjectsOfType<UIDocument>().ToList();
        otherUI.Remove(GetComponent<UIDocument>());

        isOptionOpen = !isOptionOpen;

        if (otherUI.Count > 0)
        {
            foreach (var ui in otherUI)
            {
                if (ui != null)
                {
                    VisualElement tempRoot = ui.rootVisualElement;
                    if(tempRoot != null)
                    {
                        tempRoot.visible = !isOptionOpen;
                        tempRoot.style.display = isOptionOpen ? DisplayStyle.None : DisplayStyle.Flex;
                        tempRoot.SetEnabled(!isOptionOpen);
                    }                    
                }
            }
        }

        _optionsRoot.visible = isOptionOpen;
        _optionsRoot.style.display = isOptionOpen ? DisplayStyle.Flex : DisplayStyle.None;
        _optionsRoot.SetEnabled(isOptionOpen);

        if (isClient && GameNetwork.condition != LevelCondition.Login && GameNetwork.condition != LevelCondition.None)
        {
            // There is some bugs with leave UI, so I have to ban it temporarily
            /* _optionsRoot.Q<Button>("Leave").visible = isOptionOpen;
             _optionsRoot.Q<Button>("Leave").style.display = isOptionOpen ? DisplayStyle.Flex : DisplayStyle.None;
             _optionsRoot.Q<Button>("Leave").SetEnabled(isOptionOpen);*/
            _optionsRoot.Q<Button>("Leave").visible = false;
            _optionsRoot.Q<Button>("Leave").style.display = DisplayStyle.None;
            _optionsRoot.Q<Button>("Leave").SetEnabled(false);
        }
        else
        {
            _optionsRoot.Q<Button>("Leave").visible = false;
            _optionsRoot.Q<Button>("Leave").style.display = DisplayStyle.None;
            _optionsRoot.Q<Button>("Leave").SetEnabled(false);
        }

        if (isServer && GameNetwork.condition == LevelCondition.Battle)
        {
            _optionsRoot.Q<Button>("Back").visible = isOptionOpen;
            _optionsRoot.Q<Button>("Back").style.display = isOptionOpen ? DisplayStyle.Flex : DisplayStyle.None;
            _optionsRoot.Q<Button>("Back").SetEnabled(isOptionOpen);
        }
        else
        {
            _optionsRoot.Q<Button>("Back").visible = false;
            _optionsRoot.Q<Button>("Back").style.display = DisplayStyle.None;
            _optionsRoot.Q<Button>("Back").SetEnabled(false);
        }

        HandleCursor();
       
    }

    private void HandleCursor()
    {
        bool visible = GameNetwork.condition != LevelCondition.Battle || isOptionOpen;
        UnityEngine.Cursor.visible = visible;
        UnityEngine.Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void LeaveRoom()
    {
        switch (GameNetwork.singleton.mode)
        {
            // In a standalone server, shutdown.
            case NetworkManagerMode.ServerOnly:
                GameNetwork.singleton.StopServer();
                Application.Quit();
                break;
            // For a client, stop the client.
            case NetworkManagerMode.ClientOnly:
                GameNetwork.singleton.StopClient();
                break;
            // For a host, stop the host.
            case NetworkManagerMode.Host:
                GameNetwork.singleton.StopHost();
                break;
            // Otherwise, nothing to be done.
            case NetworkManagerMode.Offline:
            default:
                break;
        }
    }

    [Server]
    private void BackToLobby()
    {
        NetworkManager.singleton.ServerChangeScene("Lobby");
    }

    private void Exit()
    {
        GameState.QuitGame();
    }

    private void UpdatePitchSensitivity(ChangeEvent<float> evt)
    {
        GameState.PitchSensitivity = evt.newValue;
        PlayerPrefs.SetFloat(nameof(GameState.PitchSensitivity), GameState.PitchSensitivity);
    }

    private void UpdateYawSensitivity(ChangeEvent<float> evt)
    {
        GameState.YawSensitivity = evt.newValue;
        PlayerPrefs.SetFloat(nameof(GameState.YawSensitivity), GameState.YawSensitivity);
    }
}
