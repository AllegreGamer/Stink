using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameCompletePanel;
    [SerializeField] private GameObject pausePanel;

    [Header("Buttons")]
    [SerializeField] private Button gameOverRestartButton;
    [SerializeField] private Button gameCompleteRestartButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button gameOverQuitButton;
    [SerializeField] private Button gameCompleteQuitButton;
    [SerializeField] private Button pauseQuitButton;

    [Header("First Selected Buttons")]
    [SerializeField] private Button firstSelectedGameOver;
    [SerializeField] private Button firstSelectedGameComplete;
    [SerializeField] private Button firstSelectedPause;

    private static UIManager instance;
    public static UIManager Instance => instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        InitializePanels();
    }

    private void InitializePanels()
    {
        gameOverPanel?.SetActive(false);
        gameCompletePanel?.SetActive(false);
        pausePanel?.SetActive(false);
    }

    private void Start()
    {
        SetupButtons();
    }

    private void SetupButtons()
    {
        if (gameOverRestartButton != null)
            gameOverRestartButton.onClick.AddListener(OnRestartClick);
        if (gameCompleteRestartButton != null)
            gameCompleteRestartButton.onClick.AddListener(OnRestartClick);
        if (resumeButton != null)
            resumeButton.onClick.AddListener(OnResumeClick);

        if (gameOverQuitButton != null)
            gameOverQuitButton.onClick.AddListener(OnQuitClick);
        if (gameCompleteQuitButton != null)
            gameCompleteQuitButton.onClick.AddListener(OnQuitClick);
        if (pauseQuitButton != null)
            pauseQuitButton.onClick.AddListener(OnQuitClick);
    }

    private void SetInitialSelection(Button button)
    {
        if (button == null || EventSystem.current == null) return;
        EventSystem.current.SetSelectedGameObject(null);
        StartCoroutine(SelectButtonNextFrame(button));
    }

    private IEnumerator SelectButtonNextFrame(Button button)
    {
        yield return new WaitForEndOfFrame();
        if (EventSystem.current != null && button != null)
        {
            EventSystem.current.SetSelectedGameObject(button.gameObject);
        }
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
        SetInitialSelection(firstSelectedGameOver);
    }

    public void ShowGameComplete()
    {
        gameCompletePanel.SetActive(true);
        Time.timeScale = 0f;
        SetInitialSelection(firstSelectedGameComplete);
    }

    public void ShowPauseMenu()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        SetInitialSelection(firstSelectedPause);
    }

    public void HidePauseMenu()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void OnRestartClick()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnResumeClick()
    {
        HidePauseMenu();
    }

    private void OnQuitClick()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0); // Torna al menu principale
    }
}