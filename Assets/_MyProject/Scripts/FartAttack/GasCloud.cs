using UnityEngine;
using System.Collections.Generic;

public class GasCloud : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ParticleSystem gasParticles;
    [SerializeField] private GameObject damageNumberPrefab;

    private float damage;
    private float duration;
    private float radius;
    private float damageTickRate = 0.5f;
    private float nextDamageTime;

    private SphereCollider cloudCollider;
    private HashSet<GameObject> enemiesInCloud = new HashSet<GameObject>();
    private List<(StatusEffectType type, float duration, float power)> statusEffects =
        new List<(StatusEffectType, float, float)>();

    private void Awake()
    {
        if (gasParticles == null)
            gasParticles = GetComponentInChildren<ParticleSystem>();

        cloudCollider = GetComponent<SphereCollider>();
        if (cloudCollider == null)
        {
            cloudCollider = gameObject.AddComponent<SphereCollider>();
            cloudCollider.isTrigger = true;
        }
    }

    public void Initialize(float damage, float duration, float radius)
    {
        this.damage = damage;
        this.duration = duration;
        this.radius = radius;

        if (cloudCollider != null)
        {
            cloudCollider.radius = radius;
        }

        if (gasParticles != null)
        {
            gasParticles.Play();
        }

        Destroy(gameObject, duration);
    }

    public void AddStatusEffect(StatusEffectType type, float duration, float power)
    {
        statusEffects.Add((type, duration, power));
    }

    private void Update()
    {
        if (Time.time >= nextDamageTime)
        {
            DealDamageToEnemiesInCloud();
            nextDamageTime = Time.time + damageTickRate;
        }
    }

    private void DealDamageToEnemiesInCloud()
    {
        foreach (GameObject enemy in enemiesInCloud)
        {
            if (enemy != null)
            {
                ApplyDamageAndEffects(enemy);
            }
        }
        enemiesInCloud.RemoveWhere(enemy => enemy == null);
    }

    private void ApplyDamageAndEffects(GameObject targetObject)
    {
        EnemyTarget target = targetObject.GetComponent<EnemyTarget>();
        IEnemy enemy = targetObject.GetComponentInParent<IEnemy>();

        if (target != null && enemy != null)
        {
            float actualDamage = damage * damageTickRate * target.damageMultiplier;

            // Trova il DamageNumberManager nel parent dell'EnemyTarget
            DamageNumberManager damageManager = target.GetComponentInParent<DamageNumberManager>();
            if (damageManager != null)
            {
                damageManager.AddDamage(actualDamage, targetObject.transform.position);
            }
            else
            {
                ShowDamageNumber(targetObject.transform.position, actualDamage);
            }

            // Applica danno e effetti
            enemy.TakeDamage(actualDamage);
            foreach (var effect in statusEffects)
            {
                enemy.ApplyStatusEffect(effect.type, effect.duration, effect.power);
            }

            DamageVisualEffect visualEffect = targetObject.GetComponentInParent<DamageVisualEffect>();
            if (visualEffect != null)
            {
                visualEffect.ApplyEffect(AttackType.Gas);
            }
        }
    }

    private void ShowDamageNumber(Vector3 position, float damageAmount)
    {
        if (damageNumberPrefab != null)
        {
            GameObject damageNumberObj = Instantiate(damageNumberPrefab, position, Quaternion.identity);
            DamageNumber damageNumber = damageNumberObj.GetComponent<DamageNumber>();
            if (damageNumber != null)
            {
                damageNumber.Setup(damageAmount);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyTarget target = other.GetComponent<EnemyTarget>();
        if (target != null)
        {
            enemiesInCloud.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        EnemyTarget target = other.GetComponent<EnemyTarget>();
        if (target != null)
        {
            enemiesInCloud.Remove(other.gameObject);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.3f, 0.8f, 0.3f, 0.2f);
        Gizmos.DrawSphere(transform.position, radius);
    }
#endif
}