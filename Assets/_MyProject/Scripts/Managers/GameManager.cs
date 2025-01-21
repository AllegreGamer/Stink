using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Game Time Settings")]
    [SerializeField] private float initialTime;
    [SerializeField] private float gameTime;
    [SerializeField] private bool gameStarted = false;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Game Events")]
    [SerializeField] private List<GameEvent> gameEvents = new List<GameEvent>();

    [Header("Enemy Spawn Settings")]
    [SerializeField] private EnemySpawner enemySpawner;

    // Eventi Unity
    public static event Action<GameEvent> OnGameEventTriggered;
    public static event Action<float> OnGameTimeUpdated;

    private static GameManager instance;
    public static GameManager Instance => instance;

    private bool isPaused = false;
    private InputSystem_Actions playerInput;
    private int currentLevelIndex = 1; // Cominciamo dal livello 1

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        playerInput = new InputSystem_Actions();
        playerInput.UI.Start.performed += OnPause;
        Time.timeScale = 1f;
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    private void Start()
    {
        if (timerText == null)
        {
            Debug.LogError("Timer Text reference not set in GameManager!");
        }

        gameEvents.Sort((a, b) => a.timeToTrigger.CompareTo(b.timeToTrigger));
        StartGame();
    }

    private void Update()
    {
        if (!gameStarted || isPaused) return;

        // Sottrae il tempo
        gameTime -= Time.deltaTime;

        // Controlla se il tempo è finito
        if (gameTime <= 0)
        {
            gameTime = 0;
            GameOver();
        }

        UpdateTimerUI();
        OnGameTimeUpdated?.Invoke(gameTime);
        CheckEvents();
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(gameTime / 60);
            int seconds = Mathf.FloorToInt(gameTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    private void CheckEvents()
    {
        float currentMinutes = gameTime / 60f;

        foreach (var gameEvent in gameEvents)
        {
            if (!gameEvent.hasTriggered &&
                gameEvent.levelNumber == currentLevelIndex &&
                currentMinutes >= gameEvent.timeToTrigger)
            {
                TriggerEvent(gameEvent);
                gameEvent.hasTriggered = true;
            }
        }
    }

    private void TriggerEvent(GameEvent gameEvent)
    {
        switch (gameEvent.eventType)
        {
            case GameEventType.IncreaseEnemyCount:
                if (enemySpawner != null)
                {
                    enemySpawner.IncreaseMaxEnemies(gameEvent.enemyCountIncrease);
                }
                break;

            case GameEventType.SpawnNewEnemyType:
                if (enemySpawner != null && gameEvent.enemyPrefabToSpawn != null)
                {
                    enemySpawner.AddEnemyType(
                        gameEvent.enemyPrefabToSpawn,
                        gameEvent.enemyHealthMultiplier,
                        gameEvent.enemyDamageMultiplier,
                        gameEvent.enemySpeedMultiplier
                    );
                }
                break;

            case GameEventType.BossEvent:
                if (enemySpawner != null && gameEvent.enemyPrefabToSpawn != null)
                {
                    enemySpawner.SpawnBoss(gameEvent.enemyPrefabToSpawn);
                }
                break;
        }

        OnGameEventTriggered?.Invoke(gameEvent);
    }

    public void StartGame()
    {
        gameTime = initialTime;
        gameStarted = true;
        isPaused = false;
        Time.timeScale = 1f;
        currentLevelIndex = 1;

        foreach (var gameEvent in gameEvents)
        {
            gameEvent.hasTriggered = false;
        }
    }

    public void AdvanceToNextLevel()
    {
        currentLevelIndex++;
        // Reset degli eventi per il nuovo livello
        foreach (var gameEvent in gameEvents)
        {
            if (gameEvent.levelNumber < currentLevelIndex)
            {
                gameEvent.hasTriggered = true; // Marca come triggerati gli eventi dei livelli precedenti
            }
        }
    }

    public void GameOver()
    {
        gameStarted = false;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameOver();
        }
    }

    public void GameComplete()
    {
        gameStarted = false;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameComplete();
        }
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        if (!gameStarted) return;

        isPaused = !isPaused;

        if (UIManager.Instance != null)
        {
            if (isPaused)
                UIManager.Instance.ShowPauseMenu();
            else
                UIManager.Instance.HidePauseMenu();
        }
    }

    public void PauseGame()
    {
        gameStarted = false;
        isPaused = true;
    }

    public void ResumeGame()
    {
        gameStarted = true;
        isPaused = false;
    }

    public void AddTime(float additionalTime)
    {
        float maxTime = 600f; // 10 minuti
        gameTime = Mathf.Min(gameTime + additionalTime, maxTime);
    }

    public void SetGameTime(float newTime) => gameTime = newTime;

    // Getter per l'editor e altri script
    public float GetGameTime() => gameTime;
    public bool IsGameStarted() => gameStarted;
    public List<GameEvent> GetGameEvents() => gameEvents;
    public int GetCurrentLevel() => currentLevelIndex;
}