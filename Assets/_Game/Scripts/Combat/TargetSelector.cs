using UnityEngine;
using System.Linq;

public class TargetSelector : ITargetSelector
{
    private readonly IEnemyService _enemyService;
    
    public TargetSelector(IEnemyService enemyService)
    {
        _enemyService = enemyService;
    }
    
    public Enemy GetClosestAliveEnemy(Vector3 position)
    {
        Enemy closest = null;
        float closestSqrDistance = float.MaxValue;
        
        foreach (var enemy in _enemyService.ActiveEnemies)
        {
            if (!enemy.IsAlive) continue;
            
            var enemyView = _enemyService.GetEnemyView(enemy);
            if (enemyView == null) continue;
            
            var hittableEnemy = enemyView.GameObject.GetComponent<HittableEnemy>();
            if (hittableEnemy != null && !hittableEnemy.CanBeHit) continue;
            
            var sqrDistance = (enemyView.Transform.position - position).sqrMagnitude;
            if (sqrDistance < closestSqrDistance)
            {
                closestSqrDistance = sqrDistance;
                closest = enemy;
            }
        }
        
        return closest;
    }
    
    public Enemy GetBestEnemyTarget(Vector3 position, Enemy currentTarget)
    {
        if (currentTarget != null && currentTarget.IsAlive)
        {
            var currentView = _enemyService.GetEnemyView(currentTarget);
            if (currentView != null)
            {
                var hittableEnemy = currentView.GameObject.GetComponent<HittableEnemy>();
                if (hittableEnemy != null && hittableEnemy.CanBeHit)
                {
                    return currentTarget;
                }
            }
        }
        
        return GetClosestAliveEnemy(position);
    }
} 