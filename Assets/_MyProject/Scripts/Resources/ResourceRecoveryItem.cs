using UnityEngine;

[System.Serializable]
public class ResourceRecoveryAmount
{
    public float healthRecovery = 0f;
    public float urineRecovery = 0f;
    public float foodRecovery = 0f;
    public float alcoholRecovery = 0f;
    public float burpRecovery = 0f;
}

public enum ResourceItemType
{
    ToiletPaper,    // Sempre disponibile
    WaterBottle,    // Richiede waterSkill (Livello 3)
    Ration,         // Richiede foodSkill (Livello 5)
    BeerCan,        // Richiede beerSkill (Livello 8)
    SoftDrink,      // Richiede beerSkill (Livello 8)
    WineBottle      // Richiede alcoholSkill (Livello 6)
}

public class ResourceRecoveryItem : MonoBehaviour
{
    [Header("Item Settings")]
    [SerializeField] private ResourceItemType itemType;
    [SerializeField] private int unlockLevel = 1;
    [SerializeField] private ResourceRecoveryAmount recoveryAmounts;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnProbability = 0.5f;
    [SerializeField] private float spawnFrequency = 30f; // Seconds
    [SerializeField] private GameObject itemPrefab;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ApplyRecoveryEffects(other.gameObject);
            Destroy(gameObject);
        }
    }

    private void ApplyRecoveryEffects(GameObject player)
    {
        ResourceManager resourceManager = player.GetComponent<ResourceManager>();
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

        if (resourceManager != null)
        {
            if (recoveryAmounts.urineRecovery > 0)
                resourceManager.AddUrine(recoveryAmounts.urineRecovery);

            if (recoveryAmounts.foodRecovery > 0)
                resourceManager.AddFood(recoveryAmounts.foodRecovery);

            if (recoveryAmounts.alcoholRecovery > 0)
                resourceManager.AddAlcohol(recoveryAmounts.alcoholRecovery);

            if (recoveryAmounts.burpRecovery > 0)
                resourceManager.AddBurp(recoveryAmounts.burpRecovery);
        }

        if (playerHealth != null && recoveryAmounts.healthRecovery > 0)
        {
            playerHealth.Heal(recoveryAmounts.healthRecovery);
        }
    }

    public ResourceItemType GetItemType() => itemType;
    public int GetUnlockLevel() => unlockLevel;
    public float GetSpawnProbability() => spawnProbability;
    public float GetSpawnFrequency() => spawnFrequency;
    public GameObject GetItemPrefab() => itemPrefab;
}