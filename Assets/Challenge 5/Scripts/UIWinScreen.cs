using TMPro;
using UnityEngine;

public class UIWinScreen : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panel;
    public TextMeshProUGUI winText;

    [Header("Settings")]
    public string winMessage;
    public string ultraWinMessage;

    private void OnEnable()
    {
        GameManagerX.onWin += OnWin;
        GameManagerX.onUltraWin += OnUltraWin;
    }
    private void OnDisable()
    {
        GameManagerX.onWin -= OnWin;
        GameManagerX.onUltraWin -= OnUltraWin;
    }

    private void OnWin()
    {
        winText.text = winMessage;
    }
    private void OnUltraWin()
    {
        winText.text = ultraWinMessage;
    }

    private void Update()
    {
        // only show while in game win state
        panel.SetActive(GameManagerX.singleton.IsGameWon);
    }

    public void RestartGame()
    {
        GameManagerX.singleton.RestartGame();
    }
}
