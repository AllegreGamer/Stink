using UnityEngine;
using TMPro;

public class TearUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tearCountText;

    private void Update()
    {
        if (TearCollector.Instance != null && tearCountText != null)
        {
            tearCountText.text = $"Tears: {TearCollector.Instance.GetCurrentTears()}";
        }
    }
}