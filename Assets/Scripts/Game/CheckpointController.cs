using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CheckpointController : MonoBehaviour
{

    [SerializeField] private List<Vector2Int> checkpoints;

    [SerializeField] private Tilemap checkpointTileMap;

    [SerializeField] private Tile checkpointTile;

    private Dictionary<Player, int> capturedCheckpoints;

    // Start is called before the first frame update
    void Start()
    {
        capturedCheckpoints = new Dictionary<Player, int>();
        foreach (var checkpoint in checkpoints)
        {
            Vector3Int location = new(checkpoint.x, checkpoint.y, (int)checkpointTileMap.transform.position.z + 1);
            checkpointTileMap.SetTile(location, checkpointTile);
        }
        
    }

    public bool CaptureCheckpoint(Player player) {
        int nextCheckpointIndex = GetPlayerNextCheckpoint(player);
        Vector2Int nextCheckpoint = checkpoints[nextCheckpointIndex];
        Debug.Log("location: " + player.currentPoint);
        Debug.Log("next checkpoint before capture: " + nextCheckpoint);
        if (nextCheckpoint.Equals(player.currentPoint)) {
            capturedCheckpoints[player] = ++nextCheckpointIndex;
        }

        Debug.Log("NextCheckpout after capture: " + nextCheckpointIndex);
        return nextCheckpointIndex >= checkpoints.Count;
    }

    private int GetPlayerNextCheckpoint(Player player) {
        if (!capturedCheckpoints.TryGetValue(player, out int nextCheckpoint))
        {
            nextCheckpoint = 0;
            capturedCheckpoints.Add(player, nextCheckpoint);
        }
        return nextCheckpoint;
    }

    public List<Player> VictoriousPlayers() 
    {
        var victoriousPlayers = capturedCheckpoints.Where(
            player => player.Key.GetComponent<Player>().IsAlive &&
            player.Value >= checkpoints.Count).Select(player => player.Key);
        return victoriousPlayers.ToList();
    }
}
