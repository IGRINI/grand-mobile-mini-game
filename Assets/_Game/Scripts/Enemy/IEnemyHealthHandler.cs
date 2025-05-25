using System;

public interface IEnemyHealthHandler
{
    event Action<Enemy> EnemyDied;
    void TakeDamage(Enemy enemy, float damage);
    void Heal(Enemy enemy, float amount);
    bool IsAlive(Enemy enemy);
    float GetCurrentHealth(Enemy enemy);
    float GetMaxHealth(Enemy enemy);
} 