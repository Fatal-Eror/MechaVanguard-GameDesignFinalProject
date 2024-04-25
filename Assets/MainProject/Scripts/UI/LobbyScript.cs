using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class LobbyScript : NetworkBehaviour
{
    private VisualElement _root;
    private Button _ready;
    private Button _changeToRed;
    private Button _changeToBlue;


    private bool isClientBindReady = false;
    private bool isServerBindStart = false;

    private void Start()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _ready = _root.Q<Button>("Ready");
        _changeToBlue = _root.Q<Button>("ChangeToBlue");
        _changeToRed = _root.Q<Button>("ChangeToRed");

        if(isServer)
        {
            InitializeHostReady();
        }
        else if(isClient)
        {
            InitializeClientReady();
        }
        else
        {
            InitializeErrorReady();
        }

        _changeToBlue.clicked += ChangeToBlue;
        _changeToRed.clicked += ChangeToRed;
    }

    private void Update()
    {
        if (isServer)
        {
            int readyNumber = 0;

            foreach (var player in GameNetwork.players)
            {
                if (player != null && player.ready) 
                {
                    readyNumber++;
                }
            }

            if (readyNumber == GameState.singleton.redTeamNumber + GameState.singleton.blueTeamNumber - 1) 
            {
                BindHostStart();
            }
            else
            {
                BindHostReady();
            }
        }
    }

    private void OnDestroy()
    {
        if (isClientBindReady)
        {
            _ready.clicked += ClientReadyToggle;
            isClientBindReady = false;
        }
        if (isServerBindStart)
        {
            _ready.clicked -= GameStartEvent;
            isServerBindStart = false;
        }

        _changeToBlue.clicked -= ChangeToBlue;
        _changeToRed.clicked -= ChangeToRed;
    }

    private void InitializeClientReady()
    {
        _ready.text = "READY?";

        // Totally black
        _ready.style.unityTextOutlineColor = new StyleColor(new Color(0f / 255, 0f / 255, 0f / 255));

        // Totally white
        _ready.style.borderLeftColor = new StyleColor(new Color(255f / 255, 255f / 255, 255f / 255));
        _ready.style.borderRightColor = new StyleColor(new Color(255f / 255, 255f / 255, 255f / 255));
        _ready.style.borderTopColor = new StyleColor(new Color(255f / 255, 255f / 255, 255f / 255));
        _ready.style.borderBottomColor = new StyleColor(new Color(255f / 255, 255f / 255, 255f / 255));

        // Grey
        _ready.style.backgroundColor = new StyleColor(new Color(185f / 255, 185f / 255, 185f / 255));

        _ready.clicked += ClientReadyToggle;

        isClientBindReady = true;
    }

    private void ClientReadyToggle()
    {
        // Update UI Display accroding to the old state of ready 
        if (GameState.localPlayer.ready)
        {
            _ready.text = "READY?";

            // Totally black Text
            _ready.style.unityTextOutlineColor = new StyleColor(new Color(0f / 255, 0f / 255, 0f / 255));
             
            // Totally white Border
            _ready.style.borderLeftColor = new StyleColor(new Color(255f / 255, 255f / 255, 255f / 255));
            _ready.style.borderRightColor = new StyleColor(new Color(255f / 255, 255f / 255, 255f / 255));
            _ready.style.borderTopColor = new StyleColor(new Color(255f / 255, 255f / 255, 255f / 255));
            _ready.style.borderBottomColor = new StyleColor(new Color(255f / 255, 255f / 255, 255f / 255));

            // Grey Background
            _ready.style.backgroundColor = new StyleColor(new Color(185f / 255, 185f / 255, 185f / 255));
        }
        else
        {
            _ready.text = "READY!";

            // Totally white Text
            _ready.style.unityTextOutlineColor = new StyleColor(new Color(255f / 255, 255f / 255, 255f / 255));

            // Totally black Border
            _ready.style.borderLeftColor = new StyleColor(new Color(0f / 255, 0f / 255, 0f / 255));
            _ready.style.borderRightColor = new StyleColor(new Color(0f / 255, 0f / 255, 0f / 255));
            _ready.style.borderTopColor = new StyleColor(new Color(0f / 255, 0f / 255, 0f / 255));
            _ready.style.borderBottomColor = new StyleColor(new Color(0f / 255, 0f / 255, 0f / 255));

            // Deep Grey Background
            _ready.style.backgroundColor = new StyleColor(new Color(63f / 255, 55f / 255, 55f / 255));
        }


        // Update ready attribute to new state
        GameState.localPlayer.UpdateReadyState();
    }

    private void InitializeHostReady()
    {
        _ready.text = "WAITING";

        // Totally black
        _ready.style.unityTextOutlineColor = new StyleColor(new Color(0f / 255, 0f / 255, 0f / 255));

        // Totally white
        _ready.style.borderLeftColor = new StyleColor(new Color(255f / 255, 255f / 255, 255f / 255));
        _ready.style.borderRightColor = new StyleColor(new Color(255f / 255, 255f / 255, 255f / 255));
        _ready.style.borderTopColor = new StyleColor(new Color(255f / 255, 255f / 255, 255f / 255));
        _ready.style.borderBottomColor = new StyleColor(new Color(255f / 255, 255f / 255, 255f / 255));

        // Grey
        _ready.style.backgroundColor = new StyleColor(new Color(185f / 255, 185f / 255, 185f / 255));
    }

    private void BindHostReady()
    {
        if (isServerBindStart)
        {
            _ready.text = "WAITING";

            // Totally black
            _ready.style.unityTextOutlineColor = new StyleColor(new Color(0f / 255, 0f / 255, 0f / 255));

            // Totally white
            _ready.style.borderLeftColor = new StyleColor(new Color(255f / 255, 255f / 255, 255f / 255));
            _ready.style.borderRightColor = new StyleColor(new Color(255f / 255, 255f / 255, 255f / 255));
            _ready.style.borderTopColor = new StyleColor(new Color(255f / 255, 255f / 255, 255f / 255));
            _ready.style.borderBottomColor = new StyleColor(new Color(255f / 255, 255f / 255, 255f / 255));

            // Grey
            _ready.style.backgroundColor = new StyleColor(new Color(185f / 255, 185f / 255, 185f / 255));

            _ready.clicked -= GameStartEvent;

            isServerBindStart = false;
        }            
    }

    private void BindHostStart()
    {
        if (!isServerBindStart)
        {
            _ready.text = "START";

            // Totally white Text
            _ready.style.unityTextOutlineColor = new StyleColor(new Color(255f / 255, 255f / 255, 255f / 255));

            // Totally black Border
            _ready.style.borderLeftColor = new StyleColor(new Color(0f / 255, 0f / 255, 0f / 255));
            _ready.style.borderRightColor = new StyleColor(new Color(0f / 255, 0f / 255, 0f / 255));
            _ready.style.borderTopColor = new StyleColor(new Color(0f / 255, 0f / 255, 0f / 255));
            _ready.style.borderBottomColor = new StyleColor(new Color(0f / 255, 0f / 255, 0f / 255));

            // Deep Grey Background
            _ready.style.backgroundColor = new StyleColor(new Color(63f / 255, 55f / 255, 55f / 255));

            _ready.clicked += GameStartEvent;

            isServerBindStart = true;
        }
    }

    private void GameStartEvent()
    {
        NetworkManager.singleton.ServerChangeScene("BattleMap");
    }

    private void InitializeErrorReady()
    {
        _ready.text = "???";

        // Totally black
        _ready.style.unityTextOutlineColor = new StyleColor(new Color(0f / 255, 0f / 255, 0f / 255));

        // Totally white
        _ready.style.borderLeftColor = new StyleColor(new Color(255f / 255, 255f / 255, 255f / 255));
        _ready.style.borderRightColor = new StyleColor(new Color(255f / 255, 255f / 255, 255f / 255));
        _ready.style.borderTopColor = new StyleColor(new Color(255f / 255, 255f / 255, 255f / 255));
        _ready.style.borderBottomColor = new StyleColor(new Color(255f / 255, 255f / 255, 255f / 255));

        // Grey
        _ready.style.backgroundColor = new StyleColor(new Color(185f / 255, 185f / 255, 185f / 255));
    }

    private void ChangeToRed()
    {
        GameState.localPlayer.ChangeTeamToRed();
    }

    private void ChangeToBlue()
    {
        GameState.localPlayer.ChangeTeamToBlue();
    }

}
