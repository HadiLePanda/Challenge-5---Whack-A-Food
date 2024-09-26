using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public enum GameState
{
    MainMenu,
    Playing,
    Won,
    GameOver
}

public class GameManagerX : MonoBehaviour
{
    [Header("References")]
    public List<GameObject> targetPrefabs;

    [Header("Settings")]
    [SerializeField] private float spawnRate = 1.5f;
    [SerializeField] private int baseWinScore = 100;
    [SerializeField] private float gameDuration = 30f;
    [SerializeField] private float musicMenuVolume = 0.1f;
    [SerializeField] private float musicGameplayVolume = 0.3f;
    [SerializeField] private float scorePercentageIncrement = 0.2f;
    [SerializeField] private float spawnRatePercentageIncrement = 0.25f;
    [SerializeField] private float ultraWinTimeMultiplier = 0.2f;

    [Header("Sounds")]
    [SerializeField] private AudioClip startSound;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip gameWinSound;
    [SerializeField] private AudioClip ultraWinSound;

    public GameState GameState { get ; private set; }
    public int Difficulty { get; private set; }
    public int TargetWinScore { get; private set; }
    private int _score;
    public int Score
    {
        get => _score;
        set => SetScore(value);
    }
    private bool hasUltraWon = false;
    public bool HasUltraWon => hasUltraWon;
    private float gameStartTime;
    private float gameTimeLeft;
    public float GameTimeLeft => gameTimeLeft;
    public float RespawnRate => spawnRate;

    public bool IsInMainMenu => GameState == GameState.MainMenu;
    public bool IsGameActive => GameState == GameState.Playing;
    public bool IsGameWon => GameState == GameState.Won;
    public bool IsGameOver => GameState == GameState.GameOver;

    private float spaceBetweenSquares = 2.5f;
    private float minValueX = -3.75f; //  x value of the center of the left-most square
    private float minValueY = -3.75f; //  y value of the center of the bottom-most square

    public static Action onGameStarted;
    public static Action onGameOver;
    public static Action onWin;
    public static Action onUltraWin;
    //public static Action<GameState> onGameStateChanged;

    public static GameManagerX singleton;
    
    private void Awake()
    {
        singleton = this;
    }

    private void Start()
    {
        // enter main menu state as the default state
        GameState = GameState.MainMenu;

        // update the game timer
        gameTimeLeft = gameDuration;

        // play the music with a lower volume
        AudioManager.singleton.SetMusicVolume(musicMenuVolume);
        AudioManager.singleton.PlayMusic();
    }

    private void Update()
    {
        ProcessTimerLogic();
    }

    private void ProcessTimerLogic()
    {
        if (IsGameActive)
        {
            // no more time left
            if (gameTimeLeft <= 0)
            {
                // trigger game over
                GameOver();
                return;
            }

            // count down the timer
            gameTimeLeft = Mathf.Max(0, gameTimeLeft - Time.deltaTime);
        }
    }

    public float GetTargetUltraWinMinimumTime() => gameDuration * ultraWinTimeMultiplier;

    public void StartGame(int difficulty)
    {
        if (IsGameActive)
            return;

        // set game difficulty
        Difficulty = difficulty;

        // reset score
        _score = 0;

        // adjust values based on difficulty chosen
        float spawnRateMultiplier = GetSpawnrateDifficultyMultiplier();
        float scoreMultiplier = GetScoreDifficultyMultiplier();
        spawnRate /= Mathf.Max(1, spawnRateMultiplier);
        TargetWinScore = (int)Mathf.Round(baseWinScore * scoreMultiplier);

        // play game start sound
        AudioManager.singleton.PlaySound2DOneShot(startSound);

        // make the music louder
        AudioManager.singleton.SetMusicVolume(musicGameplayVolume);

        // make game active and start spawning targets
        GameState = GameState.Playing;
        gameStartTime = Time.time;
        StartCoroutine(ProcessTargetSpawning());

        onGameStarted?.Invoke();
    }

    // handle spawning targets over time
    private IEnumerator ProcessTargetSpawning()
    {
        while (IsGameActive)
        {
            // wait for timer
            yield return new WaitForSeconds(spawnRate);

            // spawn a random target
            if (IsGameActive)
            {
                int index = Random.Range(0, targetPrefabs.Count);
                Instantiate(targetPrefabs[index], RandomSpawnPosition(), targetPrefabs[index].transform.rotation);
            }
        }
    }

    // update score
    public void AddScore(int scoreToAdd)
    {
        Score += scoreToAdd;
    }
    public void SetScore(int score)
    {
        if (IsGameActive)
        {
            // update the score
            _score = score;

            // dropped below 0 score, game over
            if (score < 0)
            {
                GameOver();
            }
            // achieved win score
            else if (score >= TargetWinScore)
            {
                // there are some time left, trigger ultra win
                if (gameTimeLeft >= GetTargetUltraWinMinimumTime())
                {
                    UltraWin();
                }
                // trigger normal win
                else
                {
                    Win();
                }
            }
        }
    }

    // we lose the game
    public void GameOver()
    {
        // set game state to game over
        GameState = GameState.GameOver;

        // stop the music
        AudioManager.singleton.StopMusic();

        // play game over sound
        AudioManager.singleton.PlaySound2DOneShot(gameOverSound);

        onGameOver?.Invoke();
    }

    // we survived the timer
    private void Win()
    {
        // set game state to win
        GameState = GameState.Won;

        // play normal win sound
        AudioManager.singleton.PlaySound2DOneShot(gameWinSound);

        onWin?.Invoke();
    }

    // we win while having a certain time amount left
    private void UltraWin()
    {
        // set ultra win flag
        hasUltraWon = true;

        // set game state to win
        GameState = GameState.Won;

        // play ultra win sound
        AudioManager.singleton.PlaySound2DOneShot(gameWinSound);
        AudioManager.singleton.PlaySound2DOneShot(ultraWinSound);

        onUltraWin?.Invoke();
    }

    // restart game by reloading the scene
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public float GetScoreDifficultyMultiplier()
    {
        return GetMultiplierByLevel(Difficulty, 1f, scorePercentageIncrement);
    }
    public float GetSpawnrateDifficultyMultiplier()
    {
        return GetMultiplierByLevel(Difficulty, 1f, spawnRatePercentageIncrement);
    }
    public float GetMultiplierByLevel(int level, float baseMultiplier, float percentageIncreasePerLevel)
    {
        // for level 1,
        // the speed should be at base multiplier
        if (level == 1)
            return baseMultiplier;

        // for difficulty levels after level 1,
        // flat increase of x% every level
        return baseMultiplier + (percentageIncreasePerLevel * (level - 1));
    }

    // generate a random spawn position based on a random index from 0 to 3
    private Vector3 RandomSpawnPosition()
    {
        float spawnPosX = minValueX + (RandomSquareIndex() * spaceBetweenSquares);
        float spawnPosY = minValueY + (RandomSquareIndex() * spaceBetweenSquares);

        Vector3 spawnPosition = new(spawnPosX, spawnPosY, 0);
        return spawnPosition;
    }

    // generates random square index from 0 to 3, which determines which square the target will appear in
    private int RandomSquareIndex()
    {
        return Random.Range(0, 4);
    }
}
