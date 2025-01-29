using UnityEngine;

public class InnerArmorPowerUp : PowerUpItem
{
    [Header("Inner Armor Effect")]
    [SerializeField] private float healthBoostMultiplier = 1.3f;  // +30% vita massima
    [SerializeField] private float damageReductionPercent = 20f;  // -20% danni
    [SerializeField] private float armorHealth = 100f;           // L'armatura si rompe dopo 100 danni

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PowerUpManager powerUpManager = other.GetComponent<PowerUpManager>();
            if (powerUpManager != null)
            {
                powerUpManager.ApplyInnerArmor(healthBoostMultiplier, damageReductionPercent, armorHealth);
                Destroy(gameObject);
            }
        }
    }
}