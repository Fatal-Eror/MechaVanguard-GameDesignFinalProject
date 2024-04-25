using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMapState : LevelState
{
    private void Start()
    {
        GameNetwork.condition = LevelCondition.Battle;
        CloseCursor();
    }
    public override void MovePlayerToSpawnPosition(GameObject player)
    {
        Vector3 spawnPosition;
        Quaternion spawnRotation;

        switch (GameState.belongingTeam)
        {
            case PlayerTeam.Red:
                spawnPosition = _redSpawnPoint[0].transform.position;
                spawnRotation = Quaternion.Euler(Vector3.zero);
                player.transform.position = spawnPosition;

                break;

            case PlayerTeam.Blue:
                spawnPosition = _blueSpawnPoint[0].transform.position;
                spawnRotation = Quaternion.Euler(Vector3.zero);
                player.transform.position = spawnPosition;

                break;

            default:
                Debug.LogError($"Error spawn in battle map state. MovePlayerToSpawnPosition with {GameState.belongingTeam}");
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

    private void CloseCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

}
