using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DiarrheaProjectile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float lifeTime = 2f;
    [SerializeField] private TrailRenderer trailRenderer;

    [Header("Splash Effect")]
    [SerializeField] private GameObject diarrheaSplashPrefab;
    [SerializeField] private float splashDuration = 8f;  // Più lungo dell'urina

    private float damage;
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

    public void AddStatusEffect(StatusEffectType type, float duration, float power)
    {
        statusEffects.Add((type, duration, power));
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Se colpisce il pavimento, crea lo splash
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            CreateSplash(collision.contacts[0].point);
            return;
        }

        // Ignora il collider della damage zone del nemico
        if (collision.gameObject.layer == LayerMask.NameToLayer("EnemyDamageZone")) return;

        // Gestione colpito nemico
        EnemyTarget target = collision.gameObject.GetComponent<EnemyTarget>();
        if (target != null)
        {
            DamageNumberManager damageManager = target.GetComponentInParent<DamageNumberManager>();
            if (damageManager != null)
            {
                damageManager.AddDamage(damage * target.damageMultiplier, collision.contacts[0].point);
            }
            ApplyDamageAndEffects(collision.gameObject);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Se colpisce il pavimento, crea lo splash
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            CreateSplash(other.ClosestPoint(transform.position));
            return;
        }

        // Ignora il collider della damage zone del nemico
        if (other.gameObject.layer == LayerMask.NameToLayer("EnemyDamageZone")) return;

        // Gestione colpito nemico
        EnemyTarget target = other.GetComponent<EnemyTarget>();
        if (target != null)
        {
            DamageNumberManager damageManager = target.GetComponentInParent<DamageNumberManager>();
            if (damageManager != null)
            {
                damageManager.AddDamage(damage * target.damageMultiplier, other.ClosestPoint(transform.position));
            }
            ApplyDamageAndEffects(other.gameObject);
            Destroy(gameObject);
        }
    }

    private void CreateSplash(Vector3 hitPoint)
    {
        if (diarrheaSplashPrefab != null)
        {
            // Usa la scala del proiettile per determinare la dimensione della macchia
            float splashSize = transform.localScale.x * 2f;

            // Usiamo un offset Y molto piccolo
            Vector3 spawnPosition = hitPoint + Vector3.up * 0.001f;
            GameObject splash = Instantiate(diarrheaSplashPrefab, spawnPosition, Quaternion.Euler(90, 0, 0));
            splash.transform.localScale = Vector3.one * splashSize;

            // Aggiungi e inizializza DiarrheaSplash per il danno over time
            DiarrheaSplash splashComponent = splash.AddComponent<DiarrheaSplash>();
            splashComponent.Initialize(damage * 0.5f, splashDuration);

            // Usa il SplashEffect per il fade
            var splashEffect = splash.GetComponent<SplashEffect>();
            if (splashEffect != null)
            {
                splashEffect.StartFade(splashDuration);
            }
        }

        Destroy(gameObject);
    }

    private void ApplyDamageAndEffects(GameObject targetObject)
    {
        IEnemy enemy = targetObject.GetComponentInParent<IEnemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            foreach (var effect in statusEffects)
            {
                enemy.ApplyStatusEffect(effect.type, effect.duration, effect.power);
            }

            DamageVisualEffect visualEffect = targetObject.GetComponentInParent<DamageVisualEffect>();
            if (visualEffect != null)
            {
                visualEffect.ApplyEffect(AttackType.Diarrhea);
            }
        }
    }
}