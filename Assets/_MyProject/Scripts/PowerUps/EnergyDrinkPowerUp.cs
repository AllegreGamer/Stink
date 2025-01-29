using UnityEngine;

public class EnergyDrinkPowerUp : PowerUpItem
{
    [Header("Energy Drink Effect")]
    [SerializeField] private float damageMultiplier = 1.1f;  // +10% danno
    [SerializeField] private float duration = 20f;            // durata effetto

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PowerUpManager powerUpManager = other.GetComponent<PowerUpManager>();
            if (powerUpManager != null)
            {
                powerUpManager.ApplyEnergyDrink(damageMultiplier, duration);
                Destroy(gameObject);
            }
        }
    }
}