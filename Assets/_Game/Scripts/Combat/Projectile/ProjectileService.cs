using UnityEngine;
using Zenject;

public class ProjectileService : IProjectileService
{
    private readonly IHealthService _healthService;
    private readonly ProjectileFactory _projectileFactory;
    private readonly IEnemyService _enemyService;

    public ProjectileService(IHealthService healthService, ProjectileFactory projectileFactory, IEnemyService enemyService)
    {
        _healthService = healthService;
        _projectileFactory = projectileFactory;
        _enemyService = enemyService;
    }

    public void FireProjectile(Vector3 spawnPosition, Vector3 direction, float speed, float damage, GameObject projectilePrefab, float projectileLifetime)
    {
        var projectileGO = _projectileFactory.CreateProjectile(projectilePrefab, spawnPosition, Quaternion.LookRotation(direction));
        
        var timedProjectile = projectileGO.GetComponent<TimedProjectile>();
        if (timedProjectile == null)
        {
            timedProjectile = projectileGO.AddComponent<TimedProjectile>();
        }
        timedProjectile.Initialize(projectileLifetime);
        
        var projectileComponent = projectileGO.GetComponent<Projectile>();
        if (projectileComponent != null)
        {
            projectileComponent.Initialize(damage);
        }
        
        var rb = projectileGO.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = direction * speed;
        }
    }
} 