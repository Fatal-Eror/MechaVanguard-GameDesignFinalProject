using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class MenuScript : MonoBehaviour
{
    private VisualElement _root;
    private Button _hostButton;
    private Button _joinButton;
    private Button _quitButton;
    private TextField _name;
    private TextField _ip;

    private void Awake()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _hostButton = _root.Q<Button>("Host");
        _joinButton = _root.Q<Button>("Join");
        _quitButton= _root.Q<Button>("Quit");
        _name = _root.Q<TextField>("NameValue");
        _ip = _root.Q<TextField>("IPValue");

        // Set default or stored name and address
        _name.value = PlayerPrefs.GetString(nameof(GameState.localPlayerName));
        _ip.value = GameState.DefaultAddress;
            GameState.SetPlayerName(_name.value);
    }
    private void OnEnable()
    {
        _hostButton.clicked += GameState.HostGame;
        _joinButton.clicked += GameState.JoinGame;
        _quitButton.clicked += GameState.QuitGame;
        _name?.RegisterValueChangedCallback(UpdatePlayerName);
        _ip?.RegisterValueChangedCallback(UpdateIPAddress);
    }

    private void OnDisable()
    {
        if(_hostButton!= null)
        {
            _hostButton.clicked -= GameState.HostGame;
        }
        if(_joinButton!= null)
        {
            _joinButton.clicked -= GameState.JoinGame;
        }
        if(_quitButton!= null)
        {
            _quitButton.clicked -= GameState.QuitGame;
        }        
        _name?.UnregisterValueChangedCallback(UpdatePlayerName);
        _ip?.UnregisterValueChangedCallback(UpdateIPAddress);
    }

    private void UpdatePlayerName(ChangeEvent<string> evt)
    {
        // If the string is empty, use the default name.
        if (string.IsNullOrWhiteSpace(evt.newValue))
        {
            GameState.SetPlayerName(GameState.DefaultLocalPlayerName);
            return;
        }

        // Remove any invalid characters.
        string playerName = new(evt.newValue.Trim().ToCharArray().Where(x => !char.IsWhiteSpace(x)).ToArray());
        if (playerName != evt.newValue)
        {
            _name.value = playerName;
        }

        // Set the player name.
        GameState.SetPlayerName(playerName);
       
        
        PlayerPrefs.SetString(nameof(GameState.localPlayerName), playerName);
    }

    private void UpdateIPAddress(ChangeEvent<string> evt)
    {
        // If the string is empty, use the default address.
        if (string.IsNullOrWhiteSpace(evt.newValue))
        {
            NetworkManager.singleton.networkAddress = GameState.DefaultAddress;
            return;
        }

        // Remove any invalid characters.
        string networkAddress = new(evt.newValue.Trim().ToCharArray().Where(x => !char.IsWhiteSpace(x)).ToArray());
        if (networkAddress != evt.newValue)
        {
            _ip.value = networkAddress;
        }

        // Set the address.
        NetworkManager.singleton.networkAddress = networkAddress;
        PlayerPrefs.SetString(nameof(NetworkManager.singleton.networkAddress), networkAddress);
    }
}
