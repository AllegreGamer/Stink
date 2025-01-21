using UnityEngine;
using System.Collections.Generic;

public class SpitProjectile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private GameObject damageNumberPrefab;
    [SerializeField] private GameObject spitSplashPrefab;
    [SerializeField] private float splashDuration = 2f;

    private float damage;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Se colpisce il pavimento, crea lo splash
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            CreateSplash(collision.contacts[0].point);
            Destroy(gameObject);
            return;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Se colpisce il pavimento, crea lo splash
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            CreateSplash(other.ClosestPoint(transform.position));
            Destroy(gameObject);
            return;
        }

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
        if (spitSplashPrefab != null)
        {
            GameObject splash = Instantiate(spitSplashPrefab, hitPoint + Vector3.up * 0.001f, Quaternion.Euler(90, 0, 0));
            splash.transform.localScale = Vector3.one * 0.5f;  // Dimensione fissa della macchia

            var splashEffect = splash.GetComponent<SplashEffect>();
            if (splashEffect != null)
            {
                splashEffect.StartFade(splashDuration);
            }
        }
    }

    private void ApplyDamageAndEffects(GameObject targetObject)
    {
        IEnemy enemy = targetObject.GetComponentInParent<IEnemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);

            DamageVisualEffect visualEffect = targetObject.GetComponentInParent<DamageVisualEffect>();
            if (visualEffect != null)
            {
                visualEffect.ApplyEffect(AttackType.Spit);
            }
        }
    }
}