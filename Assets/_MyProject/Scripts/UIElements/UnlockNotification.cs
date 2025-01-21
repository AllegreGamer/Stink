using UnityEngine;
using TMPro;
using System.Collections;

public class UnlockNotification : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Animation Settings")]
    [SerializeField] private float showDuration = 3f;
    [SerializeField] private float fadeDuration = 0.5f;

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        // Inizializza il CanvasGroup ma mantieni il GameObject attivo
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        else
        {
            Debug.LogError("CanvasGroup not found on UnlockNotification!");
        }
    }

    public void ShowUnlock(int level, string skillName)
    {
        Debug.Log($"ShowUnlock called for Level {level} - {skillName}");

        if (titleText == null || messageText == null)
        {
            Debug.LogError($"Missing text references! TitleText: {titleText != null}, MessageText: {messageText != null}");
            return;
        }

        titleText.text = $"Level {level}";
        messageText.text = $"New Skill Unlocked:\n{skillName}";

        // Ferma eventuali coroutine precedenti
        StopAllCoroutines();
        StartCoroutine(AnimateNotification());
    }

    private IEnumerator AnimateNotification()
    {
        Debug.Log("Starting notification animation");

        // Mostra il panel
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        // Fade in
        float elapsed = 0;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = elapsed / fadeDuration;
            yield return null;
        }
        canvasGroup.alpha = 1;

        // Wait
        yield return new WaitForSeconds(showDuration);

        // Fade out
        elapsed = 0;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = 1 - (elapsed / fadeDuration);
            yield return null;
        }

        // Nascondi completamente il panel
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        Debug.Log("Notification animation completed");
    }
}