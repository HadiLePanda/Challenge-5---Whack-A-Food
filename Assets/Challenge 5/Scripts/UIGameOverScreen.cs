using UnityEngine;

public class UIGameOverScreen : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panel;

    private void Update()
    {
        // only show while in game over state
        panel.SetActive(GameManagerX.singleton.IsGameOver);
    }

    public void RestartGame()
    {
        GameManagerX.singleton.RestartGame();
    }
}
