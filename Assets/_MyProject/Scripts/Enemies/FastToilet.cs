using UnityEngine;

public class FastToilet : BaseEnemy
{
    protected override void Start()
    {
        // Statistiche personalizzate per il nemico veloce
        maxHealth = 70f;        // Vita base ridotta
        baseSpeed = 1.3f;       // Velocit� aumentata
        attackDamage = 5f;      // Danno ridotto
        attackInterval = 0.5f;   // Attacca pi� frequentemente

        // IMPORTANTE: chiama lo Start della classe base
        base.Start();
    }
}