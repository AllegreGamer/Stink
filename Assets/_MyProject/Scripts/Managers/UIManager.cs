using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameCompletePanel;
    [SerializeField] private GameObject pausePanel;

    [Header("Buttons")]
    [SerializeField] private Button gameOverRestartButton;    // Restart nel pannello Game Over
    [SerializeField] private Button gameCompleteRestartButton; // Restart nel pannello Game Complete
    [SerializeField] private Button resumeButton;             // Resume nel pannello Pause

    private static UIManager instance;
    public static UIManager Instance => instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        // Nascondi tutti i pannelli all'inizio
        gameOverPanel.SetActive(false);
        gameCompletePanel.SetActive(false);
        pausePanel.SetActive(false);
    }

    private void Start()
    {
        // Setup dei pulsanti
        if (gameOverRestartButton != null)
            gameOverRestartButton.onClick.AddListener(OnRestartClick);

        if (gameCompleteRestartButton != null)
            gameCompleteRestartButton.onClick.AddListener(OnRestartClick);

        if (resumeButton != null)
            resumeButton.onClick.AddListener(OnResumeClick);
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ShowGameComplete()
    {
        gameCompletePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ShowPauseMenu()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void HidePauseMenu()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void OnRestartClick()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }

    private void OnResumeClick()
    {
        HidePauseMenu();
    }
}