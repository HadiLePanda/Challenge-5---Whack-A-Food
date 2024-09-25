using TMPro;
using UnityEngine;

public class UIGameHUD : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panel;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI targetScoreText;
    public TextMeshProUGUI targetTimeText;

    private void Update()
    {
        // only show while in playing state
        panel.SetActive(GameManagerX.singleton.IsGameActive);

        // update the score text
        scoreText.text = $"Score: {GameManagerX.singleton.Score} / {GameManagerX.singleton.TargetWinScore}";

        // update timer text
        timerText.text = $"Time: {Mathf.Round(GameManagerX.singleton.GameTimeLeft)}";
        targetTimeText.text = $"Ultra Time Mark: {GameManagerX.singleton.GetTargetUltraWinMinimumTime()}";
    }
}
