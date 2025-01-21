using UnityEngine;

public interface IEnemy
{
    float GetMoveSpeed();
    float GetMaxHealth();
    float GetCurrentHealth();
    void TakeDamage(float damage);
    void ApplyStatusEffect(StatusEffectType effectType, float duration, float power);
    void ModifySpeed(float multiplier);
}