using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    private const int MENU_SCENE_INDEX = 0;
    private const int GAME_SCENE_INDEX = 1;
    private const string TOTAL_FRAGMENTS_KEY = "TotalGrudgeFragments";

    [Header("UI Panels")]
    [SerializeField] private GameObject buttonPanel;
    [SerializeField] private GameObject instructionsPanel;
    [SerializeField] private GameObject shopPanel;

    [Header("UI Elements")]
    [SerializeField] private Button firstSelectedButton;
    [SerializeField] private Button firstSelectedInstructions;
    [SerializeField] private Button firstSelectedShop;
    [SerializeField] private TextMeshProUGUI totalFragmentsText;
    [SerializeField] private Button shopButton;

    private void Awake()
    {
        if (shopButton != null)
        {
            shopButton.gameObject.SetActive(true);
        }
    }

    private void Start()
    {
        InitializePanels();
        UpdateFragmentCounter();
        SetInitialSelection(firstSelectedButton);
    }

    private void InitializePanels()
    {
        buttonPanel?.SetActive(true);
        instructionsPanel?.SetActive(false);
        shopPanel?.SetActive(false);

        if (shopButton != null)
        {
            shopButton.gameObject.SetActive(true);
        }
    }

    private void OnEnable()
    {
        SetInitialSelection(firstSelectedButton);
        if (shopButton != null)
        {
            shopButton.gameObject.SetActive(true);
        }
    }

    private void UpdateFragmentCounter()
    {
        if (totalFragmentsText != null)
        {
            int total = PlayerPrefs.GetInt(TOTAL_FRAGMENTS_KEY, 0);
            totalFragmentsText.text = $"GrudgeFragments: {total}";
        }
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

    public void StartGame()
    {
        SceneManager.LoadScene(GAME_SCENE_INDEX);
    }

    public void ShowInstructions()
    {
        buttonPanel?.SetActive(false);
        instructionsPanel?.SetActive(true);
        SetInitialSelection(firstSelectedInstructions);
    }

    public void HideInstructions()
    {
        instructionsPanel?.SetActive(false);
        buttonPanel?.SetActive(true);
        SetInitialSelection(firstSelectedButton);
    }

    public void ShowShop()
    {
        buttonPanel?.SetActive(false);
        shopPanel?.SetActive(true);
        SetInitialSelection(firstSelectedShop);
        UpdateFragmentCounter();
    }

    public void HideShop()
    {
        shopPanel?.SetActive(false);
        buttonPanel?.SetActive(true);
        SetInitialSelection(firstSelectedButton);
    }
}