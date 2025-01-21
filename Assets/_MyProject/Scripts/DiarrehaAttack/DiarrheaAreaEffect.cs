using UnityEngine;
using System.Collections.Generic;

public class DiarrheaAreaEffect : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject damageNumberPrefab;
    [SerializeField] private ParticleSystem explosionEffect;

    private float damage;
    private float radius;
    private float duration = 0.5f;  // Durata breve come BurpAura perché è un danno istantaneo

    private SphereCollider areaCollider;
    private HashSet<GameObject> affectedEnemies = new HashSet<GameObject>();

    private void Awake()
    {
        areaCollider = GetComponent<SphereCollider>();
        if (areaCollider == null)
        {
            areaCollider = gameObject.AddComponent<SphereCollider>();
            areaCollider.isTrigger = true;
        }
    }

    public void Initialize(float damage, float radius)
    {
        this.damage = damage;
        this.radius = radius;

        if (areaCollider != null)
        {
            areaCollider.radius = radius;
        }

        // Scala il sistema particellare in base al radius
        if (explosionEffect != null)
        {
            var mainModule = explosionEffect.main;
            mainModule.startSpeedMultiplier = radius * 2;
            mainModule.startSizeMultiplier = radius * 0.5f;
            explosionEffect.Play();
        }

        // Applica il danno immediatamente
        ApplyBurstDamage();
        Destroy(gameObject, duration);
    }

    private void ApplyBurstDamage()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider col in hitColliders)
        {
            EnemyTarget target = col.GetComponent<EnemyTarget>();
            if (target != null)
            {
                ApplyDamage(col.gameObject);
            }
        }
    }

    private void ApplyDamage(GameObject targetObject)
    {
        if (targetObject == null) return;

        EnemyTarget target = targetObject.GetComponent<EnemyTarget>();
        IEnemy enemy = targetObject.GetComponentInParent<IEnemy>();

        if (target != null && enemy != null)
        {
            float actualDamage = damage * target.damageMultiplier;

            DamageNumberManager damageManager = target.GetComponentInParent<DamageNumberManager>();
            if (damageManager != null)
            {
                damageManager.AddDamage(actualDamage, targetObject.transform.position);
            }

            enemy.TakeDamage(actualDamage);

            DamageVisualEffect visualEffect = targetObject.GetComponentInParent<DamageVisualEffect>();
            if (visualEffect != null)
            {
                visualEffect.ApplyEffect(AttackType.Diarrhea);
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.4f, 0.2f, 0f, 0.3f); // Marrone semitrasparente
        Gizmos.DrawSphere(transform.position, radius);
    }
#endif
}