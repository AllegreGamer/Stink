using UnityEngine;

public class ShopResetter : MonoBehaviour
{
    private const string SLOT_KEY = "AvailableAbilitySlots";
    private const string FRAGMENTS_KEY = "TotalGrudgeFragments";

    [SerializeField] private ShopManager shopManager;
    [SerializeField] private int testFragmentsAmount = 1000;

    public void ResetShop()
    {
        if (shopManager == null)
        {
            Debug.LogError("ShopManager reference is missing!");
            return;
        }

        // Resetta gli slot disponibili
        PlayerPrefs.SetInt(SLOT_KEY, 0);

        // Resetta tutte le abilità
        foreach (string key in shopManager.GetAllAbilityKeys())
        {
            PlayerPrefs.SetInt(key, 0);
        }

        // Imposta una quantità di test di frammenti
        PlayerPrefs.SetInt(FRAGMENTS_KEY, testFragmentsAmount);

        // Salva le modifiche
        PlayerPrefs.Save();

        // Ricarica lo shop
        shopManager.ReloadShop();

        Debug.Log("Shop Reset Completed!");
    }

    [ContextMenu("Reset Shop")]
    private void ResetShopFromEditor()
    {
        ResetShop();
    }
}