using UnityEngine;
using GameCreator.Runtime.Common;

public class StinkAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject stinkCloudPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private AttackManager attackManager;

    [Header("Settings")]
    [SerializeField] private float duration = 3f;
    [SerializeField] private float cooldown = 5f;
    [SerializeField] private float baseDamage = 3f;
    [SerializeField] private float auraRadius = 2f;

    private float nextActivationTime;
    private GameObject currentCloud;
    private bool isActive;
    private bool isInitialized;
    private ResourceManager resourceManager;

    private void OnEnable()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        if (!isInitialized)
        {
            resourceManager = GetComponentInParent<ResourceManager>();

            // Trova l'AttackManager nel parent o nei siblings
            Transform interactionsTransform = transform.parent;
            if (interactionsTransform != null)
            {
                if (attackManager == null)
                {
                    attackManager = interactionsTransform.GetComponentInChildren<AttackManager>();
                    if (attackManager == null)
                    {
                        attackManager = interactionsTransform.GetComponent<AttackManager>();
                    }
                }
            }

            if (attackManager == null)
            {
                Debug.LogError("AttackManager not found!");
                return;
            }

            // Default spawn point se non impostato
            if (spawnPoint == null)
            {
                spawnPoint = transform;
            }

            isInitialized = true;
            nextActivationTime = Time.time;
            Debug.Log("StinkAttack initialized successfully!");
        }
    }

    private void Update()
    {
        if (!isInitialized || !attackManager.IsAttackUnlocked(gameObject)) return;

        if (Time.time >= nextActivationTime && !isActive)
        {
            ActivateStinkCloud();
        }
    }

    private void ActivateStinkCloud()
    {
        if (stinkCloudPrefab != null)
        {
            isActive = true;
            currentCloud = Instantiate(stinkCloudPrefab, spawnPoint.position, Quaternion.identity, spawnPoint);
            StinkCloud cloudComponent = currentCloud.GetComponent<StinkCloud>();

            if (cloudComponent != null)
            {
                cloudComponent.Initialize(baseDamage, duration, auraRadius);
                cloudComponent.OnCloudDestroyed += OnCloudDestroyed;
            }
            else
            {
                Debug.LogError("StinkCloud component not found on prefab!");
                Destroy(currentCloud);
            }
        }
        else
        {
            Debug.LogError("Stink Cloud prefab not assigned!");
        }
    }

    private void OnCloudDestroyed()
    {
        isActive = false;
        nextActivationTime = Time.time + cooldown;
        if (currentCloud != null)
        {
            var cloudComponent = currentCloud.GetComponent<StinkCloud>();
            if (cloudComponent != null)
            {
                cloudComponent.OnCloudDestroyed -= OnCloudDestroyed;
            }
        }
        currentCloud = null;
    }

    public void UpgradeDamage(float multiplier)
    {
        baseDamage *= multiplier;
    }

    public void UpgradeRadius(float multiplier)
    {
        auraRadius *= multiplier;
    }

    public void UpgradeDuration(float multiplier)
    {
        duration *= multiplier;
    }

    public void SetCooldown(float newCooldown)
    {
        cooldown = newCooldown;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (spawnPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(spawnPoint.position, auraRadius);
        }
    }
#endif
}