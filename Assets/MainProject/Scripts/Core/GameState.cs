using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// There are 2 teams in game and these are the enums for them
public enum PlayerTeam
{
    None,
    Red,
    Blue
};

public enum LevelCondition
{
    None,
    Login,
    Lobby,
    Battle
};

public class GameState : NetworkBehaviour
{
    // singleton and point to self
    public static GameState singleton { get; private set; }

    // Observation mode
    public static Action OnGameStateStart;
    public static Action OnGameExit;


    // Player and network address settings
    public const string DefaultLocalPlayerName = "Default";
    public const string DefaultAddress = "localhost";

    public const string RedTagName = "RedPlayer";
    public const string BlueTagName = "BluePlayer";

    public static string localPlayerName;
    public static PlayerAttributes localPlayer;


    // It will record which level state is recorded in the current level
    // Like lobby level has lobby state
    public static LevelState currentLevelState { get; private set; }


    // Record number of people in each team
    [SyncVar] public int redTeamNumber = 0;
    [SyncVar] public int blueTeamNumber = 0;


    // Which team the local palyer belongs to, and which index player has
    public static PlayerTeam belongingTeam = PlayerTeam.None;
    public static int belongingIndex = -1;

    // Sensitivity of mouse movement, should be divided by 250 when be used
    public static float YawSensitivity = 50f;
    public static float PitchSensitivity = 50f;
    public static float defaultYawSensitivity = 50f;
    public static float defaultPitchSensitivity = 50f;

    // Kill and Death num of local player, will be set to 0 in each new battle
    public static int killNum = 0;
    public static int deathNum = 0;

    private void Awake()
    {
        // Register singleton
        if(singleton == null)
        {
            singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }    

        if(PlayerPrefs.HasKey(nameof(YawSensitivity)))
        {
            YawSensitivity = PlayerPrefs.GetFloat(nameof(YawSensitivity));
        }
        else
        {
            YawSensitivity = defaultYawSensitivity;
        }

        if (PlayerPrefs.HasKey(nameof(PitchSensitivity)))
        {
            PitchSensitivity = PlayerPrefs.GetFloat(nameof(PitchSensitivity));
        }
        else
        {
            PitchSensitivity = defaultPitchSensitivity;
        }
        
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += UpdateLevelState;
        SceneManager.sceneUnloaded += ClearLevelState;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= UpdateLevelState;
        SceneManager.sceneUnloaded -= ClearLevelState;
    }
    // Function for UI
    public static void HostGame()
    {
        NetworkManager.singleton.StartHost();
    }

    public static void JoinGame()
    {
        NetworkManager.singleton.StartClient();
    }

    public static void QuitGame()
    {
        OnGameExit?.Invoke();
        Application.Quit();
    }

    // Used by text field UI to set local player name
    public static void SetPlayerName(String name)
    {
        localPlayerName = name.Trim();
    }

    // Level state relevant
    // update will be called when new scene starts
    private void UpdateLevelState(Scene arg0, LoadSceneMode arg1)
    {
        currentLevelState = GameObject.FindFirstObjectByType<LevelState>();
    }

    // Clear will be called when current scene unloads
    private void ClearLevelState(Scene arg0)
    {
        currentLevelState = null;
    }
}
