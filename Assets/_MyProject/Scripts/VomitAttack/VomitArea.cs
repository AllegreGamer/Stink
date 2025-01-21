using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using GameCreator.Runtime.Common;
using System.Collections.Generic;

public class VomitArea : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackDuration = 1f;

    [Header("References")]
    [SerializeField] private GameObject damageNumberPrefab;
    [SerializeField] private GameObject vomitSplashPrefab;
    [SerializeField] private float splashDuration = 5f;

    private float damage;
    private float radius;
    private SphereCollider areaCollider;

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

        // Applica il danno immediatamente
        ApplyDamageInArea();

        // Crea l'effetto macchia
        CreateVomitSplash();

        // Distruggi l'oggetto dopo la durata specificata
        Destroy(gameObject, attackDuration);
    }

    private void ApplyDamageInArea()
    {
        // Usa OverlapSphere per trovare tutti i nemici nell'area frontale
        Vector3 forward = transform.forward;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider col in hitColliders)
        {
            // Verifica se il target è nella direzione frontale
            Vector3 directionToTarget = (col.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(forward, directionToTarget);

            // Se il target è entro un angolo di 90 gradi (45 gradi per lato)
            if (angle <= 45f)
            {
                EnemyTarget target = col.GetComponent<EnemyTarget>();
                if (target != null)
                {
                    ApplyDamage(col.gameObject);
                }
            }
        }
    }

    private void ApplyDamage(GameObject targetObject)
    {
        EnemyTarget target = targetObject.GetComponent<EnemyTarget>();
        IEnemy enemy = targetObject.GetComponentInParent<IEnemy>();

        if (target != null && enemy != null)
        {
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

            // Applica danno
            enemy.TakeDamage(actualDamage);

            // Applica l'effetto vomito
            DamageVisualEffect effect = target.GetComponentInParent<DamageVisualEffect>();
            if (effect != null)
            {
                effect.ApplyEffect(AttackType.Vomit); // o il tipo appropriato
            }
        }
    }

    private void CreateVomitSplash()
    {
        // Definisci un LayerMask per il layer "Ground"
        int groundLayer = LayerMask.GetMask("Ground");

        // Determina la posizione davanti al player
        Vector3 forwardPosition = transform.position + transform.forward * radius * 1f;

        // Effettua un Raycast solo sul layer "Ground"
        RaycastHit hit;
        if (Physics.Raycast(forwardPosition, Vector3.down, out hit, Mathf.Infinity, groundLayer))
        {
            // Se il Raycast colpisce qualcosa nel layer "Ground", procedi
            Vector3 splashPosition = hit.point + Vector3.up * 0.01f;  // Ridotto l'offset a 0.001f

            GameObject splash = Instantiate(vomitSplashPrefab, splashPosition, Quaternion.Euler(90, 0, 0));
            splash.transform.localScale = Vector3.one * (radius * 2); // Dimensione basata sul raggio

            // Usa il SplashEffect per il fade
            var splashEffect = splash.GetComponent<SplashEffect>();
            if (splashEffect != null)
            {
                splashEffect.StartFade(splashDuration);
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
        Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
        Gizmos.DrawSphere(transform.position, radius);

        // Disegna il cono di attacco
        Vector3 forward = transform.forward;
        Vector3 right = Quaternion.Euler(0, 45, 0) * forward;
        Vector3 left = Quaternion.Euler(0, -45, 0) * forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, forward * radius);
        Gizmos.DrawRay(transform.position, right * radius);
        Gizmos.DrawRay(transform.position, left * radius);
    }
#endif
}