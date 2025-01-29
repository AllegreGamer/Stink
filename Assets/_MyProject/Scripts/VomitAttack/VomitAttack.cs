using UnityEngine;
using UnityEngine.InputSystem;
using GameCreator.Runtime.Common;
public class VomitAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject vomitAreaPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private AttackManager attackManager;

    [Header("Attack Settings")]
    [SerializeField] private float baseDamage = 50f;
    [SerializeField] private float attackRadius = 3f;
    [SerializeField] private float cooldownTime = 2f;

    [Header("Resource Costs")]
    [SerializeField] private float foodCost = 20f;
    [SerializeField] private float alcoholCost = 15f;

    private ResourceManager resourceManager;
    private PowerUpManager powerUpManager;
    private bool isInitialized;
    private float nextAttackTime;

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
                Debug.LogError($"Required components missing in VomitAttack! ResourceManager: {resourceManager != null}, AttackManager: {attackManager != null}");
                return;
            }

            if (spawnPoint == null)
            {
                spawnPoint = transform;
            }

            isInitialized = true;
        }
    }

    public void OnVomit(InputAction.CallbackContext context)
    {
        if (!isInitialized) return;

        if (!attackManager.IsAttackUnlocked(gameObject))
        {
            Debug.Log("VomitAttack not yet unlocked!");
            return;
        }

        if (context.performed && Time.time >= nextAttackTime)
        {
            PerformVomitAttack();
        }
    }

    private void PerformVomitAttack()
    {
        if (resourceManager.ConsumeFood(foodCost) && resourceManager.ConsumeAlcohol(alcoholCost))
        {
            SpawnVomitArea();
            nextAttackTime = Time.time + cooldownTime;
        }
        else
        {
            Debug.Log("Not enough resources for Vomit Attack!");
        }
    }

    private void SpawnVomitArea()
    {
        if (vomitAreaPrefab != null && spawnPoint != null)
        {
            GameObject vomitArea = Instantiate(vomitAreaPrefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
            VomitArea areaComponent = vomitArea.GetComponent<VomitArea>();

            if (areaComponent != null)
            {
                float finalDamage = baseDamage;
                if (powerUpManager != null)
                {
                    finalDamage *= powerUpManager.GetDamageMultiplier();
                }
                areaComponent.Initialize(finalDamage, attackRadius);
            }
            else
            {
                Debug.LogError("VomitArea component not found on prefab!");
                Destroy(vomitArea);
            }
        }
        else
        {
            Debug.LogError("Missing references for vomit area spawn!");
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (spawnPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(spawnPoint.position, attackRadius);
        }
    }
#endif
}