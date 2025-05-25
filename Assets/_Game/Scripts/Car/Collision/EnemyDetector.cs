using System.Collections.Generic;
using UnityEngine;

public class EnemyDetector : IEnemyDetector
{
    private readonly List<IHittable> _enemies = new List<IHittable>();

    public void RegisterEnemy(IHittable enemy)
    {
        if (!_enemies.Contains(enemy))
        {
            _enemies.Add(enemy);
        }
    }

    public void UnregisterEnemy(IHittable enemy)
    {
        _enemies.Remove(enemy);
    }

    public IHittable GetCollidingEnemy(Vector3 position, Vector3 size, Quaternion rotation)
    {
        float carRadius = Mathf.Max(size.x, size.z) * 0.5f;
        
        foreach (var enemy in _enemies)
        {
            var mb = enemy as MonoBehaviour;
            if (mb == null || !mb.gameObject.activeInHierarchy) continue;

            Vector3 enemyPos = mb.transform.position + enemy.CollisionCenterOffset;
            float totalRadius = carRadius + enemy.CollisionRadius;
            if ((position - enemyPos).sqrMagnitude <= totalRadius * totalRadius)
            {
                return enemy;
            }
        }
        
        return null;
    }
} 