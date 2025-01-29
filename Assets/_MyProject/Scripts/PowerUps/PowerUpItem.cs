using UnityEngine;
public class PowerUpItem : MonoBehaviour
{
    [Header("Power-Up Settings")]
    [SerializeField] private PowerUpId powerUpId;  // Cambiato da PowerUpType a PowerUpId

    // Stato runtime
    private bool isUnlocked = false;
    private bool isEquipped = false;

    // Getters
    public PowerUpId GetPowerUpId() => powerUpId;
    public bool IsUnlocked() => isUnlocked;
    public bool IsEquipped() => isEquipped;

    // Setters
    public void SetUnlocked(bool value) => isUnlocked = value;
    public void SetEquipped(bool value) => isEquipped = value;
}