using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EnemyType
{
    public GameObject prefab;
    public float healthMultiplier = 1f;
    public float damageMultiplier = 1f;
    public float speedMultiplier = 1f;
}

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private float spawnRadius = 10f;
    [SerializeField] private float minDistanceFromPlayer = 5f;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private int maxEnemies = 10;
    [SerializeField] private Transform player;
    [SerializeField] private float spawnHeight = 1f; // Altezza aggiuntiva dal suolo

    [Header("Enemy Types")]
    [SerializeField] private List<EnemyType> enemyTypes = new List<EnemyType>();

    private float nextSpawnTime;
    private List<GameObject> activeEnemies = new List<GameObject>();

    private void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    private void Update()
    {
        // Rimuovi i nemici nulli dalla lista (distrutti)
        activeEnemies.RemoveAll(enemy => enemy == null);

        // Spawna nuovi nemici se necessario
        if (Time.time >= nextSpawnTime && activeEnemies.Count < maxEnemies && enemyTypes.Count > 0)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    private void SpawnEnemy()
    {
        Vector3 spawnPosition = GetValidSpawnPosition();
        if (spawnPosition != Vector3.zero)
        {
            // Seleziona un tipo di nemico casuale
            EnemyType enemyType = enemyTypes[Random.Range(0, enemyTypes.Count)];

            // Aggiungi l'altezza di spawn alla posizione
            spawnPosition.y += spawnHeight;

            GameObject enemy = Instantiate(enemyType.prefab, spawnPosition, Quaternion.identity);
            activeEnemies.Add(enemy);

            // Applica i modificatori
            BaseEnemy baseEnemy = enemy.GetComponent<BaseEnemy>();
            if (baseEnemy != null)
            {
                baseEnemy.SetMultipliers(
                    enemyType.healthMultiplier,
                    enemyType.damageMultiplier,
                    enemyType.speedMultiplier
                );
            }
        }
    }

    private Vector3 GetValidSpawnPosition()
    {
        for (int i = 0; i < 30; i++) // Numero massimo di tentativi
        {
            Vector2 randomCircle = Random.insideUnitCircle.normalized * spawnRadius;
            Vector3 potentialPosition = player.position + new Vector3(randomCircle.x, 0, randomCircle.y);

            if (Vector3.Distance(potentialPosition, player.position) >= minDistanceFromPlayer)
            {
                // Fai un raycast verso il basso per trovare il terreno
                if (Physics.Raycast(potentialPosition + Vector3.up * 10, Vector3.down, out RaycastHit hit, 20f))
                {
                    return hit.point;
                }
            }
        }

        return Vector3.zero;
    }

    public void IncreaseMaxEnemies(int amount)
    {
        maxEnemies += amount;
        Debug.Log($"Increased max enemies to {maxEnemies}");
    }

    public void AddEnemyType(GameObject enemyPrefab, float healthMult = 1f, float damageMult = 1f, float speedMult = 1f)
    {
        EnemyType newType = new EnemyType
        {
            prefab = enemyPrefab,
            healthMultiplier = healthMult,
            damageMultiplier = damageMult,
            speedMultiplier = speedMult
        };

        enemyTypes.Add(newType);
        Debug.Log($"Added new enemy type with health mult: {healthMult}, damage mult: {damageMult}, speed mult: {speedMult}");
    }

    public void SpawnBoss(GameObject bossPrefab)
    {
        Vector3 spawnPosition = GetValidSpawnPosition();
        if (spawnPosition != Vector3.zero)
        {
            spawnPosition.y += spawnHeight;
            GameObject boss = Instantiate(bossPrefab, spawnPosition, Quaternion.identity);
            activeEnemies.Add(boss);
            Debug.Log("Boss spawned!");
        }
    }

    public void ClearAllEnemies()
    {
        foreach (var enemy in activeEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        activeEnemies.Clear();
    }

    public int GetActiveEnemiesCount()
    {
        return activeEnemies.Count;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            // Disegna il raggio di spawn
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(player.position, spawnRadius);

            // Disegna la distanza minima dal player
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.position, minDistanceFromPlayer);
        }
    }
#endif
}