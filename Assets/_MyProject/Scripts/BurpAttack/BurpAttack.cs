using UnityEngine;
using UnityEngine.InputSystem;
using GameCreator.Runtime.Common;

public class BurpAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject burpAuraPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private AttackManager attackManager;

    [Header("Burp Settings")]
    [SerializeField] private float baseDamage = 50f;
    [SerializeField] private float auraRadius = 5f;
    [SerializeField] private float pushForce = 20f;
    [SerializeField] private float burpCost = 20f;  // Aggiunto il costo del burp

    private ResourceManager resourceManager;
    private PowerUpManager powerUpManager;
    private bool isInitialized;

    private void OnEnable()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        if (!isInitialized)
        {
            resourceManager = GetComponentInParent<ResourceManager>();
            powerUpManager = GetComponentInParent<PowerUpManager>();

            // Trova l'AttackManager nel parent o nei siblings
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
                Debug.LogError($"Required components missing in BurpAttack! ResourceManager: {resourceManager != null}, AttackManager: {attackManager != null}");
                return;
            }

            // Default spawn point se non impostato
            if (spawnPoint == null)
            {
                spawnPoint = transform;
            }
            
            isInitialized = true;
            Debug.Log("BurpAttack initialized successfully!");
        }
    }

    public void OnBurp(InputAction.CallbackContext context)
    {
        if (!isInitialized) return;

        if (!attackManager.IsAttackUnlocked(gameObject))
        {
            Debug.Log("BurpAttack not yet unlocked!");
            return;
        }

        if (context.performed)
        {
            PerformBurpAttack();
        }
    }

    private void PerformBurpAttack()
    {
        if (resourceManager.ConsumeBurp(burpCost))
        {
            SpawnBurpAura();
        }
        else
        {
            Debug.Log("Not enough burp power!");
        }
    }

    private void SpawnBurpAura()
    {
        if (burpAuraPrefab != null)
        {
            GameObject burpAura = Instantiate(burpAuraPrefab, spawnPoint.position, Quaternion.identity);
            BurpAura auraComponent = burpAura.GetComponent<BurpAura>();
            if (auraComponent != null)
            {
                float finalDamage = baseDamage;
                if (powerUpManager != null)
                {
                    finalDamage *= powerUpManager.GetDamageMultiplier();
                }
                auraComponent.Initialize(finalDamage, auraRadius);
            }
            else
            {
                Debug.LogError("BurpAura component not found on prefab!");
                Destroy(burpAura);
            }
        }
        else
        {
            Debug.LogError("Burp Aura prefab not assigned!");
        }
    }

    public void UpgradeDamage(float multiplier)
    {
        baseDamage *= multiplier;
    }

    public void UpgradeRadius(float multiplier)
    {
        auraRadius *= multiplier;
    }

    public void UpgradePushForce(float multiplier)
    {
        pushForce *= multiplier;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (spawnPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(spawnPoint.position, auraRadius);
        }
    }
#endif
}