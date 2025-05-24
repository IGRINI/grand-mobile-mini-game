using UnityEngine;

public interface IProjectileService
{
    void FireProjectile(Vector3 spawnPosition, Vector3 direction, float speed, float damage, GameObject projectilePrefab, float projectileLifetime);
} 