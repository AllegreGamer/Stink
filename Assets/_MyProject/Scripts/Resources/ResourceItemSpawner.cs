using UnityEngine;
using System.Collections.Generic;

public class ResourceItemSpawner : MonoBehaviour
{
    [System.Serializable]
    public class ItemSpawnData
    {
        public ResourceRecoveryItem itemPrefab;
        public float nextSpawnTime;
        public bool isUnlocked;
        public ResourceItemType itemType;
    }

    [Header("Spawn Settings")]
    [SerializeField] private float spawnRadius = 10f;
    [SerializeField] private float minDistanceFromPlayer = 5f;
    [SerializeField] private Transform player;

    [Header("Items")]
    [SerializeField] private List<ItemSpawnData> itemsData = new List<ItemSpawnData>();

    private PlayerStats playerStats;
    private ResourceManager resourceManager;

    private void Start()
    {
        playerStats = player.GetComponent<PlayerStats>();
        resourceManager = player.GetComponent<ResourceManager>();

        if (playerStats == null || resourceManager == null)
        {
            Debug.LogError("Required components missing!");
            return;
        }

        InitializeSpawnTimes();
    }

    private void Update()
    {
        UpdateItemUnlockStatus();
        HandleItemSpawning();
    }

    private void UpdateItemUnlockStatus()
    {
        if (playerStats == null) return;

        foreach (var itemData in itemsData)
        {
            // Controlla se il livello del player è sufficiente per sbloccare l'item
            itemData.isUnlocked = playerStats.GetLevel() >= itemData.itemPrefab.GetUnlockLevel();
        }
    }

    private void InitializeSpawnTimes()
    {
        foreach (var itemData in itemsData)
        {
            itemData.nextSpawnTime = Time.time + itemData.itemPrefab.GetSpawnFrequency();
        }
    }

    private void HandleItemSpawning()
    {
        foreach (var itemData in itemsData)
        {
            if (!itemData.isUnlocked) continue;

            if (Time.time >= itemData.nextSpawnTime)
            {
                if (Random.value <= itemData.itemPrefab.GetSpawnProbability())
                {
                    SpawnItem(itemData.itemPrefab.GetItemPrefab());
                }
                itemData.nextSpawnTime = Time.time + itemData.itemPrefab.GetSpawnFrequency();
            }
        }
    }

    private void SpawnItem(GameObject itemPrefab)
    {
        Vector3 spawnPosition = GetValidSpawnPosition();
        if (spawnPosition != Vector3.zero)
        {
            Instantiate(itemPrefab, spawnPosition, Quaternion.identity);
        }
    }

    private Vector3 GetValidSpawnPosition()
    {
        for (int i = 0; i < 30; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle.normalized * spawnRadius;
            Vector3 potentialPosition = player.position + new Vector3(randomCircle.x, 0, randomCircle.y);

            if (Vector3.Distance(potentialPosition, player.position) >= minDistanceFromPlayer)
            {
                if (Physics.Raycast(potentialPosition + Vector3.up * 10, Vector3.down, out RaycastHit hit, 20f))
                {
                    return hit.point + Vector3.up * 0.5f;
                }
            }
        }
        return Vector3.zero;
    }

    private void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(player.position, spawnRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.position, minDistanceFromPlayer);
        }
    }
}