using UnityEngine;
using TMPro;

public class FragmentManager : MonoBehaviour
{
    public static FragmentManager Instance { get; private set; }
    private const string TOTAL_FRAGMENTS_KEY = "TotalGrudgeFragments";

    [SerializeField] private TextMeshProUGUI sessionCounterText;
    private int sessionFragments = 0;
    private int totalFragments;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadTotalFragments();
            UpdateUI();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddFragments(int amount)
    {
        sessionFragments += amount;
        totalFragments += amount;
        SaveTotalFragments();
        UpdateUI();
    }

    private void LoadTotalFragments()
    {
        totalFragments = PlayerPrefs.GetInt(TOTAL_FRAGMENTS_KEY, 0);
    }

    private void SaveTotalFragments()
    {
        PlayerPrefs.SetInt(TOTAL_FRAGMENTS_KEY, totalFragments);
        PlayerPrefs.Save();
    }

    private void UpdateUI()
    {
        if (sessionCounterText != null)
        {
            sessionCounterText.text = $"GrudgeFragments: {sessionFragments}";
        }
    }

    public int GetTotalFragments() => totalFragments;
}