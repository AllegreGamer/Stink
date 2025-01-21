using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class SplashEffect : MonoBehaviour
{
    private Renderer splashRenderer;
    private Material originalMaterial;

    private void Awake()
    {
        splashRenderer = GetComponentInChildren<Renderer>();
        if (splashRenderer != null)
        {
            // Salva il materiale originale
            originalMaterial = splashRenderer.material;
        }
    }

    public void StartFade(float duration)
    {
        StartCoroutine(FadeOut(duration));
    }

    private IEnumerator FadeOut(float duration)
    {
        //Debug.Log("Starting fade process...");
        if (splashRenderer == null)
        {
            Debug.LogError("No renderer found!");
            yield break;
        }

        Color startColor = originalMaterial.GetColor("_BaseColor");
        float fadeOutTime = 1f;

        yield return new WaitForSeconds(duration - fadeOutTime);
        //Debug.Log("Starting fade out...");

        float elapsedTime = 0f;
        while (elapsedTime < fadeOutTime)
        {
            float alpha = Mathf.Lerp(startColor.a, 0f, elapsedTime / fadeOutTime);
            Color newColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
            originalMaterial.SetColor("_BaseColor", newColor);

            //Debug.Log($"Current alpha: {alpha}");
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //Debug.Log("Fade complete, destroying splash");
        Destroy(gameObject);
    }
}