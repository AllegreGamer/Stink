using UnityEngine;
using UnityEngine.InputSystem;
using GameCreator.Runtime.Common;

public class SpitAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject spitPrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private AttackManager attackManager;
    [SerializeField] private GameObject chargeIndicator;

    [Header("Charge Settings")]
    [SerializeField] private float maxChargeTime = 4f;      // Tempo massimo di carica

    [Header("Projectile Settings")]
    [SerializeField] private float baseDamage = 5f;         // Danno base del proiettile
    [SerializeField] private float maxDamageMultiplier = 3f;// Moltiplicatore massimo del danno

    [Header("Spread Settings")]
    [SerializeField] private int minProjectiles = 2;        // Proiettili con tap
    [SerializeField] private int maxProjectiles = 5;        // Proiettili con carica massima
    [SerializeField] private float minSpread = 5f;          // Spread con tap
    [SerializeField] private float maxSpread = 20f;         // Spread con carica massima

    [Header("Force Settings")]
    [SerializeField] private float minForce = 15f;          // Forza con tap
    [SerializeField] private float maxForce = 25f;          // Forza con carica massima
    [SerializeField] private float upwardForce = 0.1f;

    private float chargeStartTime;
    private bool isCharging;

    private void Start()
    {
        InitializeComponents();
        if (chargeIndicator != null)
        {
            chargeIndicator.SetActive(false);  // Nascondi l'indicatore all'inizio
        }
    }

    private void Update()
    {
        if (isCharging)
        {
            UpdateChargeIndicator();
        }
    }

    private void InitializeComponents()
    {
        if (attackManager == null)
        {
            Transform interactionsTransform = transform.parent;
            if (interactionsTransform != null)
            {
                attackManager = interactionsTransform.GetComponentInChildren<AttackManager>();
            }
        }

        if (attackManager == null)
        {
            Debug.LogError("AttackManager missing in SpitAttack!");
        }
    }

    public void OnSpit(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            StartCharging();
        }
        else if (context.canceled)
        {
            ReleaseCharge();
        }
    }

    private void StartCharging()
    {
        isCharging = true;
        chargeStartTime = Time.time;
        if (chargeIndicator != null)
        {
            chargeIndicator.SetActive(true);
            chargeIndicator.transform.localScale = Vector3.one * 0.1f;  // Dimensione iniziale
        }
    }

    private void ReleaseCharge()
    {
        if (!isCharging) return;

        float chargeTime = Time.time - chargeStartTime;
        float chargeProgress = Mathf.Clamp01(chargeTime / maxChargeTime);

        float currentDamage = baseDamage * Mathf.Lerp(1f, maxDamageMultiplier, chargeProgress);
        ShootProjectiles(chargeProgress, currentDamage);

        isCharging = false;
        if (chargeIndicator != null)
        {
            chargeIndicator.SetActive(false);
        }
    }

    private void UpdateChargeIndicator()
    {
        if (chargeIndicator != null)
        {
            float chargeTime = Time.time - chargeStartTime;
            float chargeProgress = Mathf.Clamp01(chargeTime / maxChargeTime);

            // Scala da 0.3 a 1.0 in base al progresso
            float scale = 0.1f + (chargeProgress * 0.4f);

            // Se la carica è al massimo, fai pulsare l'indicatore
            if (chargeProgress >= 0.99f)
            {
                float pulse = Mathf.Sin(Time.time * 30f) * 0.2f + 0.3f;
                chargeIndicator.transform.localScale = Vector3.one * pulse;
            }
            else
            {
                chargeIndicator.transform.localScale = Vector3.one * scale;
            }
        }
    }

    private void ShootProjectiles(float chargeProgress, float currentDamage)
    {
        int projectileCount = Mathf.RoundToInt(Mathf.Lerp(minProjectiles, maxProjectiles, chargeProgress));
        float spreadAngle = Mathf.Lerp(minSpread, maxSpread, chargeProgress);
        float force = Mathf.Lerp(minForce, maxForce, chargeProgress);

        for (int i = 0; i < projectileCount; i++)
        {
            float angleStep = spreadAngle / (projectileCount - 1);
            float currentAngle = -spreadAngle / 2 + angleStep * i;
            if (projectileCount == 1) currentAngle = 0f;

            GameObject projectile = Instantiate(spitPrefab, shootPoint.position, shootPoint.rotation);
            projectile.transform.Rotate(Vector3.up, currentAngle);

            SpitProjectile spitProjectile = projectile.GetComponent<SpitProjectile>();
            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            if (spitProjectile != null && rb != null)
            {
                spitProjectile.SetDamage(currentDamage);
                Vector3 directionWithSpread = Quaternion.Euler(0, currentAngle, 0) * shootPoint.forward;
                Vector3 throwForce = (directionWithSpread * force) + (Vector3.up * upwardForce);
                rb.AddForce(throwForce, ForceMode.Impulse);
            }
            else
            {
                Debug.LogError("Missing components on spit prefab!");
                Destroy(projectile);
            }
        }
    }
}