using UnityEngine;
using UnityEngine.InputSystem;
using GameCreator.Runtime.Common;

public class FartAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject gasCloudPrefab;
    [SerializeField] private GameObject dashTriggerObject;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private AttackManager attackManager;

    [Header("Gas Cloud Settings")]
    [SerializeField] private float gasCloudDuration = 3f;
    [SerializeField] private float baseDamage = 5f;
    [SerializeField] private float gasCloudRadius = 2f;

    [Header("Resource Settings")]
    [SerializeField] private float fartCost = 20f;

    private ResourceManager resourceManager;
    private bool isActive = true;
    private bool isInitialized = false;

    private void OnEnable()
    {
        // Inizializza i componenti quando l'oggetto viene attivato
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        if (!isInitialized)
        {
            // Get required components
            resourceManager = GetComponentInParent<ResourceManager>();

            // Se attackManager non è stato assegnato manualmente, cercalo
            if (attackManager == null)
            {
                Transform interactionsTransform = transform.parent;
                if (interactionsTransform != null)
                {
                    attackManager = interactionsTransform.GetComponentInChildren<AttackManager>();
                    if (attackManager == null)
                    {
                        attackManager = interactionsTransform.GetComponent<AttackManager>();
                    }
                }
            }

            if (resourceManager == null || attackManager == null)
            {
                Debug.LogError($"Required components missing in FartAttack! ResourceManager: {resourceManager != null}, AttackManager: {attackManager != null}");
                return;
            }

            // Default spawn point if not set
            if (spawnPoint == null)
            {
                spawnPoint = transform;
            }

            isInitialized = true;
            Debug.Log("FartAttack initialized successfully!");
        }
    }

    private void Update()
    {
        if (!isInitialized) return;

        // Check stamina availability
        if (resourceManager != null)
        {
            bool hasEnoughFart = resourceManager.GetCurrentFart() >= fartCost;

            if (hasEnoughFart != isActive)
            {
                isActive = hasEnoughFart;
                if (dashTriggerObject != null)
                {
                    dashTriggerObject.SetActive(isActive && attackManager.IsAttackUnlocked(gameObject));
                }
            }
        }
    }

    public void OnFart(InputAction.CallbackContext context)
    {
        if (!isInitialized) return;

        Debug.Log($"OnFart called. IsUnlocked: {attackManager.IsAttackUnlocked(gameObject)}, IsActive: {isActive}");

        if (!attackManager.IsAttackUnlocked(gameObject) || !isActive)
            return;

        if (context.performed)
        {
            Debug.Log("Attempting to perform fart attack...");
            PerformFartAttack();
        }
    }

    private void PerformFartAttack()
    {
        Debug.Log($"PerformFartAttack called. ResourceManager: {resourceManager != null}, StaminaCost: {fartCost}");

        if (resourceManager != null && resourceManager.ConsumeFart(fartCost))
        {
            SpawnGasCloud();
        }
    }

    private void SpawnGasCloud()
    {
        Debug.Log($"SpawnGasCloud called. GasCloudPrefab: {gasCloudPrefab != null}, SpawnPoint: {spawnPoint != null}");

        if (gasCloudPrefab != null && spawnPoint != null)
        {
            Vector3 spawnPosition = spawnPoint.position;
            Debug.Log($"Spawning gas cloud at position: {spawnPosition}");

            GameObject gasCloud = Instantiate(gasCloudPrefab, spawnPosition, Quaternion.identity);
            GasCloud cloudComponent = gasCloud.GetComponent<GasCloud>();

            if (cloudComponent != null)
            {
                cloudComponent.Initialize(baseDamage, gasCloudDuration, gasCloudRadius);
                Debug.Log("Gas cloud initialized successfully!");
            }
            else
            {
                Debug.LogError("GasCloud component not found on prefab!");
                Destroy(gasCloud);
            }
        }
        else
        {
            Debug.LogError($"Missing references for gas cloud spawn! Prefab: {gasCloudPrefab != null}, SpawnPoint: {spawnPoint != null}");
        }
    }

    public void UpgradeDamage(float multiplier)
    {
        baseDamage *= multiplier;
    }

    public void UpgradeRadius(float multiplier)
    {
        gasCloudRadius *= multiplier;
    }

    public void UpgradeDuration(float multiplier)
    {
        gasCloudDuration *= multiplier;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (spawnPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(spawnPoint.position, gasCloudRadius);
        }
    }
#endif
}