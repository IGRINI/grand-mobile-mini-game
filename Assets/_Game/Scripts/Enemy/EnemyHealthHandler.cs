using System;
using Zenject;

public class EnemyHealthHandler : IEnemyHealthHandler
{
    private readonly IHealthService _healthService;
    
    public event Action<Enemy> EnemyDied;
    
    public EnemyHealthHandler(IHealthService healthService)
    {
        _healthService = healthService;
    }
    
    public void TakeDamage(Enemy enemy, float damage)
    {
        if (enemy?.Health == null) return;
        
        bool wasAlive = enemy.Health.IsAlive;
        enemy.Health.TakeDamage(damage);
        
        if (wasAlive && !enemy.Health.IsAlive)
        {
            EnemyDied?.Invoke(enemy);
        }
    }
    
    public void Heal(Enemy enemy, float amount)
    {
        enemy?.Health?.Heal(amount);
    }
    
    public bool IsAlive(Enemy enemy)
    {
        return enemy?.Health?.IsAlive ?? false;
    }
    
    public float GetCurrentHealth(Enemy enemy)
    {
        return enemy?.Health?.CurrentHealth ?? 0f;
    }
    
    public float GetMaxHealth(Enemy enemy)
    {
        return enemy?.Health?.MaxHealth ?? 0f;
    }
} 