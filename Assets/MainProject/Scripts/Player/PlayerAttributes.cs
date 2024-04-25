using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.InputSystem.XR;

public class PlayerAttributes : NetworkBehaviour
{
    [SyncVar] public string playerName;
    [SyncVar] public PlayerTeam team = PlayerTeam.None;


    public GameObject redTeamModel;
    public GameObject blueTeamModel;


    [SyncVar] public bool ready = false;

    [SyncVar] public float healthPoint = 200;
    [SyncVar] public float maxHealPoint = 200;
    // Record score of this battle, can be checked in the present battle and the lobby after this battle
    // But these will be initialized at the next battle start
    /*[HideInInspector]*/[SyncVar] public int killNum = 0;
    /*[HideInInspector]*/[SyncVar] public int deathNum = 0;
    [SerializeField] private float respawnTime = 5f;
    [SerializeField] private AudioClip deadExplosionSound;
    [SerializeField] private ParticleSystem deadExlosionVisual;

    private CharacterController _controller;
    private CapsuleCollider _collider;
    private bool _isDead = false;  


    private void Awake()
    {
        GameNetwork.players.Add(this);
    }

    private void Start()
    {
        // print(transform.position.ToString());
        // If local player's team and index is not default value
        // Then place them to spawn points which is decided by current level state
        if (GameState.belongingTeam != PlayerTeam.None && GameState.belongingIndex != -1)
        {
            GameState.currentLevelState.MovePlayerToSpawnPosition(this.gameObject);
        }
        else
        {
            Debug.LogWarning($"Player {playerName} does not have team and index in Player Attributes, OnStartLocalPlayer");
        }

        // Initialize the kill and death in each new battle
        if(GameNetwork.condition == LevelCondition.Battle)
        {
            killNum = 0;
            deathNum = 0;
            GameState.killNum = 0;
            GameState.deathNum = 0;
        }

        _controller = GetComponent<CharacterController>();
        _collider = GetComponent<CapsuleCollider>();

        if(GameNetwork.condition == LevelCondition.Lobby && isLocalPlayer)
        {
            killNum = GameState.killNum;
            deathNum = GameState.deathNum;
            UpdateKillAndDeathInLobby(GameState.killNum, GameState.deathNum);
        }
    }

    [Command]
    private void UpdateKillAndDeathInLobby(int kill, int death)
    {
        killNum = kill;
        deathNum = death;
    }

    private void Update()
    {
        // Decide which model should be actived to show, red model or blue model
        UpdateModel();

        if(isLocalPlayer && GameNetwork.condition == LevelCondition.Battle)
        {
            GameState.killNum = killNum;
            GameState.deathNum = deathNum;
        }
    }


    private void OnDestroy()
    {
        if (isClient)
        {
            GameState.localPlayer = null;
        }

        GameNetwork.players.Remove(this);
    }


    public override void OnStartLocalPlayer()
    {
        GameState.localPlayer = this;
        SetPlayerNameOnLocal(GameState.localPlayerName);
        SetPlayerNameOnNetwork(GameState.localPlayerName);

        // If local player's team and index is not set(default value is none and -1)
        // Give local player a team and index
        if (GameState.belongingTeam == PlayerTeam.None && GameState.belongingIndex == -1)
        {
            if (GameState.singleton.redTeamNumber <= GameState.singleton.blueTeamNumber)
            { 
                GameState.belongingTeam = PlayerTeam.Red;
                GameState.belongingIndex = GameState.singleton.redTeamNumber;
                UpdateRedTeamNum(GameState.singleton.redTeamNumber + 1);
                
            }
            else
            {
                GameState.belongingTeam = PlayerTeam.Blue;
                GameState.belongingIndex = GameState.singleton.blueTeamNumber;
                UpdateBlueTeamNum(GameState.singleton.blueTeamNumber + 1);
                
            }
        }
        else
        {
            Debug.Log($"Player already has team and index: {GameState.belongingTeam}, {GameState.belongingIndex}");
        }


        /*// If local player's team and index is not default value
        // Then place them to spawn points which is decided by current level state
        if (GameState.belongingTeam != PlayerTeam.None && GameState.belongingIndex != -1)
        {
            GameState.currentLevelState.MovePlayerToSpawnPosition(this.gameObject);
        }
        else
        {
            Debug.LogWarning($"Player {playerName} does not have team and index in Player Attributes, OnStartLocalPlayer");
        }*/

        SetPlayerTeam(GameState.belongingTeam);        
    }


    // It seems command and syncVar will only update the value in server and other clients
    // but not self, so I have to set local value at first, then set server and other clients with command
    [Command]
    private void SetPlayerNameOnNetwork(string PlayerName)
    {
        SetPlayerName(PlayerName);
    }

    private void SetPlayerNameOnLocal(string PlayerName)
    {
        SetPlayerName(PlayerName);
    }

    private void SetPlayerName(string PlayerName)
    {
        playerName = PlayerName;
    }


    [Command]
    private void SetPlayerTeamOnNetwork(PlayerTeam newTeam)
    {
        SetPlayerTeam(newTeam);
    }

    private void SetPlayerTeamOnLocal(PlayerTeam newTeam)
    {
        SetPlayerTeam(newTeam);
    }

    private void SetPlayerTeam(PlayerTeam newTeam)
    {
        team = newTeam;
    }

    // Decide which model should be actived to show, red model or blue model
    // Will be called in update
    public void UpdateModel()
    {     
        switch (team)
        {
            case PlayerTeam.Red:
                redTeamModel.SetActive(_isDead ? false : true);
                blueTeamModel.SetActive(false);
                tag = GameState.RedTagName;
                break;

            case PlayerTeam.Blue:
                blueTeamModel.SetActive(_isDead ? false : true);
                redTeamModel.SetActive(false);
                tag = GameState.BlueTagName;
                break;

            default:
                if (isServer)
                {
                    Debug.Log("Server cannot update model in PlayerAttributes, OnStartLocalPlayer");
                }
                else if (isClient)
                {
                    Debug.Log($"Client {playerName} cannot update model in PlayerAttributes, OnStartLocalPlayer");
                }
                else
                {
                    Debug.Log("Running PlayerAttributes, OnStartLocalPlayer in machine which is not server and not client");
                }

                redTeamModel.SetActive(false);
                blueTeamModel.SetActive(false);
                break;
        }
    }


    // Use command to update the syncVar value in server
    [Command]
    public void UpdateRedTeamNum(int newValue)
    {
        GameState.singleton.redTeamNumber = newValue;
    }


    // Use command to update the syncVar value in server
    [Command]
    public void UpdateBlueTeamNum(int newValue)
    {
        GameState.singleton.blueTeamNumber = newValue;
    }

    public void UpdateReadyState()
    {
        ready = ready ? false : true;
        UpdateReadyStateOnServer(ready);
    }

    [Command]
    public void UpdateReadyStateOnServer(bool isReady)
    {
        ready = isReady;
    }

    public void ChangeTeamToRed()
    {
        if(!ready && team == PlayerTeam.Blue && GameState.singleton.redTeamNumber < GameState.currentLevelState._redSpawnPoint.Count)
        {
            GameState.belongingTeam = PlayerTeam.Red;
            GameState.belongingIndex = GameState.singleton.redTeamNumber;
            UpdateRedTeamNum(GameState.singleton.redTeamNumber + 1);
            UpdateBlueTeamNum(GameState.singleton.blueTeamNumber - 1);
            GameState.currentLevelState.MovePlayerToSpawnPosition(this.gameObject);
            SetPlayerTeam(GameState.belongingTeam);
        }       
    }

    public void ChangeTeamToBlue()
    {
        if (!ready && team == PlayerTeam.Red && GameState.singleton.blueTeamNumber < GameState.currentLevelState._blueSpawnPoint.Count)
        {
            GameState.belongingTeam = PlayerTeam.Blue;
            GameState.belongingIndex = GameState.singleton.blueTeamNumber;
            UpdateBlueTeamNum(GameState.singleton.blueTeamNumber + 1);
            UpdateRedTeamNum(GameState.singleton.redTeamNumber - 1);
            GameState.currentLevelState.MovePlayerToSpawnPosition(this.gameObject);
            SetPlayerTeam(GameState.belongingTeam);
        }
    }


    [Server]
    public void GetDamage(PlayerAttributes instigator, float originalDamage)
    {
        if (instigator.team == this.team || healthPoint <= 0)
        {
            return;
        }

        healthPoint = Mathf.Max(healthPoint - originalDamage, 0);
        UpdateHealth(healthPoint);
        // print(playerName + " get damage " + originalDamage + " from " + instigator.playerName);

        if(healthPoint <= 0)
        {
            instigator.killNum++;
            UpdateKillNum(instigator.killNum, instigator);

            deathNum++;
            UpdateDeathNum(deathNum);
        }
    }

    [ClientRpc]
    private void UpdateHealth(float value)
    {
        healthPoint = value;
    }

    [ClientRpc]
    private void UpdateKillNum(int value, PlayerAttributes updateTarget)
    {
        updateTarget.killNum = value;
    }

    [ClientRpc]
    private void UpdateDeathNum(int value)
    {
        deathNum = value;
        StartCoroutine(DeadAndRespawn());
    }

    private IEnumerator DeadAndRespawn()
    {
/*        if (!isLocalPlayer)
        {
            yield break;
        }*/

        _controller.enabled = false;
        _isDead = true;
        AudioSource deathSound = gameObject.AddComponent<AudioSource>();
        deathSound.clip = deadExplosionSound;
        deathSound.Play();
        deadExlosionVisual.Play();
        Destroy(deathSound, 5);
        _collider.enabled = false;
        print("set anime and controller false");

        yield return new WaitForSeconds(respawnTime);

        GameState.currentLevelState.MovePlayerToSpawnPosition(gameObject);

        _controller.enabled = true;
        _isDead = false;
        healthPoint = maxHealPoint;
        _collider.enabled = true;
        print("set anime and controller true");        
    }
}


