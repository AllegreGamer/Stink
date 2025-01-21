using UnityEngine;

public class TankToilet : BaseEnemy
{
    protected override void Start()
    {
        // Statistiche personalizzate per il nemico tank
        maxHealth = 100f;       // Vita molto alta
        baseSpeed = 0.8f;       // Movimento lento
        attackDamage = 20f;     // Danno elevato
        attackInterval = 2f;     // Attacca lentamente

        base.Start();
    }

    // Esempio di comportamento speciale
    public override void DealDamageToPlayer(Collider other)
    {
        base.DealDamageToPlayer(other);

        // Aggiunge un effetto di spinta quando colpisce il player
        if (other.CompareTag("Player"))
        {
            Vector3 pushDirection = (other.transform.position - transform.position).normalized;
            other.attachedRigidbody?.AddForce(pushDirection * 5f, ForceMode.Impulse);
        }
    }
}