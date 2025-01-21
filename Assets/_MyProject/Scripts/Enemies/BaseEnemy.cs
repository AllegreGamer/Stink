using UnityEngine;
using System.Collections.Generic;

public abstract class BaseEnemy : MonoBehaviour, IEnemy
{
    [Header("Base Stats")]
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected float baseSpeed = 1.5f;
    [SerializeField] protected float attackDamage = 10f;
    [SerializeField] protected float attackInterval = 1f;

    protected float currentHealth;
    protected float currentSpeed;
    protected float nextAttackTime;

    // Aggiungiamo il flag isDying
    private bool isDying = false;

    // Aggiunti moltiplicatori
    protected float healthMultiplier = 1f;
    protected float damageMultiplier = 1f;
    protected float speedMultiplier = 1f;

    protected Dictionary<StatusEffectType, (float duration, float power)> activeEffects =
        new Dictionary<StatusEffectType, (float duration, float power)>();

    protected virtual void Start()
    {
        // Applica i moltiplicatori alle statistiche base
        maxHealth *= healthMultiplier;
        baseSpeed *= speedMultiplier;
        attackDamage *= damageMultiplier;

        currentHealth = maxHealth;
        currentSpeed = baseSpeed;
    }

    // Aggiunto metodo per settare i moltiplicatori
    public virtual void SetMultipliers(float health, float damage, float speed)
    {
        this.healthMultiplier = health;
        this.damageMultiplier = damage;
        this.speedMultiplier = speed;
    }

    protected virtual void Update()
    {
        UpdateStatusEffects();
    }

    public virtual float GetMoveSpeed() => currentSpeed;
    public virtual float GetMaxHealth() => maxHealth;
    public virtual float GetCurrentHealth() => currentHealth;

    public virtual void ModifySpeed(float multiplier)
    {
        currentSpeed = baseSpeed * multiplier;
    }

    public virtual void ResetSpeed()
    {
        currentSpeed = baseSpeed;
    }

    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0 && !isDying)
        {
            isDying = true;
            Die();
        }
    }

    public void ApplyStatusEffect(StatusEffectType effectType, float duration, float power)
    {
        if (effectType == StatusEffectType.None) return;

        if (activeEffects.ContainsKey(effectType))
        {
            var currentEffect = activeEffects[effectType];
            activeEffects[effectType] = (
                Mathf.Max(currentEffect.duration, duration),
                Mathf.Max(currentEffect.power, power)
            );
        }
        else
        {
            activeEffects.Add(effectType, (duration, power));
        }
    }

    private void UpdateStatusEffects()
    {
        List<StatusEffectType> effectsToRemove = new List<StatusEffectType>();
        foreach (var effect in activeEffects)
        {
            var effectType = effect.Key;
            var duration = effect.Value.duration - Time.deltaTime;
            var power = effect.Value.power;

            if (duration <= 0)
            {
                effectsToRemove.Add(effectType);
                continue;
            }

            switch (effectType)
            {
                case StatusEffectType.Poison:
                    TakeDamage(power * Time.deltaTime);
                    break;
                case StatusEffectType.Fire:
                    TakeDamage(power * Time.deltaTime * 1.5f);
                    break;
                case StatusEffectType.Slow:
                    ModifySpeed(1f - (power / 100f));
                    break;
                case StatusEffectType.Stun:
                    ModifySpeed(0f);
                    break;
            }

            activeEffects[effectType] = (duration, power);
        }

        foreach (var effect in effectsToRemove)
        {
            if (effect == StatusEffectType.Slow || effect == StatusEffectType.Stun)
            {
                ResetSpeed();
            }
            activeEffects.Remove(effect);
        }
    }

    protected virtual void Die()
    {
        if (!isDying) return;  // Controllo aggiuntivo

        DropXPGem();
        DropTear();
        Destroy(gameObject);
    }

    protected virtual void DropXPGem()
    {
        XPGemDropper gemDropper = GetComponent<XPGemDropper>();
        if (gemDropper != null)
        {
            gemDropper.DropRandomGem();
        }
    }

    protected virtual void DropTear()
    {
        TearDropper tearDropper = GetComponent<TearDropper>();
        if (tearDropper != null)
        {
            tearDropper.DropTear();
        }
    }

    public virtual void DealDamageToPlayer(Collider other)
    {
        if (Time.time >= nextAttackTime)
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
                nextAttackTime = Time.time + attackInterval;
            }
        }
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DealDamageToPlayer(other);
        }
    }
}