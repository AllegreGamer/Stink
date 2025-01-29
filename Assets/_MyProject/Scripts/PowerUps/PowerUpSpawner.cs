using UnityEngine;
using System.Collections.Generic;

public class PowerUpSpawner : MonoBehaviour
{
    [SerializeField] private PowerUpDatabase powerUpDatabase;
    [SerializeField] private float spawnRadius = 10f;
    [SerializeField] private float minDistanceFromPlayer = 5f;
    [SerializeField] private Transform player;

    private Dictionary<PowerUpId, float> nextSpawnTimes = new Dictionary<PowerUpId, float>();


    private void Start()
    {
        foreach (var powerUp in powerUpDatabase.powerUps)
        {
            nextSpawnTimes[powerUp.id] = Time.time + powerUp.spawnFrequency;
        }
    }

    private void Update()
    {
        foreach (var powerUp in powerUpDatabase.powerUps)
        {
            if (!IsPowerUpActive(powerUp.id)) continue;

            if (Time.time >= nextSpawnTimes[powerUp.id])
            {
                if (Random.value <= powerUp.spawnProbability)
                {
                    SpawnPowerUp(powerUp.prefab);  // Ora passiamo il prefab invece di PowerUpConfig
                }
                nextSpawnTimes[powerUp.id] = Time.time + powerUp.spawnFrequency;
            }
        }
    }
    private bool IsPowerUpActive(PowerUpId id)
    {
        return PlayerPrefs.GetInt($"PowerUp_{id}_Unlocked", 0) == 1 &&
               PlayerPrefs.GetInt($"PowerUp_{id}_Equipped", 0) == 1;
    }

    private void SpawnPowerUp(GameObject powerUpPrefab)
    {
        Vector3 spawnPosition = GetValidSpawnPosition();
        if (spawnPosition != Vector3.zero)
        {
            Instantiate(powerUpPrefab, spawnPosition, Quaternion.identity);
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
        if (player)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(player.position, spawnRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.position, minDistanceFromPlayer);
        }
    }
}