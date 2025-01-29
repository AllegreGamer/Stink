using UnityEngine;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Characters;
using UnityEngine.InputSystem;

public class UrineShooter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject urinePrefab;
    [SerializeField] private ParticleSystem urineStream;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private AttackManager attackManager;

    [Header("Shooting Settings")]
    [SerializeField] private float minForce = 5f;
    [SerializeField] private float maxForce = 15f;
    [SerializeField] private float shootRate = 0.1f;
    [SerializeField] private float upwardForce = 0.1f;
    [SerializeField] private float baseDamage = 10f;
    [SerializeField] private float urineCost = 10f;

    [Header("Spread Settings")]
    [SerializeField] private float baseSpread = 2f;
    [SerializeField] private float maxSpread = 10f;
    [SerializeField] private float spreadIncreaseRate = 2f;

    [Header("Debug")]
    [SerializeField] private bool showDebugGizmos = true;

    private float currentSpread;
    private float nextShootTime;
    private bool isShooting;
    private float currentTriggerValue;
    private ResourceManager resourceManager;
    private PowerUpManager powerUpManager;
    private bool isActive = true;

    private void Start()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        resourceManager = GetComponentInParent<ResourceManager>();
        powerUpManager = GetComponentInParent<PowerUpManager>();

        // Se attackManager non è stato assegnato, cercalo nel parent
        if (attackManager == null)
        {
            Transform interactionsTransform = transform.parent;
            if (interactionsTransform != null)
            {
                attackManager = interactionsTransform.GetComponentInChildren<AttackManager>();
            }
        }

        if (resourceManager == null || attackManager == null)
        {
            Debug.LogError("Required components missing for UrineShooter!");
            return;
        }

        if (urineStream != null)
        {
            urineStream.Stop();
        }

        // Disattiva inizialmente se l'attacco non è sbloccato
        if (!attackManager.IsAttackUnlocked(gameObject))
        {
            isActive = false;
            enabled = false;
        }
    }

    private void Update()
    {
        UpdateActiveState();

        if (!isActive) return;

        HandleShooting();
    }

    private void UpdateActiveState()
    {
        bool hasEnoughUrine = resourceManager != null && resourceManager.GetCurrentUrine() >= urineCost;
        bool isUnlocked = attackManager != null && attackManager.IsAttackUnlocked(gameObject);
        bool shouldBeActive = hasEnoughUrine && isUnlocked;

        if (shouldBeActive != isActive)
        {
            isActive = shouldBeActive;
            if (!isActive && isShooting)
            {
                StopShooting();
            }
        }
    }

    private void HandleShooting()
    {
        if (currentTriggerValue > 0.1f)
        {
            if (!isShooting)
            {
                StartShooting();
            }

            currentSpread = Mathf.Min(currentSpread + (spreadIncreaseRate * Time.deltaTime), maxSpread);

            if (Time.time >= nextShootTime)
            {
                if (resourceManager != null && resourceManager.ConsumeUrine(urineCost))
                {
                    Shoot(currentTriggerValue);
                    nextShootTime = Time.time + shootRate;
                }
                else
                {
                    StopShooting();
                }
            }
        }
        else if (isShooting)
        {
            StopShooting();
        }
    }

    public void OnTriggerFire(InputAction.CallbackContext context)
    {
        if (!isActive) return;
        currentTriggerValue = context.ReadValue<float>();
    }

    private void StartShooting()
    {
        isShooting = true;
        currentSpread = baseSpread;
        if (urineStream != null)
        {
            urineStream.Play();
            urineStream.transform.position = shootPoint.position;
            urineStream.transform.rotation = shootPoint.rotation;
        }
    }

    private void StopShooting()
    {
        isShooting = false;
        if (urineStream != null)
        {
            urineStream.Stop();
        }
    }

    private void Shoot(float triggerValue)
    {
        float force = Mathf.Lerp(minForce, maxForce, triggerValue);
        float randomSpreadX = Random.Range(-currentSpread, currentSpread);
        float randomSpreadY = Random.Range(-currentSpread, currentSpread);
        GameObject projectile = Instantiate(urinePrefab, shootPoint.position, shootPoint.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        UrineProjectile urineProjectile = projectile.GetComponent<UrineProjectile>();
        if (rb != null)
        {
            if (urineProjectile != null)
            {
                float finalDamage = baseDamage;
                if (powerUpManager != null)
                {
                    finalDamage *= powerUpManager.GetDamageMultiplier();
                }
                urineProjectile.SetDamage(finalDamage);
            }
            Vector3 direction = shootPoint.forward;
            direction = Quaternion.Euler(randomSpreadX, randomSpreadY, 0) * direction;
            Vector3 forceVector = (direction + Vector3.up * upwardForce).normalized * force;
            rb.AddForce(forceVector, ForceMode.Impulse);
            Debug.DrawRay(shootPoint.position, forceVector, Color.red, 1f);
        }
        else
        {
            Debug.LogError("No Rigidbody found on urine prefab!");
            Destroy(projectile);
        }
    }

    private void OnDrawGizmos()
    {
        if (showDebugGizmos && shootPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(shootPoint.position, 0.1f);
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(shootPoint.position, shootPoint.forward * 2f);
        }
    }
}