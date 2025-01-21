using UnityEngine;
using System.Collections;

public class MirrorTerminal : MonoBehaviour
{
    [Header("Mirror Settings")]
    [SerializeField] private float absorptionRadius = 5f;
    [SerializeField] private int requiredTears = 50;
    [SerializeField] private GameObject doorToOpen;
    [SerializeField] private ParticleSystem absorptionEffect;
    [SerializeField] private AudioSource absorptionSound;

    [Header("Visual Feedback")]
    [SerializeField] private Material mirrorMaterial;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color activeColor = Color.cyan;
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float pulseIntensity = 0.5f;

    private int currentTears = 0;
    private bool isActive = true;
    private bool isDoorUnlocked = false;
    private MaterialPropertyBlock propBlock;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    private void Start()
    {
        propBlock = new MaterialPropertyBlock();
        if (mirrorMaterial != null)
        {
            mirrorMaterial.EnableKeyword("_EMISSION");
        }
    }

    private void Update()
    {
        if (!isActive) return;

        // Cerca il player nelle vicinanze
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, absorptionRadius);
        foreach (Collider col in nearbyColliders)
        {
            if (col.CompareTag("Player"))
            {
                TearCollector tearCollector = col.GetComponent<TearCollector>();
                if (tearCollector != null && !isDoorUnlocked)
                {
                    AbsorbTearsFromPlayer(tearCollector);
                }
            }
        }

        UpdateVisualFeedback();
    }

    private void AbsorbTearsFromPlayer(TearCollector tearCollector)
    {
        int playerTears = tearCollector.GetCurrentTears();
        if (playerTears > 0)
        {
            // Calcola quante lacrime possiamo ancora assorbire
            int tearsNeeded = requiredTears - currentTears;
            int tearsToAbsorb = Mathf.Min(playerTears, tearsNeeded);

            if (tearsToAbsorb > 0)
            {
                // Assorbi le lacrime
                currentTears += tearsToAbsorb;
                tearCollector.RemoveTears(tearsToAbsorb);

                // Effetti
                if (absorptionEffect != null)
                    absorptionEffect.Play();
                if (absorptionSound != null)
                    absorptionSound.Play();

                // Controlla se abbiamo raggiunto il numero richiesto
                if (currentTears >= requiredTears && !isDoorUnlocked)
                {
                    OpenDoor();
                }
            }
        }
    }

    private void OpenDoor()
    {
        isDoorUnlocked = true;
        if (doorToOpen != null)
        {
            StartCoroutine(OpenDoorWithEffect());
        }
    }

    private IEnumerator OpenDoorWithEffect()
    {
        if (doorToOpen.TryGetComponent<Renderer>(out var renderer))
        {
            Material material = renderer.material;
            Color startColor = material.color;
            float elapsedTime = 0f;
            float duration = 1f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
                material.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
                yield return null;
            }
        }

        doorToOpen.SetActive(false);
    }

    private void UpdateVisualFeedback()
    {
        if (mirrorMaterial != null)
        {
            float pulse = (Mathf.Sin(Time.time * pulseSpeed) * 0.5f + 0.5f) * pulseIntensity;
            Color currentColor = Color.Lerp(normalColor, activeColor, (float)currentTears / requiredTears);
            currentColor *= 1f + pulse;
            mirrorMaterial.SetColor(EmissionColor, currentColor);
        }
    }

    public int GetCurrentTears() => currentTears;
    public int GetRequiredTears() => requiredTears;
    public bool IsDoorUnlocked() => isDoorUnlocked;
}