using System;
using UnityEngine;

public class Health : IHealth
{
    private float _maxHealth;
    private float _currentHealth;
    private System.Func<float, float> _damageModifier;
    
    public float MaxHealth => _maxHealth;
    public float CurrentHealth => _currentHealth;
    public bool IsAlive => _currentHealth > 0f;
    public float HealthPercent => _maxHealth > 0f ? _currentHealth / _maxHealth : 0f;
    
    public event Action<float> HealthChanged;
    public event Action Died;
    
    public Health(float maxHealth)
    {
        _maxHealth = maxHealth;
        _currentHealth = maxHealth;
    }
    
    public void SetDamageModifier(System.Func<float, float> modifier)
    {
        _damageModifier = modifier;
    }
    
    public void TakeDamage(float damage)
    {
        if (!IsAlive) return;
        
        float modifiedDamage = _damageModifier?.Invoke(damage) ?? damage;
        
        _currentHealth = Mathf.Max(0f, _currentHealth - modifiedDamage);
        HealthChanged?.Invoke(_currentHealth);
        
        if (!IsAlive)
        {
            Died?.Invoke();
        }
    }
    
    public void Heal(float amount)
    {
        if (!IsAlive) return;
        
        _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);
        HealthChanged?.Invoke(_currentHealth);
    }
    
    public void SetMaxHealth(float maxHealth)
    {
        _maxHealth = maxHealth;
        _currentHealth = Mathf.Min(_currentHealth, _maxHealth);
        HealthChanged?.Invoke(_currentHealth);
    }
} 