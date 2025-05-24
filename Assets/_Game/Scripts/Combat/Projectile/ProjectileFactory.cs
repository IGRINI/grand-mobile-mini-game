using UnityEngine;
using Zenject;

public class ProjectileFactory
{
    private readonly ProjectilePool _projectilePool;

    public ProjectileFactory(ProjectilePool projectilePool)
    {
        _projectilePool = projectilePool;
    }

    public GameObject CreateProjectile(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        var projectile = _projectilePool.GetProjectile(prefab);
        projectile.transform.position = position;
        projectile.transform.rotation = rotation;
        return projectile;
    }
} 