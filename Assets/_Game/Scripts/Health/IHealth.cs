using System;

public interface IHealth
{
    float MaxHealth { get; }
    float CurrentHealth { get; }
    bool IsAlive { get; }
    float HealthPercent { get; }
    
    event Action<float> HealthChanged;
    event Action Died;
    
    void TakeDamage(float damage);
    void Heal(float amount);
    void SetMaxHealth(float maxHealth);
    void SetDamageModifier(System.Func<float, float> modifier);
} 