using UnityEngine;
using TMPro;

public class MirrorUI : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private TextMeshPro counterText;
    [SerializeField] private TextMeshPro unlockText;
    [SerializeField] private float displayDistance = 5f;
    [SerializeField] private Vector3 textOffset = new Vector3(0, 0.5f, -0.1f);

    private Transform playerTransform;
    private MirrorTerminal mirror;

    private void Start()
    {
        mirror = GetComponent<MirrorTerminal>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        if (counterText != null)
            counterText.enabled = false;
        if (unlockText != null)
        {
            unlockText.enabled = false;
            unlockText.text = "Door Unlocked";
        }
    }

    private void Update()
    {
        if (playerTransform != null && counterText != null && mirror != null)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            bool shouldShow = distance <= displayDistance;

            counterText.enabled = shouldShow;
            if (unlockText != null)
                unlockText.enabled = shouldShow && mirror.IsDoorUnlocked();

            if (shouldShow)
            {
                counterText.text = $"{mirror.GetCurrentTears()}/{mirror.GetRequiredTears()}";
            }
        }
    }
}