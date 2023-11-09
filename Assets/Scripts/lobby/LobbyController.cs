using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyController : NetworkBehaviour
{
    [SerializeField] private Button lockInButton;

    [SerializeField] private Button startGameButton;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) 
        {
            startGameButton.interactable = false;
            TMP_Text buttonText = startGameButton.GetComponentInChildren<TMP_Text>();
            buttonText.SetText("Server Must Start Game");
        }
    }

    public void LockIn()
    {
        ulong clientId = NetworkManager.Singleton.LocalClientId;
        LockInServerRpc(clientId);
    }

    public void StartGame()
    {
        if (!IsServer)
        {
            Debug.Log("Only the host can start the game!");
            return;
        }
        NetworkManager.Singleton.SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }

    [ServerRpc(RequireOwnership = false)]
    public void LockInServerRpc(ulong clientId)
    {
        Player player = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<Player>();
        player.isLockedIn.Value = !player.isLockedIn.Value;
        SetLockedInButtonClientRpc(player.isLockedIn.Value, new ClientRpcParams {Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> {clientId}}});
    }

    [ClientRpc]
    private void SetLockedInButtonClientRpc(bool isLockedIn, ClientRpcParams rpcParams)
    {
        TMP_Text buttonText = lockInButton.GetComponentInChildren<TMP_Text>();
        string newText = isLockedIn? "LOCKED IN" : "CLICK_TO_LOCK_IN";
        buttonText.SetText(newText);
    }
}
