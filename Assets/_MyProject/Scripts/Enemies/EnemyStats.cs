using UnityEngine;

public abstract class EnemyStats : MonoBehaviour
{
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float moveSpeed;

    protected float currentHealth;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    public float GetMoveSpeed()
    {
        return moveSpeed;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        // Implementare la logica di morte specifica per ogni nemico
        Destroy(gameObject);
    }
}