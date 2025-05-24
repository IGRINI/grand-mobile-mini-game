using System;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyService
{
    IReadOnlyList<Enemy> ActiveEnemies { get; }
    event Action<Enemy> EnemySpawned;
    event Action<Enemy> EnemyDied;
    
    Enemy SpawnEnemy(EnemyData data, Vector3 position);
    void DespawnEnemy(Enemy enemy);
    void DamageEnemy(Enemy enemy, float damage);
    Enemy GetClosestEnemy(Vector3 position);    
    Enemy GetEnemyByView(IEnemyView view);    
    Vector3 GetEnemyPosition(Enemy enemy);
    IEnemyView GetEnemyView(Enemy enemy);
} 