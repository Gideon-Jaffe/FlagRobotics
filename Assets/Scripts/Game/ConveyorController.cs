using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ConveyorController : MonoBehaviour
{
    [SerializeField] private Tilemap board;

    [SerializeField] private List<TileDetails> conveyorToAction;

    [SerializeField] public int maxConveyorSpeed;

    public bool IsOnConveyor(Player player, out Utilities.Direction direction, out int amount)
    {
        var tile = board.GetTile(Utilities.Vector2IntToVector3Int(player.currentPoint, 0));
        TileDetails tileDetails = conveyorToAction.Find(tileWrapper => tileWrapper.tile == tile);
        if (tileDetails != null) 
        {
            direction = tileDetails.direction;
            amount = tileDetails.amount;
            return true;
        }
        else
        {
            direction = Utilities.Direction.Forward;
            amount = 0;
            return false;
        }
    }

    public Dictionary<Player, Utilities.Direction> PlayersToMove(List<Player> players, int minConveyorSpeed) 
    {
        Dictionary<Player, Utilities.Direction> movingPlayers = new();
        Dictionary<Vector2Int, int> spacesMovingTo = new();
        foreach (var player in players)
        {
            if (IsOnConveyor(player, out Utilities.Direction direction, out int conveyorSpeed) && conveyorSpeed >= minConveyorSpeed)
            {
                movingPlayers[player] = direction;
                Vector2Int space = player.GetCurrentPoint() + direction.DirectionVector();
                spacesMovingTo[space] = spacesMovingTo.GetValueOrDefault(space, 0) + 1;
            }
        }
        Debug.Log("Check 1: " + movingPlayers.Count);
        List<Player> playersToDelete = movingPlayers
            .Where(pair => spacesMovingTo[pair.Key.GetCurrentPoint() + pair.Value.DirectionVector()] > 1)
            .Select(pair => pair.Key).ToList();
        playersToDelete.ForEach(player => movingPlayers.Remove(player));
        Debug.Log("Check 2: " + movingPlayers.Count);
        List<Vector2Int> LocationOfNonMovingPlayers = new();
        foreach (var player in players)
        {
            if (!movingPlayers.ContainsKey(player))
            {
                LocationOfNonMovingPlayers.Add(player.currentPoint);
            }
        }
        playersToDelete = movingPlayers
            .Where(pair => LocationOfNonMovingPlayers.Contains(pair.Key.GetCurrentPoint() + pair.Value.DirectionVector()))
            .Select(pair => pair.Key).ToList();
        playersToDelete.ForEach(player => movingPlayers.Remove(player));

        return movingPlayers;
    }
}

[Serializable]
public class TileDetails {
    public Tile tile;
    public Utilities.Direction direction;

    public int amount;
}
