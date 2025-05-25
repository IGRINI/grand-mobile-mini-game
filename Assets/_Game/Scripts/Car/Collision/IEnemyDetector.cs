using System.Collections.Generic;
using UnityEngine;

public interface IEnemyDetector
{
    void RegisterEnemy(IHittable enemy);
    void UnregisterEnemy(IHittable enemy);
    IHittable GetCollidingEnemy(Vector3 position, Vector3 size, Quaternion rotation);
} 