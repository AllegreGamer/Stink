using UnityEngine;

public class DamageNumberManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject damageNumberPrefab;

    [Header("Accumulation Settings")]
    [SerializeField] private float accumulationTime = 0.1f;
    [SerializeField] private float spawnRadius = 0.5f;

    private float accumulatedDamage = 0;
    private float accumulationTimer = 0;
    private bool isAccumulating = false;
    private Vector3 damagePosition;

    public void AddDamage(float damage, Vector3 position)
    {
        if (!isAccumulating)
        {
            // Prima istanza di danno in questo ciclo
            isAccumulating = true;
            damagePosition = position;
            accumulationTimer = accumulationTime;
        }

        accumulatedDamage += damage;
    }

    private void Update()
    {
        if (isAccumulating)
        {
            accumulationTimer -= Time.deltaTime;
            if (accumulationTimer <= 0)
            {
                ShowAccumulatedDamage();
                ResetAccumulation();
            }
        }
    }

    private void ShowAccumulatedDamage()
    {
        if (damageNumberPrefab != null && accumulatedDamage > 0)
        {
            // Calcola una posizione random nel raggio
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = damagePosition + new Vector3(randomOffset.x, 0, randomOffset.y);

            GameObject damageNumberObj = Instantiate(damageNumberPrefab, spawnPosition, Quaternion.identity);
            DamageNumber damageNumber = damageNumberObj.GetComponent<DamageNumber>();
            if (damageNumber != null)
            {
                damageNumber.Setup(accumulatedDamage);
            }
        }
    }

    private void ResetAccumulation()
    {
        accumulatedDamage = 0;
        isAccumulating = false;
        accumulationTimer = 0;
    }
}