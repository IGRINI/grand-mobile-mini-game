using UnityEngine;

public interface IProjectileService
{
    // Выпускает снаряд и возвращает GameObject снаряда
    GameObject FireProjectile(Vector3 spawnPosition, Vector3 direction, float speed, float damage, GameObject projectilePrefab, float projectileLifetime);
} 