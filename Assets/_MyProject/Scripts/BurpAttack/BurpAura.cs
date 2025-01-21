using UnityEngine;
using System.Collections.Generic;

public class BurpAura : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject damageNumberPrefab;

    private float damage;
    private float radius;
    private float duration = 0.5f;

    private SphereCollider auraCollider;
    private HashSet<GameObject> affectedEnemies = new HashSet<GameObject>();

    private void Awake()
    {
        auraCollider = GetComponent<SphereCollider>();
        if (auraCollider == null)
        {
            auraCollider = gameObject.AddComponent<SphereCollider>();
            auraCollider.isTrigger = true;
        }
    }

    public void Initialize(float damage, float radius)
    {
        this.damage = damage;
        this.radius = radius;

        if (auraCollider != null)
        {
            auraCollider.radius = radius;
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
            // Calcola il danno
            float actualDamage = damage * target.damageMultiplier;

            // Mostra il numero del danno
            DamageNumberManager damageManager = target.GetComponentInParent<DamageNumberManager>();
            if (damageManager != null)
            {
                damageManager.AddDamage(actualDamage, targetObject.transform.position);
            }
            else
            {
                ShowDamageNumber(targetObject.transform.position, actualDamage);
            }

            // Applica il danno
            enemy.TakeDamage(actualDamage);

            DamageVisualEffect visualEffect = targetObject.GetComponentInParent<DamageVisualEffect>();
            if (visualEffect != null)
            {
                visualEffect.ApplyEffect(AttackType.Burp);
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

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 1f, 0.2f);
        Gizmos.DrawSphere(transform.position, radius);
    }
#endif
}