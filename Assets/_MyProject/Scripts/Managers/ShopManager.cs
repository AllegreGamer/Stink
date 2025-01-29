using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    [Header("Database Reference")]
    [SerializeField] private PowerUpDatabase powerUpDatabase;

    [Header("UI References")]
    [SerializeField] private Transform abilitiesGrid;
    [SerializeField] private GameObject abilityPrefab;
    [SerializeField] private TextMeshProUGUI fragmentsText;
    [SerializeField] private GameObject detailsPanel;

    [Header("Slot References")]
    [SerializeField] private Transform[] abilitySlots;
    [SerializeField] private Color unlockedSlotColor = Color.white;
    [SerializeField] private Color lockedSlotColor = Color.gray;
    [SerializeField] private int baseSlotCount = 0;
    [SerializeField] private int maxSlotCount = 5;
    [SerializeField] private int slotUnlockCost = 100;

    [Header("Details Panel References")]
    [SerializeField] private Image selectedAbilityIcon;
    [SerializeField] private TextMeshProUGUI abilityNameText;
    [SerializeField] private TextMeshProUGUI abilityDescriptionText;
    [SerializeField] private TextMeshProUGUI abilityCostText;
    [SerializeField] private Button buyEquipButton;
    [SerializeField] private TextMeshProUGUI buttonText;

    [Header("PopUp Message")]
    [SerializeField] private GameObject popUpPanel;
    [SerializeField] private TextMeshProUGUI popUpText;
    [SerializeField] private float popUpDuration = 2f;

    // Chiavi per PlayerPrefs
    private string GetUnlockKey(PowerUpId id) => $"PowerUp_{id}_Unlocked";
    private string GetEquipKey(PowerUpId id) => $"PowerUp_{id}_Equipped";

    // Stato corrente
    private PowerUpId selectedPowerUpId;
    private int availableSlots;
    private int equippedCount;
    private Dictionary<PowerUpId, GameObject> abilityButtons = new Dictionary<PowerUpId, GameObject>();
    private Coroutine currentPopUpCoroutine;

    private void Start()
    {
        LoadSavedState();
        InitializeShop();
        UpdateUI();
        detailsPanel.SetActive(false);
        popUpPanel.SetActive(false);
    }

    private void LoadSavedState()
    {
        // Carica il numero di slot disponibili
        availableSlots = PlayerPrefs.GetInt("AvailableAbilitySlots", baseSlotCount);
        equippedCount = 0;

        // Carica lo stato di ogni power-up
        foreach (var config in powerUpDatabase.powerUps)
        {
            // Aggiorna il conteggio delle abilità equipaggiate
            if (IsPowerUpEquipped(config.id))
            {
                equippedCount++;
            }
        }
    }

    private void InitializeShop()
    {
        // Pulisce la griglia delle abilità
        foreach (Transform child in abilitiesGrid)
        {
            Destroy(child.gameObject);
        }

        // Crea i bottoni per ogni power-up nel database
        foreach (var config in powerUpDatabase.powerUps)
        {
            GameObject abilityButton = Instantiate(abilityPrefab, abilitiesGrid);
            SetupAbilityButton(abilityButton, config);
            abilityButtons[config.id] = abilityButton;
        }

        UpdateSlotVisuals();
    }

    private void SetupAbilityButton(GameObject buttonObj, PowerUpConfig config)
    {
        // Setup dell'icona
        Image icon = buttonObj.transform.Find("Icon").GetComponent<Image>();
        icon.sprite = config.icon;

        // Setup del nome
        TextMeshProUGUI nameText = buttonObj.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        nameText.text = config.name;

        // Setup del costo
        TextMeshProUGUI costText = buttonObj.transform.Find("Cost").GetComponent<TextMeshProUGUI>();
        costText.text = $"{config.cost} fragments";

        // Setup del bottone principale
        Button mainButton = buttonObj.GetComponent<Button>();
        mainButton.onClick.RemoveAllListeners();
        mainButton.onClick.AddListener(() => {
            SelectPowerUp(config.id);
            detailsPanel.SetActive(true);
            if (EventSystem.current != null && buyEquipButton != null)
            {
                EventSystem.current.SetSelectedGameObject(buyEquipButton.gameObject);
            }
        });

        UpdateAbilityButtonState(buttonObj, config);
    }

    private void UpdateAbilityButtonState(GameObject buttonObj, PowerUpConfig config)
    {
        Button buyButton = buttonObj.transform.Find("BuyButton").GetComponent<Button>();
        TextMeshProUGUI buttonText = buyButton.GetComponentInChildren<TextMeshProUGUI>();

        bool isUnlocked = IsPowerUpUnlocked(config.id);
        bool isEquipped = IsPowerUpEquipped(config.id);

        if (isUnlocked)
        {
            buttonText.text = isEquipped ? "Unequip" : "Equip";
            buyButton.interactable = true;
        }
        else
        {
            buttonText.text = "Buy";
            buyButton.interactable = GetCurrentFragments() >= config.cost;
        }
    }

    private void SelectPowerUp(PowerUpId id)
    {
        selectedPowerUpId = id;
        UpdateDetailsPanel();
    }

    private void UpdateDetailsPanel()
    {
        var config = powerUpDatabase.GetPowerUpConfig(selectedPowerUpId);
        if (config == null) return;

        selectedAbilityIcon.sprite = config.icon;
        abilityNameText.text = config.name;
        abilityDescriptionText.text = config.description;
        abilityCostText.text = $"Cost: {config.cost} fragments";

        UpdateBuyEquipButton();
    }

    private void UpdateBuyEquipButton()
    {
        bool isUnlocked = IsPowerUpUnlocked(selectedPowerUpId);
        bool isEquipped = IsPowerUpEquipped(selectedPowerUpId);

        if (isUnlocked)
        {
            buttonText.text = isEquipped ? "Unequip" : "Equip";
            buyEquipButton.interactable = !isEquipped || (isEquipped && equippedCount > 0);
        }
        else
        {
            buttonText.text = "Buy";
            var config = powerUpDatabase.GetPowerUpConfig(selectedPowerUpId);
            buyEquipButton.interactable = config != null && GetCurrentFragments() >= config.cost;
        }
    }

    public void OnBuyEquipButton()
    {
        if (selectedPowerUpId == PowerUpId.None) return;

        if (IsPowerUpUnlocked(selectedPowerUpId))
        {
            ToggleEquipPowerUp();
        }
        else
        {
            TryBuyPowerUp();
        }
    }

    private void TryBuyPowerUp()
    {
        var config = powerUpDatabase.GetPowerUpConfig(selectedPowerUpId);
        if (config == null) return;

        int currentFragments = GetCurrentFragments();
        if (currentFragments >= config.cost)
        {
            SetFragments(currentFragments - config.cost);
            SetPowerUpUnlocked(selectedPowerUpId, true);
            UpdateUI();

            // Aggiorna il prefab del power-up con il nuovo stato
            UpdatePowerUpPrefabState(config);
        }
        else
        {
            ShowPopUpMessage("Not enough fragments!");
        }
    }

    private void ToggleEquipPowerUp()
    {
        bool isCurrentlyEquipped = IsPowerUpEquipped(selectedPowerUpId);

        if (isCurrentlyEquipped)
        {
            SetPowerUpEquipped(selectedPowerUpId, false);
            equippedCount--;
        }
        else if (equippedCount < availableSlots)
        {
            SetPowerUpEquipped(selectedPowerUpId, true);
            equippedCount++;
        }
        else
        {
            ShowPopUpMessage("No available slots!");
            return;
        }

        // Aggiorna il prefab del power-up con il nuovo stato
        var config = powerUpDatabase.GetPowerUpConfig(selectedPowerUpId);
        if (config != null)
        {
            UpdatePowerUpPrefabState(config);
        }

        UpdateUI();
    }

    private void UpdatePowerUpPrefabState(PowerUpConfig config)
    {
        if (config.prefab != null)
        {
            PowerUpItem powerUpItem = config.prefab.GetComponent<PowerUpItem>();
            if (powerUpItem != null)
            {
                powerUpItem.SetUnlocked(IsPowerUpUnlocked(config.id));
                powerUpItem.SetEquipped(IsPowerUpEquipped(config.id));
            }
        }
    }
    public void TryUnlockSlot()
    {
        int currentFragments = GetCurrentFragments();
        if (currentFragments >= slotUnlockCost && availableSlots < maxSlotCount)
        {
            SetFragments(currentFragments - slotUnlockCost);
            availableSlots++;
            PlayerPrefs.SetInt("AvailableAbilitySlots", availableSlots);
            UpdateUI();
        }
        else if (currentFragments < slotUnlockCost)
        {
            ShowPopUpMessage("Not enough fragments!");
        }
        else
        {
            ShowPopUpMessage("Maximum slots reached!");
        }
    }
    private void UpdateSlotVisuals()
    {
        for (int i = 0; i < abilitySlots.Length; i++)
        {
            // Aggiorna il bordo dello slot
            Image borderImage = abilitySlots[i].Find("Border").GetComponent<Image>();
            if (borderImage != null)
            {
                borderImage.color = i < availableSlots ? unlockedSlotColor : lockedSlotColor;
            }

            // Aggiorna l'icona dello slot
            Image iconImage = abilitySlots[i].Find("Icon")?.GetComponent<Image>();
            if (iconImage != null)
            {
                var equippedPowerUp = GetEquippedPowerUpForSlot(i);
                if (equippedPowerUp != null && i < availableSlots)
                {
                    iconImage.gameObject.SetActive(true);
                    iconImage.sprite = equippedPowerUp.icon;
                }
                else
                {
                    iconImage.gameObject.SetActive(false);
                }
            }
        }
    }

    private PowerUpConfig GetEquippedPowerUpForSlot(int slotIndex)
    {
        return powerUpDatabase.powerUps
            .Where(p => IsPowerUpEquipped(p.id) && !IsInEarlierSlot(p.id, slotIndex))
            .FirstOrDefault();
    }

    private bool IsInEarlierSlot(PowerUpId id, int slotIndex)
    {
        for (int i = 0; i < slotIndex; i++)
        {
            var powerUpInSlot = GetEquippedPowerUpForSlot(i);
            if (powerUpInSlot != null && powerUpInSlot.id == id)
            {
                return true;
            }
        }
        return false;
    }

    // Metodi Helper per PlayerPrefs
    private bool IsPowerUpUnlocked(PowerUpId id) => PlayerPrefs.GetInt(GetUnlockKey(id), 0) == 1;
    private bool IsPowerUpEquipped(PowerUpId id) => PlayerPrefs.GetInt(GetEquipKey(id), 0) == 1;
    private void SetPowerUpUnlocked(PowerUpId id, bool value) => PlayerPrefs.SetInt(GetUnlockKey(id), value ? 1 : 0);
    private void SetPowerUpEquipped(PowerUpId id, bool value) => PlayerPrefs.SetInt(GetEquipKey(id), value ? 1 : 0);

    private int GetCurrentFragments() => PlayerPrefs.GetInt("TotalGrudgeFragments", 0);
    private void SetFragments(int amount)
    {
        PlayerPrefs.SetInt("TotalGrudgeFragments", amount);
        PlayerPrefs.Save();
    }

    private void UpdateUI()
    {
        fragmentsText.text = $"GrudgeFragments: {GetCurrentFragments()}";
        UpdateSlotVisuals();

        foreach (var kvp in abilityButtons)
        {
            var config = powerUpDatabase.GetPowerUpConfig(kvp.Key);
            if (config != null)
            {
                UpdateAbilityButtonState(kvp.Value, config);
            }
        }

        if (selectedPowerUpId != PowerUpId.None)
        {
            UpdateDetailsPanel();
        }
    }

    private void ShowPopUpMessage(string message)
    {
        if (currentPopUpCoroutine != null)
            StopCoroutine(currentPopUpCoroutine);

        currentPopUpCoroutine = StartCoroutine(PopUpRoutine(message));
    }

    private IEnumerator PopUpRoutine(string message)
    {
        popUpText.text = message;
        popUpPanel.SetActive(true);
        yield return new WaitForSeconds(popUpDuration);
        popUpPanel.SetActive(false);
    }

    public void CloseDetailsPanel()
    {
        detailsPanel.SetActive(false);

        // Riseleziona l'ultimo bottone utilizzato nello shop panel
        if (EventSystem.current != null && abilityButtons.Count > 0)
        {
            // Se abbiamo un selectedPowerUpId, usa il suo bottone
            if (selectedPowerUpId != PowerUpId.None && abilityButtons.ContainsKey(selectedPowerUpId))
            {
                EventSystem.current.SetSelectedGameObject(abilityButtons[selectedPowerUpId]);
            }
            // Altrimenti seleziona il primo bottone disponibile
            else
            {
                EventSystem.current.SetSelectedGameObject(abilityButtons.Values.First());
            }
        }

        selectedPowerUpId = PowerUpId.None;
    }

    public void ReloadShop()
    {
        LoadSavedState();
        InitializeShop();
        UpdateUI();
    }
    public string[] GetAllAbilityKeys()
    {
        List<string> keys = new List<string>();

        // Aggiungi tutte le chiavi di unlock e equip per ogni power-up nel database
        foreach (var powerUp in powerUpDatabase.powerUps)
        {
            keys.Add(GetUnlockKey(powerUp.id));
            keys.Add(GetEquipKey(powerUp.id));
        }

        return keys.ToArray();
    }
}