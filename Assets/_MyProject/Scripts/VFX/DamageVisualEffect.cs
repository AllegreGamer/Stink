using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Enum per i tipi di attacco
public enum AttackType
{
    None,
    Spit,
    Vomit,
    Urine,
    Gas,
    Burp,
    Stink,
    Diarrhea,
    Shit
}

// Classe per configurare gli effetti nell'inspector
[System.Serializable]
public class DamageEffectConfig
{
    public AttackType attackType;
    public Color effectColor = Color.white;
    public float fadeDuration = 3f;
    public GameObject projectilePrefab; // Riferimento al prefab dell'attacco
}

public class DamageVisualEffect : MonoBehaviour
{
    [Header("Effect Configurations")]
    [SerializeField] private List<DamageEffectConfig> effectConfigs = new List<DamageEffectConfig>();

    [Header("Debug")]
    [SerializeField] private bool showDebugLog = false;

    private Renderer[] renderers;
    private Dictionary<AttackType, Material[]> effectMaterials = new Dictionary<AttackType, Material[]>();
    private Material[] originalMaterials;
    private Dictionary<AttackType, Coroutine> activeEffects = new Dictionary<AttackType, Coroutine>();
    private AttackType currentEffect = AttackType.None;

    private void Awake()
    {
        InitializeRenderers();
        InitializeEffectMaterials();
    }

    private void InitializeRenderers()
    {
        renderers = GetComponentsInChildren<Renderer>(true);
        if (renderers.Length > 0)
        {
            originalMaterials = new Material[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
            {
                originalMaterials[i] = renderers[i].material;
            }
        }
        else
        {
            Debug.LogWarning($"No renderers found in {gameObject.name}!");
        }
    }

    private void InitializeEffectMaterials()
    {
        foreach (var config in effectConfigs)
        {
            Material[] materials = new Material[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
            {
                materials[i] = new Material(originalMaterials[i]);
                materials[i].color = config.effectColor;
            }
            effectMaterials[config.attackType] = materials;
        }
    }

    public void ApplyEffect(AttackType attackType)
    {
        if (!effectMaterials.ContainsKey(attackType))
        {
            if (showDebugLog) Debug.LogWarning($"No effect configuration found for {attackType}");
            return;
        }

        // Se c'è già un effetto attivo dello stesso tipo, resetta solo il timer
        if (currentEffect == attackType && activeEffects.ContainsKey(attackType))
        {
            if (activeEffects[attackType] != null)
            {
                StopCoroutine(activeEffects[attackType]);
            }
            var config = effectConfigs.Find(c => c.attackType == attackType);
            activeEffects[attackType] = StartCoroutine(FadeOutEffect(attackType, config.fadeDuration));
            return;
        }

        // Applica il nuovo effetto
        currentEffect = attackType;
        ApplyEffectMaterials(attackType);

        // Avvia il fade out
        var effectConfig = effectConfigs.Find(c => c.attackType == attackType);
        if (effectConfig != null)
        {
            if (activeEffects.ContainsKey(attackType) && activeEffects[attackType] != null)
            {
                StopCoroutine(activeEffects[attackType]);
            }
            activeEffects[attackType] = StartCoroutine(FadeOutEffect(attackType, effectConfig.fadeDuration));
        }
    }

    private void ApplyEffectMaterials(AttackType attackType)
    {
        if (effectMaterials.TryGetValue(attackType, out Material[] materials))
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null)
                {
                    renderers[i].material = materials[i];
                }
            }
        }
    }

    private IEnumerator FadeOutEffect(AttackType attackType, float duration)
    {
        if (!effectMaterials.TryGetValue(attackType, out Material[] materials))
        {
            yield break;
        }

        var config = effectConfigs.Find(c => c.attackType == attackType);
        if (config == null) yield break;

        float elapsedTime = 0f;
        Color startColor = config.effectColor;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / duration;

            Color currentColor = Color.Lerp(startColor, Color.white, normalizedTime);
            foreach (Material mat in materials)
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

        currentEffect = AttackType.None;
        activeEffects.Remove(attackType);
    }

    private void OnDestroy()
    {
        foreach (var materials in effectMaterials.Values)
        {
            foreach (var mat in materials)
            {
                if (mat != null)
                {
                    Destroy(mat);
                }
            }
        }
    }

    // Metodo helper per aggiungere nuove configurazioni a runtime (se necessario)
    public void AddEffectConfig(AttackType type, Color color, float duration, GameObject prefab)
    {
        var config = new DamageEffectConfig
        {
            attackType = type,
            effectColor = color,
            fadeDuration = duration,
            projectilePrefab = prefab
        };
        effectConfigs.Add(config);
        InitializeEffectMaterials();
    }
}