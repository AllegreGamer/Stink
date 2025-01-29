using UnityEngine;
using System.Collections.Generic;

public class MemoryBoxSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject memoryBoxPrefab;
    [SerializeField] private float spawnInterval = 30f;
    [SerializeField] private float spawnProbability = 0.3f;
    [SerializeField] private float minDistanceFromPlayer = 10f;
    [SerializeField] private float maxSpawnRadius = 30f;

    [Header("Map Boundaries")]
    [SerializeField] private Vector2 mapSize = new Vector2(100f, 100f); // X e Z della mappa
    [SerializeField] private Vector3 mapCenter = Vector3.zero;

    [Header("Fragment Settings")]
    [SerializeField] private int minFragments = 1;
    [SerializeField] private int maxFragments = 6;
    [SerializeField] [Range(0.1f, 1f)] private float dropProbabilityFalloff = 0.7f;

    private float nextSpawnTime;
    private Transform playerTransform;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        nextSpawnTime = Time.time + spawnInterval;
    }

    private void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            if (Random.value < spawnProbability)
            {
                SpawnMemoryBox();
            }
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    private void SpawnMemoryBox()
    {
        Vector3 spawnPosition = GetValidSpawnPosition();
        if (spawnPosition != Vector3.zero)
        {
            GameObject box = Instantiate(memoryBoxPrefab, spawnPosition, Quaternion.identity);
            MemoryBox memoryBox = box.GetComponent<MemoryBox>();
            if (memoryBox != null)
            {
                memoryBox.Initialize(CalculateFragmentCount());
            }
        }
    }

    private Vector3 GetValidSpawnPosition()
    {
        for (int i = 0; i < 30; i++)
        {
            // Genera una posizione casuale all'interno dei limiti della mappa
            float randomX = Random.Range(-mapSize.x / 2, mapSize.x / 2) + mapCenter.x;
            float randomZ = Random.Range(-mapSize.y / 2, mapSize.y / 2) + mapCenter.z;
            Vector3 randomPosition = new Vector3(randomX, 0, randomZ);

            // Verifica la distanza dal player
            if (Vector3.Distance(randomPosition, playerTransform.position) < minDistanceFromPlayer)
            {
                continue;
            }

            // Raycast per trovare il terreno
            if (Physics.Raycast(randomPosition + Vector3.up * 10f, Vector3.down, out RaycastHit hit, 20f))
            {
                return hit.point + Vector3.up * 0.5f;
            }
        }
        return Vector3.zero;
    }

    private void OnDrawGizmosSelected()
    {
        // Visualizza i limiti della mappa nell'editor
        Gizmos.color = Color.yellow;
        Vector3 center = mapCenter;
        Vector3 size = new Vector3(mapSize.x, 1f, mapSize.y);
        Gizmos.DrawWireCube(center, size);

        if (Application.isPlaying && playerTransform != null)
        {
            // Visualizza il raggio minimo dal player
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(playerTransform.position, minDistanceFromPlayer);
        }
    }

    private int CalculateFragmentCount()
    {
        int fragments = minFragments;
        float currentProbability = 1f;

        for (int i = minFragments + 1; i <= maxFragments; i++)
        {
            currentProbability *= dropProbabilityFalloff;
            if (Random.value < currentProbability)
            {
                fragments = i;
            }
            else
            {
                break;
            }
        }

        return fragments;
    }
}