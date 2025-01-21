using UnityEngine;
using UnityEngine.InputSystem;
using GameCreator.Runtime.Common;

public class ShitAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject shitPrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private AttackManager attackManager;

    [Header("Attack Settings")]
    [SerializeField] private float baseDamage = 30f;
    [SerializeField] private float shootForce = 10f;
    [SerializeField] private float upwardForce = 5f;
    [SerializeField] private float projectileSize = 2f;
    [SerializeField] private float cooldownTime = 1f;
    [SerializeField] private float explosionRadius = 5f;    // Spostato qui dal ShitProjectile

    [Header("Resource Costs")]
    [SerializeField] private float foodCost = 15f;

    [Header("Debug")]
    [SerializeField] private bool showDebugGizmos = true;

    private ResourceManager resourceManager;
    private bool isActive = true;
    private float nextAttackTime;

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
            Debug.LogError("Required components missing for ShitAttack!");
            return;
        }

        if (!attackManager.IsAttackUnlocked(gameObject))
        {
            isActive = false;
            enabled = false;
        }
    }

    private bool HasEnoughResources()
    {
        return resourceManager != null && resourceManager.GetCurrentFood() >= foodCost;
    }

    private void UpdateActiveState()
    {
        bool hasResources = HasEnoughResources();
        bool isUnlocked = attackManager != null && attackManager.IsAttackUnlocked(gameObject);
        bool shouldBeActive = hasResources && isUnlocked;

        if (shouldBeActive != isActive)
        {
            isActive = shouldBeActive;
        }
    }

    public void OnShit(InputAction.CallbackContext context)
    {
        if (!isActive) return;

        if (context.performed && Time.time >= nextAttackTime)
        {
            if (HasEnoughResources())
            {
                Shoot();
                nextAttackTime = Time.time + cooldownTime;
            }
        }
    }

    private void Shoot()
    {
        if (resourceManager.ConsumeFood(foodCost))
        {
            GameObject projectile = Instantiate(shitPrefab, shootPoint.position, shootPoint.rotation);
            projectile.transform.localScale = Vector3.one * projectileSize;

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            ShitProjectile shitProjectile = projectile.GetComponent<ShitProjectile>();

            if (rb != null && shitProjectile != null)
            {
                shitProjectile.SetDamage(baseDamage);
                shitProjectile.SetExplosionRadius(explosionRadius);

                Vector3 throwForce = (shootPoint.forward * shootForce) + (Vector3.up * upwardForce);
                rb.AddForce(throwForce, ForceMode.Impulse);
            }
            else
            {
                Debug.LogError("Missing components on shit prefab!");
                Destroy(projectile);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (showDebugGizmos && shootPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(shootPoint.position, 0.2f);
            Gizmos.DrawRay(shootPoint.position, shootPoint.forward * 2f);
        }
    }
}