using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class CharacterOption : NetworkBehaviour
{
    private NetworkVariable<PlayerSerializable> chosenPlayer = new();

    [SerializeField] private TMP_Text playerNameText;

    [SerializeField] private string characterSpriteName;

    public override void OnNetworkSpawn() {
        chosenPlayer.OnValueChanged += SetPlayerChoiceUI;
        if (chosenPlayer.Value != null)
        {
            playerNameText.SetText(chosenPlayer.Value.username);
        }
    }

    public void OnClicked() {
        ulong cliendId = NetworkManager.Singleton.LocalClientId;
        SetPlayerServerRpc(cliendId);
    }

    private void SetPlayerChoiceUI(PlayerSerializable previousValue, PlayerSerializable newValue) {
        playerNameText.SetText(newValue.username);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerServerRpc(ulong clientId)
    {
        NetworkObject playerObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
        chosenPlayer.Value = playerObject.GetComponent<Player>().ToSerializedPlayer();
        Debug.Log(chosenPlayer.Value.username);
        playerObject.GetComponent<Player>().SetCharacterSprites(characterSpriteName);
    }
}
