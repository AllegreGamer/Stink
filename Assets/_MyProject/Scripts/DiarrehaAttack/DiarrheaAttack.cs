using UnityEngine;
using UnityEngine.InputSystem;
using GameCreator.Runtime.Common;

public class DiarrheaAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject diarrheaPrefab;
    [SerializeField] private ParticleSystem diarrheaStream;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private AttackManager attackManager;

    [Header("Shooting Settings")]
    [SerializeField] private float baseDamage = 10f;
    [SerializeField] private float minShootForce = 5f;
    [SerializeField] private float maxShootForce = 15f;
    [SerializeField] private float shootRate = 0.1f;
    [SerializeField] private float minProjectileSize = 0.3f;
    [SerializeField] private float maxProjectileSize = 1f;

    [Header("Resource Costs")]
    [SerializeField] private float foodCost = 5f;
    [SerializeField] private float urineCost = 3f;
    [SerializeField] private float alcoholCost = 3f;

    private float nextShootTime;
    private bool isShooting;
    private float currentTriggerValue;
    private ResourceManager resourceManager;
    private bool isActive = true;

    private void Start()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        resourceManager = GetComponentInParent<ResourceManager>();

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
            Debug.LogError("Required components missing for DiarrheaAttack!");
            return;
        }

        if (diarrheaStream != null)
        {
            diarrheaStream.Stop();
        }

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
        bool hasEnoughResources = resourceManager != null &&
                                resourceManager.GetCurrentFood() >= foodCost &&
                                resourceManager.GetCurrentUrine() >= urineCost &&    // Cambiato GetCurrentWater
                                resourceManager.GetCurrentAlcohol() >= alcoholCost;  // Aggiunto check per alcohol
        bool isUnlocked = attackManager != null && attackManager.IsAttackUnlocked(gameObject);
        bool shouldBeActive = hasEnoughResources && isUnlocked;

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

            if (Time.time >= nextShootTime)
            {
                if (resourceManager.ConsumeFood(foodCost) &&
                    resourceManager.ConsumeUrine(urineCost) &&     // Cambiato ConsumeWater
                    resourceManager.ConsumeAlcohol(alcoholCost))    // Aggiunto consumo alcohol
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

    public void OnDiarrhea(InputAction.CallbackContext context)
    {
        if (!isActive) return;
        currentTriggerValue = context.ReadValue<float>();
    }

    private void StartShooting()
    {
        isShooting = true;
        if (diarrheaStream != null)
        {
            diarrheaStream.Play();
            diarrheaStream.transform.position = shootPoint.position;
            diarrheaStream.transform.rotation = shootPoint.rotation;
        }
    }

    private void StopShooting()
    {
        isShooting = false;
        if (diarrheaStream != null)
        {
            diarrheaStream.Stop();
        }
    }

    private void Shoot(float triggerValue)
    {
        GameObject projectile = Instantiate(diarrheaPrefab, shootPoint.position, shootPoint.rotation);

        // Scala il proiettile in base alla pressione del trigger
        float projectileSize = Mathf.Lerp(minProjectileSize, maxProjectileSize, triggerValue);
        projectile.transform.localScale = Vector3.one * projectileSize;

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        DiarrheaProjectile diarrheaProjectile = projectile.GetComponent<DiarrheaProjectile>();

        if (rb != null && diarrheaProjectile != null)
        {
            diarrheaProjectile.SetDamage(baseDamage * triggerValue);

            // Forza di sparo verso il basso
            float force = Mathf.Lerp(minShootForce, maxShootForce, triggerValue);
            Vector3 shootDirection = -Vector3.up; // Spara verso il basso
            rb.AddForce(shootDirection * force, ForceMode.Impulse);
        }
        else
        {
            Debug.LogError("Missing components on diarrhea prefab!");
            Destroy(projectile);
        }
    }
}