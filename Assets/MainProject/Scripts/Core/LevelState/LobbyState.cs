using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyState : LevelState
{
    private void Start()
    {
        GameNetwork.condition = LevelCondition.Lobby;
        EnableCursor();
    }

    public override void MovePlayerToSpawnPosition(GameObject player)
    {
        Vector3 spawnPosition;
        Quaternion spawnRotation;

        switch (GameState.belongingTeam)
        {
            case PlayerTeam.Red:
                spawnPosition = _redSpawnPoint[GameState.belongingIndex].transform.position;
                spawnRotation = Quaternion.Euler(Vector3.zero);
                player.transform.position = spawnPosition;

                break;

            case PlayerTeam.Blue:
                spawnPosition = _blueSpawnPoint[GameState.belongingIndex].transform.position;
                spawnRotation = Quaternion.Euler(Vector3.zero);
                player.transform.position = spawnPosition;

                break;

            default:
                Debug.LogError($"Error spawn in level state. MovePlayerToSpawnPosition with {GameState.belongingTeam}");
                break;
        }
    }

    public override GameObject SpawnPlayer(GameObject player, NetworkConnectionToClient conn)
    {
        GameObject thePlayer;

        thePlayer = Instantiate(player);
        NetworkServer.AddPlayerForConnection(conn, thePlayer);
        

        return thePlayer;
    }

    private void EnableCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
