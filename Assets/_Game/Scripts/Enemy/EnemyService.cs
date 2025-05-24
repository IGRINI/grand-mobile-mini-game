using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemyService : IEnemyService
{
    private readonly List<Enemy> _activeEnemies = new List<Enemy>();
    private readonly Dictionary<Enemy, IEnemyView> _enemyViews = new Dictionary<Enemy, IEnemyView>();
    private readonly IHealthService _healthService;
    private readonly EnemyPool _enemyPool;

    public IReadOnlyList<Enemy> ActiveEnemies => _activeEnemies;
    
    public event Action<Enemy> EnemySpawned;
    public event Action<Enemy> EnemyDied;

    public EnemyService(IHealthService healthService, EnemyPool enemyPool)
    {
        _healthService = healthService;
        _enemyPool = enemyPool;
    }

    public Enemy SpawnEnemy(EnemyData data, Vector3 position)
    {
        var enemy = new Enemy(data);
        var view = _enemyPool.Spawn(position, Quaternion.identity);
        
        view.Initialize(enemy);
        _enemyViews[enemy] = view;
        
        _activeEnemies.Add(enemy);
        _healthService.RegisterEntity(enemy, enemy.Health);
        enemy.Health.Died += () => OnEnemyDied(enemy);
        EnemySpawned?.Invoke(enemy);
        
        return enemy;
    }

    public void DespawnEnemy(Enemy enemy)
    {
        if (!_activeEnemies.Contains(enemy)) return;
        
        if (_enemyViews.TryGetValue(enemy, out var view))
        {
            _enemyPool.Despawn(view as EnemyView);
            _enemyViews.Remove(enemy);
        }
        
        _activeEnemies.Remove(enemy);
        _healthService.UnregisterEntity(enemy);
    }

    public void DamageEnemy(Enemy enemy, float damage)
    {
        if (!_activeEnemies.Contains(enemy)) return;
        _healthService.DamageEntity(enemy, damage);
    }

    public Enemy GetClosestEnemy(Vector3 position)
    {
        Enemy closest = null;
        float closestSqrDistance = float.MaxValue;
        
        foreach (var enemy in _activeEnemies)
        {
            if (!_enemyViews.TryGetValue(enemy, out var view)) continue;
            
            var sqrDistance = (view.Transform.position - position).sqrMagnitude;
            if (sqrDistance < closestSqrDistance)
            {
                closestSqrDistance = sqrDistance;
                closest = enemy;
            }
        }
        
        return closest;
    }

    public Enemy GetEnemyByView(IEnemyView view)
    {
        foreach (var kvp in _enemyViews)
        {
            if (kvp.Value == view)
                return kvp.Key;
        }
        return null;
    }

    public Vector3 GetEnemyPosition(Enemy enemy)
    {
        if (_enemyViews.TryGetValue(enemy, out var view))
        {
            return view.Transform.position;
        }
        return Vector3.zero;
    }

    private void OnEnemyDied(Enemy enemy)
    {
        EnemyDied?.Invoke(enemy);
        DespawnEnemy(enemy);
    }
}