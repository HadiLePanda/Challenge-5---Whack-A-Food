using UnityEngine;
using UnityEngine.UI;

public class DifficultyButtonX : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button button;

    [Header("Settings")]
    [SerializeField] private int difficulty = 1;

    private void Start()
    {
        // setup button click event
        button.onClick.AddListener(() => SetDifficulty(difficulty));
    }

    /* when a button is clicked, call the StartGame() method
     * and pass it the difficulty value (1, 2, 3) from the button */
    private void SetDifficulty(int difficulty)
    {
        Debug.Log(button.gameObject.name + " was clicked");
        GameManagerX.singleton.StartGame(difficulty);
    }
}
