using UnityEngine;
using System.Collections.Generic;

public class DiarrheaSplash : MonoBehaviour
{
    [Header("Damage Settings")]
    private float damagePerSecond;
    private float duration;
    private float damageTickRate = 0.5f;  // Applica danno ogni mezzo secondo
    private float nextDamageTime;

    private HashSet<GameObject> enemiesInSplash = new HashSet<GameObject>();
    private SphereCollider damageCollider;

    // Aggiungi questi metodi per il DiarrheaAreaManager
    public Vector3 GetPosition() => transform.position;
    public float GetRadius() => transform.localScale.x / 2f;
    public float GetDamage() => damagePerSecond; // Restituisce il danno base della macchia

    public void Initialize(float damage, float lifetime)
    {
        this.damagePerSecond = damage;
        this.duration = lifetime;

        // Aggiungi un collider per il danno
        damageCollider = gameObject.AddComponent<SphereCollider>();
        damageCollider.isTrigger = true;
        damageCollider.radius = transform.localScale.x / 2f;  // Metà della scala per matchare la visuale

        // Registra questa macchia al manager
        if (DiarrheaAreaManager.Instance != null)
        {
            DiarrheaAreaManager.Instance.RegisterSpot(this);
        }
    }

    private void Update()
    {
        if (Time.time >= nextDamageTime)
        {
            DamageEnemiesInSplash();
            nextDamageTime = Time.time + damageTickRate;
        }
    }

    private void DamageEnemiesInSplash()
    {
        foreach (var enemy in enemiesInSplash)
        {
            if (enemy != null)  // Check per sicurezza se il nemico esiste ancora
            {
                EnemyTarget target = enemy.GetComponent<EnemyTarget>();
                if (target != null)
                {
                    float actualDamage = damagePerSecond * damageTickRate * target.damageMultiplier;

                    // Mostra il danno
                    DamageNumberManager damageManager = target.GetComponentInParent<DamageNumberManager>();
                    if (damageManager != null)
                    {
                        damageManager.AddDamage(actualDamage, enemy.transform.position);
                    }

                    // Applica il danno
                    IEnemy enemyComponent = enemy.GetComponentInParent<IEnemy>();
                    if (enemyComponent != null)
                    {
                        enemyComponent.TakeDamage(actualDamage);

                        // Applica l'effetto visivo
                        DamageVisualEffect visualEffect = enemy.GetComponentInParent<DamageVisualEffect>();
                        if (visualEffect != null)
                        {
                            visualEffect.ApplyEffect(AttackType.Diarrhea);
                        }
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) return;  // Ignora il player

        EnemyTarget target = other.GetComponent<EnemyTarget>();
        if (target != null)
        {
            enemiesInSplash.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        enemiesInSplash.Remove(other.gameObject);
    }

    private void OnDestroy()
    {
        // Deregistra questa macchia quando viene distrutta
        if (DiarrheaAreaManager.Instance != null)
        {
            DiarrheaAreaManager.Instance.UnregisterSpot(this);
        }
    }

    private void OnDrawGizmos()
    {
        // Disegna sempre il gizmo, non solo quando selezionato
        Gizmos.color = new Color(0.4f, 0.2f, 0f, 0.3f); // Marrone semitrasparente
        Gizmos.DrawSphere(transform.position, damageCollider.radius);
    }
}