using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;  // Per Button
using UnityEngine.EventSystems;  // Per EventSystem
using System.Collections;  // Per IEnumerator

public class MenuManager : MonoBehaviour
{
    // Definizione costanti per gli indici delle scene
    private const int MENU_SCENE_INDEX = 0;
    private const int GAME_SCENE_INDEX = 1;

    [SerializeField] private GameObject instructionsPanel;
    [SerializeField] private Button firstSelectedButton; // Il pulsante Start Game
    [SerializeField] private Button firstSelectedInstructions; // Il pulsante "Got It" nel pannello istruzioni

    private void Start()
    {
        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(false);
        }
        // Seleziona il primo pulsante all'avvio
        SetInitialSelection(firstSelectedButton);
    }

    private void OnEnable()
    {
        // Riseleziona il pulsante quando il menu diventa attivo
        SetInitialSelection(firstSelectedButton);
    }

    private void SetInitialSelection(Button button)
    {
        if (button == null || EventSystem.current == null) return;

        // Deseleziona qualsiasi elemento UI attualmente selezionato
        EventSystem.current.SetSelectedGameObject(null);
        // Breve delay per assicurarsi che la deseleziona sia completata
        StartCoroutine(SelectButtonNextFrame(button));
    }

    private IEnumerator SelectButtonNextFrame(Button button)
    {
        yield return new WaitForEndOfFrame();
        // Seleziona il pulsante specificato
        if (EventSystem.current != null && button != null)
        {
            EventSystem.current.SetSelectedGameObject(button.gameObject);
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(GAME_SCENE_INDEX);
    }

    public void ShowInstructions()
    {
        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(true);
            // Seleziona il primo pulsante nel pannello istruzioni
            SetInitialSelection(firstSelectedInstructions);
        }
    }

    public void HideInstructions()
    {
        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(false);
            // Ritorna al menu principale e seleziona il primo pulsante
            SetInitialSelection(firstSelectedButton);
        }
    }
}