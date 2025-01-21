using UnityEngine;
using UnityEngine.InputSystem;
using GameCreator.Runtime.Common;
using System.Collections.Generic;

public class VomitProjectile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float lifeTime = 2f;
    [SerializeField] private TrailRenderer trailRenderer;

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
            // Personalizza il trail renderer per l'effetto vomito
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

    private void OnTriggerEnter(Collider other)
    {
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
        }
    }
}