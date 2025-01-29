using UnityEngine;
using System.Collections;
using GameCreator.Runtime.Characters;

public class PowerUpManager : MonoBehaviour
{
    private float currentDamageMultiplier = 1f;
    private float currentSpeedMultiplier = 1f;
    private Character character;
    private float originalSpeed;
    private PlayerHealth playerHealth;
    private float currentInnerArmorHealth = 0f;
    private bool hasInnerArmor = false;

    private void Awake()
    {
        // ... altro codice esistente ...
        playerHealth = GetComponent<PlayerHealth>();

        // Sottoscriviti all'evento dei danni
        if (playerHealth != null)
        {
            playerHealth.OnDamageTaken += HandleDamageTaken;
        }
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnDamageTaken -= HandleDamageTaken;
        }
    }

    public void ApplyInnerArmor(float healthMultiplier, float damageReduction, float armorHealth)
    {
        if (playerHealth == null) return;

        // Applica l'aumento di vita massima
        playerHealth.AddArmorModifier(damageReduction, -1f, "InnerArmor");  // -1f per durata infinita

        // Imposta la vita dell'armatura
        currentInnerArmorHealth = armorHealth;
        hasInnerArmor = true;
    }

    private void HandleDamageTaken(float damage)
    {
        if (!hasInnerArmor) return;

        currentInnerArmorHealth -= damage;

        if (currentInnerArmorHealth <= 0)
        {
            // Rimuovi gli effetti dell'armatura
            RemoveInnerArmor();
        }
    }

    private void RemoveInnerArmor()
    {
        if (playerHealth == null) return;

        // Rimuovi i modificatori
        playerHealth.RemoveArmorModifier("InnerArmor");
        hasInnerArmor = false;
        currentInnerArmorHealth = 0f;
    }

    public float GetDamageMultiplier() => currentDamageMultiplier;
    public float GetSpeedMultiplier() => currentSpeedMultiplier;

    public void ApplyEnergyDrink(float multiplier, float duration)
    {
        StopCoroutine(nameof(EnergyDrinkTimer));  // Stop only energy drink timer
        currentDamageMultiplier = multiplier;
        StartCoroutine(EnergyDrinkTimer(duration));
    }

    public void ApplySpeedBoost(float multiplier, float duration)
    {
        if (character == null) return;

        StopCoroutine(nameof(SpeedBoostTimer));  // Stop only speed boost timer

        // Apply speed boost
        character.Kernel.Motion.LinearSpeed = originalSpeed * multiplier;
        currentSpeedMultiplier = multiplier;

        // Start timer to reset speed
        StartCoroutine(SpeedBoostTimer(duration));
    }

    private IEnumerator EnergyDrinkTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        currentDamageMultiplier = 1f;
    }

    private IEnumerator SpeedBoostTimer(float duration)
    {
        yield return new WaitForSeconds(duration);

        if (character != null)
        {
            // Reset to original speed
            character.Kernel.Motion.LinearSpeed = originalSpeed;
            currentSpeedMultiplier = 1f;
        }
    }
}