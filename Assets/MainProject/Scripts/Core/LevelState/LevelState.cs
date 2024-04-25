using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelState : MonoBehaviour
{
    public List<GameObject> _redSpawnPoint;
    public List<GameObject> _blueSpawnPoint;
    public abstract GameObject SpawnPlayer(GameObject player, NetworkConnectionToClient conn);
    public abstract void MovePlayerToSpawnPosition(GameObject player);
}
