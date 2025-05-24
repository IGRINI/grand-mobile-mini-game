using UnityEngine;

public interface IEnemyView
{
    Transform Transform { get; }
    GameObject GameObject { get; }
    Transform WeaponPivot { get; }
    Transform HitTarget { get; }
    
    void Initialize(Enemy enemy);
    void UpdateHealthBar(float healthPercent);
} 