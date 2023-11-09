using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : NetworkBehaviour
{
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private TMP_Text playerWon;

    [ClientRpc]
    public void SetGameOverClientRpc(string victoriousPlayer) {
        Debug.Log("GAME OVER - WINNER: " + victoriousPlayer);
        gameOverScreen.SetActive(true);
        playerWon.SetText(victoriousPlayer);
        Time.timeScale = 0;
    }

    public void RestartGame() {
        Time.timeScale = 1;
        NetworkManager.Shutdown();
        Destroy(NetworkManager.gameObject);
        SceneManager.LoadScene(0);
    }
}
