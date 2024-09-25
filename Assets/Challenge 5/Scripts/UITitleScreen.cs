using UnityEngine;

public class UITitleScreen : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panel;

    private void Update()
    {
        // only show while in main menu state
        panel.SetActive(GameManagerX.singleton.IsInMainMenu);
    }
}
