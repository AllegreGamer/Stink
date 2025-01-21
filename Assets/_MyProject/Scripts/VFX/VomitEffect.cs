using UnityEngine;
using System.Collections;

public class VomitEffect : MonoBehaviour
{
    [Header("Color Settings")]
    [SerializeField] private Color vomitColor = new Color(0.827f, 0.745f, 0f, 0.7f); // Il colore del vomito
    [SerializeField] private float colorFadeDuration = 3f;  // Durata totale dell'effetto

    private Renderer[] renderers;
    private Material[] originalMaterials;
    private Material[] vomitMaterials;
    private bool isVomited = false;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        // Ottieni tutti i renderer (sia Mesh che Skinned)
        renderers = GetComponentsInChildren<Renderer>(true);

        // Salva i materiali originali e crea copie per l'effetto vomito
        if (renderers.Length > 0)
        {
            originalMaterials = new Material[renderers.Length];
            vomitMaterials = new Material[renderers.Length];

            for (int i = 0; i < renderers.Length; i++)
            {
                originalMaterials[i] = renderers[i].material;
                vomitMaterials[i] = new Material(originalMaterials[i]);
                vomitMaterials[i].color = vomitColor;
            }

            Debug.Log($"VomitEffect initialized with {renderers.Length} renderers");
        }
        else
        {
            Debug.LogWarning("No renderers found in children!");
        }
    }

    public void ApplyVomitEffect()
    {
        if (renderers == null || renderers.Length == 0)
        {
            Debug.LogWarning("No renderers to apply vomit effect to!");
            return;
        }

        if (!isVomited)
        {
            isVomited = true;

            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            // Applica il materiale vomito
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null)
                {
                    renderers[i].material = vomitMaterials[i];
                }
            }

            fadeCoroutine = StartCoroutine(FadeOutVomit());
            Debug.Log("Vomit effect applied");
        }
        else
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            fadeCoroutine = StartCoroutine(FadeOutVomit());
            Debug.Log("Vomit effect reset");
        }
    }

    private IEnumerator FadeOutVomit()
    {
        float elapsedTime = 0f;
        Color startColor = vomitColor;

        while (elapsedTime < colorFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / colorFadeDuration;

            Color currentColor = Color.Lerp(startColor, Color.white, normalizedTime);
            foreach (Material mat in vomitMaterials)
            {
                if (mat != null)
                {
                    mat.color = currentColor;
                }
            }

            yield return null;
        }

        // Ripristina i materiali originali
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
            {
                renderers[i].material = originalMaterials[i];
            }
        }

        isVomited = false;
        fadeCoroutine = null;
        Debug.Log("Vomit effect faded out");
    }

    private void OnDestroy()
    {
        if (vomitMaterials != null)
        {
            foreach (Material mat in vomitMaterials)
            {
                if (mat != null)
                {
                    Destroy(mat);
                }
            }
        }
    }
}