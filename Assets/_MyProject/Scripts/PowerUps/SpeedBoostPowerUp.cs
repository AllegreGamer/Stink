using UnityEngine;
using GameCreator.Runtime.Characters;

public class SpeedBoostPowerUp : PowerUpItem
{
    [Header("Speed Boost Effect")]
    [SerializeField] private float speedMultiplier = 1.2f;  // +20% velocità
    [SerializeField] private float duration = 15f;          // durata effetto

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PowerUpManager powerUpManager = other.GetComponent<PowerUpManager>();
            if (powerUpManager != null)
            {
                powerUpManager.ApplySpeedBoost(speedMultiplier, duration);
                Destroy(gameObject);
            }
        }
    }
}