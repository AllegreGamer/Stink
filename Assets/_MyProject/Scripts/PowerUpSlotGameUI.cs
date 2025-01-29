using UnityEngine;
using UnityEngine.UI;
public class PowerUpSlotGameUI : MonoBehaviour
{
    [SerializeField] private PowerUpDatabase powerUpDatabase;
    [SerializeField] private Transform[] slots;

    private void Start()
    {
        UpdateSlots();
    }

    private void UpdateSlots()
    {
        int slotIndex = 0;
        foreach (var powerUp in powerUpDatabase.powerUps)
        {
            if (IsPowerUpActive(powerUp.id) && slotIndex < slots.Length)
            {
                Image iconImage = slots[slotIndex].Find("Icon")?.GetComponent<Image>();
                if (iconImage != null)
                {
                    iconImage.gameObject.SetActive(true);
                    iconImage.sprite = powerUp.icon;
                }
                slotIndex++;
            }
        }

        // Disattiva gli slot rimanenti
        for (int i = slotIndex; i < slots.Length; i++)
        {
            Image iconImage = slots[i].Find("Icon")?.GetComponent<Image>();
            if (iconImage != null)
            {
                iconImage.gameObject.SetActive(false);
            }
        }
    }

    private bool IsPowerUpActive(PowerUpId id)
    {
        return PlayerPrefs.GetInt($"PowerUp_{id}_Unlocked", 0) == 1 &&
               PlayerPrefs.GetInt($"PowerUp_{id}_Equipped", 0) == 1;
    }
}