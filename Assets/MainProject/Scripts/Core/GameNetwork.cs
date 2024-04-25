using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System;

public class GameNetwork : NetworkManager
{
    // singleton, Instance point to self
    public static GameNetwork Instance;

    // is players all leave lobby and move to game
    public static bool isInBattle = false;
    public static LevelCondition condition = LevelCondition.None;

    // List of Players
    public static HashSet<PlayerAttributes> players = new();


    public override void Start()
    {
        base.Start();

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        // if game has started or lobby is full with players, reject
        if (condition == LevelCondition.Battle || ((GameState.singleton.redTeamNumber + GameState.singleton.blueTeamNumber)
                            >= (GameState.currentLevelState._redSpawnPoint.Count + GameState.currentLevelState._blueSpawnPoint.Count)))
        {
            conn.Disconnect();
            return;
        }

        base.OnServerConnect(conn);
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (GameState.currentLevelState != null)
        {
            GameObject player = GameState.currentLevelState.SpawnPlayer(playerPrefab, conn);
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);

        GameState.singleton.redTeamNumber = GameObject.FindGameObjectsWithTag(GameState.RedTagName).Length;
        GameState.singleton.blueTeamNumber = GameObject.FindGameObjectsWithTag(GameState.BlueTagName).Length;
    }
}
