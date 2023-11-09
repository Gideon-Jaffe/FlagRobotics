using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameField;

    public void StartGame()
    {
        NetworkManager.Singleton.StartHost();
        NetworkObject networkObject = NetworkManager.Singleton.LocalClient.PlayerObject;
        networkObject.GetComponent<Player>().username.Value = usernameField.text;
        NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }

    public void Connect(TMP_InputField ipField)
    {
        string newUserName = usernameField.text;
        NetworkManager.Singleton.OnClientConnectedCallback += (ulong number) => {
            Player player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>();
            player.username.Value = newUserName;
            Debug.Log(player.username);
        };
        NetworkManager.Singleton.StartClient();
    }
}
