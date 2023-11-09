using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class MapController : NetworkBehaviour
{

    [SerializeField] private List<Vector2Int> startingLocations;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            var clients = NetworkManager.Singleton.ConnectedClientsList.Select(client => client.PlayerObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
