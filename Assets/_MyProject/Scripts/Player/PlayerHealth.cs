using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("Defense Settings")]
    [SerializeField] private float baseArmor = 0f;
    private float currentArmor;
    private List<ArmorModifier> armorModifiers = new List<ArmorModifier>();

    [Header("Regeneration")]
    [SerializeField] private float regenRate = 0f;
    [SerializeField] private float regenTickInterval = 1f;
    private float nextRegenTick;

    [Header("Invulnerability")]
    [SerializeField] private float invulnerabilityDuration = 0.5f;
    [SerializeField] private bool enableInvulnerabilityOnDamage = false;
    private bool isInvulnerable = false;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    // Status effects dictionary
    private Dictionary<StatusEffectType, (float duration, float power)> activeEffects =
        new Dictionary<StatusEffectType, (float duration, float power)>();

    // Struttura per i modificatori dell'armatura
    public struct ArmorModifier
    {
        public float value;
        public float duration;
        public string source;

        public ArmorModifier(float value, float duration, string source)
        {
            this.value = value;
            this.duration = duration;
            this.source = source;
        }
    }

    private void Start()
    {
        currentHealth = maxHealth;
        currentArmor = baseArmor;
    }

    private void Update()
    {
        UpdateArmorModifiers();
        UpdateStatusEffects();
        HandleRegeneration();
    }

    #region Damage and Healing
    public void TakeDamage(float damage, string source = "")
    {
        Debug.Log($"TakeDamage called with damage: {damage}"); // Aggiungi questo
        if (isInvulnerable) return;

        // Calcola il danno effettivo considerando l'armatura
        float damageReduction = CalculateDamageReduction();
        float actualDamage = damage * (1 - damageReduction);

        currentHealth = Mathf.Max(0, currentHealth - actualDamage);

        if (showDebugLogs)
        {
            Debug.Log($"Player took {actualDamage:F1} damage from {source}. Health: {currentHealth:F1}/{maxHealth}");
        }

        // Invoca l'evento di danno
        OnDamageTaken?.Invoke(actualDamage);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        // Gestisce l'invulnerabilità temporanea se abilitata
        if (enableInvulnerabilityOnDamage)
        {
            StartCoroutine(TemporaryInvulnerability());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount, string source = "")
    {
        if (currentHealth >= maxHealth) return;

        float actualHeal = Mathf.Min(amount, maxHealth - currentHealth);
        currentHealth = Mathf.Min(maxHealth, currentHealth + actualHeal);

        if (showDebugLogs)
        {
            Debug.Log($"Player healed for {actualHeal:F1} from {source}. Health: {currentHealth:F1}/{maxHealth}");
        }

        OnHealed?.Invoke(actualHeal);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    #endregion

    #region Armor System
    public void AddArmorModifier(float value, float duration, string source)
    {
        armorModifiers.Add(new ArmorModifier(value, duration, source));
        RecalculateArmor();

        if (showDebugLogs)
        {
            Debug.Log($"Added armor modifier: {value} from {source} for {duration} seconds");
        }
    }

    private void UpdateArmorModifiers()
    {
        for (int i = armorModifiers.Count - 1; i >= 0; i--)
        {
            var mod = armorModifiers[i];
            mod.duration -= Time.deltaTime;

            if (mod.duration <= 0)
            {
                armorModifiers.RemoveAt(i);
                if (showDebugLogs)
                {
                    Debug.Log($"Armor modifier from {mod.source} expired");
                }
            }
        }
        RecalculateArmor();
    }

    private void RecalculateArmor()
    {
        currentArmor = baseArmor;
        foreach (var mod in armorModifiers)
        {
            currentArmor += mod.value;
        }
    }

    private float CalculateDamageReduction()
    {
        // Formula: riduzione danno percentuale = armatura / (armatura + 100)
        return currentArmor / (currentArmor + 100f);
    }
    #endregion

    #region Regeneration System
    public void SetRegenerationRate(float rate, float duration = -1f)
    {
        regenRate = rate;
        if (duration > 0)
        {
            StartCoroutine(ResetRegenAfterDuration(duration));
        }
    }

    private void HandleRegeneration()
    {
        if (regenRate <= 0 || Time.time < nextRegenTick) return;

        Heal(regenRate, "Regeneration");
        nextRegenTick = Time.time + regenTickInterval;
    }

    private IEnumerator ResetRegenAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        regenRate = 0;
    }
    #endregion

    #region Status Effects
    public void ApplyStatusEffect(StatusEffectType type, float duration, float power)
    {
        if (activeEffects.ContainsKey(type))
        {
            var currentEffect = activeEffects[type];
            activeEffects[type] = (
                Mathf.Max(currentEffect.duration, duration),
                Mathf.Max(currentEffect.power, power)
            );
        }
        else
        {
            activeEffects.Add(type, (duration, power));
        }

        if (showDebugLogs)
        {
            Debug.Log($"Applied status effect: {type} with power {power} for {duration} seconds");
        }
    }

    private void UpdateStatusEffects()
    {
        List<StatusEffectType> effectsToRemove = new List<StatusEffectType>();

        foreach (var effect in activeEffects)
        {
            var type = effect.Key;
            var duration = effect.Value.duration - Time.deltaTime;
            var power = effect.Value.power;

            if (duration <= 0)
            {
                effectsToRemove.Add(type);
                continue;
            }

            // Applica gli effetti attivi
            ApplyStatusEffectImpact(type, power);
            activeEffects[type] = (duration, power);
        }

        foreach (var effect in effectsToRemove)
        {
            activeEffects.Remove(effect);
        }
    }

    private void ApplyStatusEffectImpact(StatusEffectType type, float power)
    {
        // Implementa gli effetti specifici qui
        switch (type)
        {
            case StatusEffectType.Poison:
                TakeDamage(power * Time.deltaTime, "Poison");
                break;
                // Aggiungi altri effetti come necessario
        }
    }
    #endregion

    #region Invulnerability
    private IEnumerator TemporaryInvulnerability()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityDuration);
        isInvulnerable = false;
    }
    #endregion

    #region Events
    // Eventi per UI e feedback
    public event System.Action<float, float> OnHealthChanged; // currentHealth, maxHealth
    public event System.Action<float> OnDamageTaken;
    public event System.Action<float> OnHealed;
    public event System.Action OnDeath;
    #endregion

    private void Die()
    {
        if (showDebugLogs)
        {
            Debug.Log("Player died!");
        }

        OnDeath?.Invoke();
        // Implementa qui la logica di morte
    }

    #region Getters
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public float GetHealthPercentage() => currentHealth / maxHealth;
    public float GetCurrentArmor() => currentArmor;
    public Dictionary<StatusEffectType, (float duration, float power)> GetActiveEffects()
    {
        return new Dictionary<StatusEffectType, (float duration, float power)>(activeEffects);
    }
    #endregion
}