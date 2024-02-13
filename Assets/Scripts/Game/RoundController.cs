using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class RoundController : NetworkBehaviour
{
    [SerializeField] private Tilemap boardTileMap;

    [SerializeField] private ActionController actionController;

    [SerializeField] private List<Vector2Int> startingLocations;

    [SerializeField] private CardController cardController;

    [SerializeField] private Button lockInButton;

    private Dictionary<Player, PlayerCards> playerCardsInHands;

    public override void OnNetworkSpawn()
    {
        lockInButton.interactable = false;
        base.OnNetworkSpawn();
        if (IsServer)
        {
            var clients = NetworkManager.ConnectedClientsList;
            int current = 0;
            foreach (NetworkClient client in clients)
            {   
                Player player = client.PlayerObject.GetComponent<Player>();
                player.currentPoint = startingLocations[current];
                player.transform.position = boardTileMap.CellToWorld(Utilities.Vector2IntToVector3Int(startingLocations[current], 2));
                current++;
                player.SetCharacterDirection(Utilities.Direction.Forward);
            }
        }
    }

    public void StartRound()
    {
        playerCardsInHands = new();
        var clients = NetworkManager.ConnectedClientsList;
        foreach (var client in clients)
        {
            var player = client.PlayerObject.GetComponent<Player>();
            List<int> cards = cardController.GetCardIds(9);
            playerCardsInHands[player] = new PlayerCards(cards);
            
            GetCardsClientRpc(CardIdsToString(cards), new ClientRpcParams { Send = new ClientRpcSendParams {TargetClientIds = new ulong[]{client.ClientId}}});
        }
    }

    private string CardIdsToString(List<int> ints)
    {
        MemoryStream memoryStream = new MemoryStream();
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(memoryStream, ints);
        return Convert.ToBase64String(memoryStream.GetBuffer());
    }

    private List<int> StringToCardIds(string data) 
    {
        MemoryStream stream = new MemoryStream(Convert.FromBase64String(data));
        BinaryFormatter formatter = new BinaryFormatter();
        return (List<int>)formatter.Deserialize(stream);
    }

    [ClientRpc]
    public void GetCardsClientRpc(string cards, ClientRpcParams rpcParams)
    {
        cardController.GetCards(StringToCardIds(cards));
    }

    public void PickCards(List<CardScript> cards)
    {
        if (cards.Count == 5)
        {
            lockInButton.interactable = true;
        }
        string data = CardIdsToString(cards.Select(card => card.Id).ToList());
        PickCardServerRpc(data, NetworkManager.Singleton.LocalClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void PickCardServerRpc(string chosenCards, ulong clientId)
    {
        Player player = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<Player>();
        playerCardsInHands[player].ChosenCards = StringToCardIds(chosenCards);
    }

    public void LockIn()
    {
        LockInServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    [ServerRpc]
    private void LockInServerRpc(ulong clientId)
    {
        NetworkObject playerObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
        playerObject.GetComponent<Player>().isLockedIn.Value = !playerObject.GetComponent<Player>().isLockedIn.Value;

        List<Player> players = NetworkManager.Singleton.ConnectedClientsList.Select(networkClient => networkClient.PlayerObject.GetComponent<Player>()).ToList();
        if (players.TrueForAll(player => player.isLockedIn.Value))
        {
            MovePlayerServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void MovePlayerServerRpc()
    {
        List<Player> players = NetworkManager.Singleton.ConnectedClientsList.Select(networkClient => networkClient.PlayerObject.GetComponent<Player>()).ToList();
        actionController.AddAction(new SubmitPlayerActions(players, playerCardsInHands));
    }
}

public class PlayerCards
{
    public List<int> CardsInHand {get; set;}
    public List<int> ChosenCards {get; set;}

    public PlayerCards(List<int> cardsInHand)
    {
        CardsInHand = cardsInHand;
        ChosenCards = new List<int>();
    }
}
