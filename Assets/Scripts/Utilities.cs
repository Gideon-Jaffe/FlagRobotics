using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class Utilities
{
    public static Vector3Int Vector2IntToVector3Int(Vector2Int originalVector, int z) {
        Vector3Int vector = new()
        {
            x = originalVector.x,
            y = originalVector.y,
            z = z
        };
        return vector;
    }

    public static bool CheckIfBlank(Vector3Int targetLocation, Tilemap board) {
        Debug.Log("checking if tile is blank: " + targetLocation);
        return !board.HasTile(targetLocation);
    }

    public enum Direction
    {
        Forward = 0, Right = 1, Back = 2, Left = 3,
    }

    public static Vector2Int DirectionVector(this Direction direction)
    {
        return direction switch
        {
            Direction.Forward => Vector2Int.up,
            Direction.Right => Vector2Int.right,
            Direction.Back => Vector2Int.down,
            Direction.Left => Vector2Int.left,
            _ => throw new System.NotImplementedException(),
        };
    }

    public static Direction Reverse(this Direction direction)
    {
        return direction switch
        {
            Direction.Forward => Direction.Back,
            Direction.Right => Direction.Left,
            Direction.Back => Direction.Forward,
            Direction.Left => Direction.Right,
            _ => throw new System.NotImplementedException(),
        };
    }
}
