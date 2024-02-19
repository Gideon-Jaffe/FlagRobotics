using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Utilities;

public class MapController : MonoBehaviour
{

    [SerializeField] private Tilemap tilemap;

    [SerializeField] private Tile leftBorder;
    [SerializeField] private Tile rightBorder;
    [SerializeField] private Tile forwardBorder;
    [SerializeField] private Tile backwardsBorder;

    [SerializeField] private int borderZHeight;

    public bool CanMove(Vector2Int startingLocation, Direction direction) {
        Vector3Int startingLocation3 = Vector2IntToVector3Int(startingLocation, borderZHeight);
        Vector3Int wantedLocation3 = Vector2IntToVector3Int(startingLocation + direction.DirectionVector(), borderZHeight);
        Debug.Log("Moving: " + direction);
        return direction switch {
            Direction.Forward => tilemap.GetTile(startingLocation3) != forwardBorder && tilemap.GetTile(wantedLocation3) != backwardsBorder,
            Direction.Right => tilemap.GetTile(startingLocation3) != rightBorder && tilemap.GetTile(wantedLocation3) != leftBorder,
            Direction.Back => tilemap.GetTile(startingLocation3) != backwardsBorder && tilemap.GetTile(wantedLocation3) != forwardBorder,
            Direction.Left => tilemap.GetTile(startingLocation3) != leftBorder && tilemap.GetTile(wantedLocation3) != rightBorder,
            _ => throw new System.NotImplementedException(),
        };
    }

    //[SerializeField] private List<Vector2Int> startingLocations;

    /*public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            var clients = NetworkManager.Singleton.ConnectedClientsList.Select(client => client.PlayerObject);
        }
    }*/

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
