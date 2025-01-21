using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShitProjectile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float lifeTime = 2f;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private GameObject damageNumberPrefab;
    [SerializeField] private GameObject shitSplashPrefab;
    [SerializeField] private float splashDuration = 5f;

    private float damage;
    private float explosionRadius;
    private List<(StatusEffectType type, float duration, float power)> statusEffects =
        new List<(StatusEffectType, float, float)>();

    private void Start()
    {
        Destroy(gameObject, lifeTime);

        if (trailRenderer == null)
        {
            trailRenderer = GetComponent<TrailRenderer>();
        }
        if (trailRenderer != null)
        {
            trailRenderer.startWidth = transform.localScale.x * 0.8f;
            trailRenderer.endWidth = 0f;
            trailRenderer.time = 0.5f;
        }
    }

    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }

    public void SetExplosionRadius(float radius)
    {
        explosionRadius = radius;
    }

    public void AddStatusEffect(StatusEffectType type, float duration, float power)
    {
        statusEffects.Add((type, duration, power));
    }

    private void OnTriggerEnter(Collider other)
    {
        // Esplodi quando colpisce il pavimento
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Explode();
        }
    }

    private void Explode()
    {
        // Crea la macchia sul terreno
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            GameObject splash = Instantiate(shitSplashPrefab, hit.point + Vector3.up * 0.01f, Quaternion.Euler(90, 0, 0));
            splash.transform.localScale = Vector3.one * (explosionRadius * 2);

            // Ottieni il componente SplashEffect e avvia il fade
            var splashEffect = splash.GetComponent<SplashEffect>();
            if (splashEffect != null)
            {
                splashEffect.StartFade(splashDuration);
            }
        }

        // Trova tutti i collider nell'area dell'esplosione
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

        foreach (var hitCollider in hitColliders)
        {
            EnemyTarget target = hitCollider.GetComponent<EnemyTarget>();
            if (target != null && !hitEnemies.Contains(hitCollider.gameObject))
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                float damageMultiplier = 1f - (distance / explosionRadius);
                damageMultiplier = Mathf.Max(0.1f, damageMultiplier);
                float actualDamage = damage * damageMultiplier * target.damageMultiplier;

                DamageNumberManager damageManager = target.GetComponentInParent<DamageNumberManager>();
                if (damageManager != null)
                {
                    damageManager.AddDamage(actualDamage, hitCollider.transform.position);
                }

                ApplyDamageAndEffects(hitCollider.gameObject, actualDamage);
                hitEnemies.Add(hitCollider.gameObject);
            }
        }

        Destroy(gameObject);
    }



    private void ApplyDamageAndEffects(GameObject targetObject, float actualDamage)
    {
        IEnemy enemy = targetObject.GetComponentInParent<IEnemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(actualDamage);
            foreach (var effect in statusEffects)
            {
                enemy.ApplyStatusEffect(effect.type, effect.duration, effect.power);
            }

            DamageVisualEffect visualEffect = targetObject.GetComponentInParent<DamageVisualEffect>();
            if (visualEffect != null)
            {
                visualEffect.ApplyEffect(AttackType.Shit);
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // Visualizza il raggio dell'esplosione nell'editor
        Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
#endif
}