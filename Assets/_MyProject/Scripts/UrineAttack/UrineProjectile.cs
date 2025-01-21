using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UrineProjectile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private GameObject damageNumberPrefab;

    [Header("Splash Effect")]
    [SerializeField] private GameObject urineSplashPrefab;
    [SerializeField] private float splashDuration = 5f;
    [SerializeField] [Range(0.1f, 2f)] private float minSplashSize = 0.3f;  // Dimensione minima
    [SerializeField] [Range(0.1f, 2f)] private float maxSplashSize = 0.7f;

    private float damage;
    private List<(StatusEffectType type, float duration, float power)> statusEffects =
        new List<(StatusEffectType, float, float)>();

    private void Start()
    {
        Destroy(gameObject, lifeTime);
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
        // Ignora il collider della damage zone del nemico
        if (collision.gameObject.layer == LayerMask.NameToLayer("EnemyDamageZone")) return;

        // Se colpisce il pavimento, crea lo splash e poi distruggi il proiettile
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            CreateSplash(collision.contacts[0].point);
            //Destroy(gameObject);
            return;
        }
        // Se colpisce un EnemyTarget, applica danno e mostra i numeri
        EnemyTarget target = collision.gameObject.GetComponent<EnemyTarget>();
        if (target != null)
        {
            // Trova il DamageNumberManager nel parent dell'EnemyTarget
            DamageNumberManager damageManager = target.GetComponentInParent<DamageNumberManager>();
            if (damageManager != null)
            {
                // Aggiungi il danno al manager invece di creare direttamente il numero
                damageManager.AddDamage(damage * target.damageMultiplier, collision.contacts[0].point);
            }

            // Applica il danno come prima
            ApplyDamageAndEffects(collision.gameObject);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ignora il collider della damage zone del nemico
        if (other.gameObject.layer == LayerMask.NameToLayer("EnemyDamageZone")) return;

        // Se colpisce il pavimento, crea lo splash e poi distruggi il proiettile
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            CreateSplash(other.ClosestPoint(transform.position));
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

    private void ApplyDamageAndEffects(GameObject targetObject)
    {
        // Ottieni l'IEnemy dal parent dell'EnemyTarget
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
                visualEffect.ApplyEffect(AttackType.Urine);
            }
        }
    }

    private void CreateSplash(Vector3 hitPoint)
    {
        if (urineSplashPrefab != null)
        {
            GameObject splash = Instantiate(urineSplashPrefab, hitPoint + Vector3.up * 0.001f, Quaternion.Euler(90, 0, 0));

            // Genera una dimensione casuale tra min e max
            float randomSize = Random.Range(minSplashSize, maxSplashSize);
            splash.transform.localScale = Vector3.one * randomSize;

            // Ottieni il componente SplashEffect e avvia il fade
            var splashEffect = splash.GetComponent<SplashEffect>();
            if (splashEffect != null)
            {
                splashEffect.StartFade(splashDuration);
            }
        }
    }

    /*private void ShowDamageNumber(Vector3 position)
    {
        if (damageNumberPrefab != null)
        {
            GameObject damageNumberObj = Instantiate(damageNumberPrefab, position, Quaternion.identity);
            DamageNumber damageNumber = damageNumberObj.GetComponent<DamageNumber>();
            if (damageNumber != null)
            {
                damageNumber.Setup(damage);
            }
        }
    }*/
}