using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ProjectilePool : IFixedTickable
{
    private readonly Dictionary<string, Queue<GameObject>> _pools = new Dictionary<string, Queue<GameObject>>();
    private readonly Dictionary<string, GameObject> _prefabs = new Dictionary<string, GameObject>();
    private readonly HashSet<GameObject> _activeProjectiles = new HashSet<GameObject>();
    private readonly DiContainer _container;

    public ProjectilePool(DiContainer container)
    {
        _container = container;
    }

    // Zenject IFixedTickable
    public void FixedTick()
    {
        CheckAndReturnExpiredProjectiles();
    }

    public GameObject GetProjectile(GameObject prefab)
    {
        string key = prefab.name;
        
        if (!_pools.ContainsKey(key))
        {
            _pools[key] = new Queue<GameObject>();
            _prefabs[key] = prefab;
        }

        if (_pools[key].Count > 0)
        {
            var pooled = _pools[key].Dequeue();
            if (pooled != null)
            {
                pooled.SetActive(true);
                ResetProjectile(pooled);
                _activeProjectiles.Add(pooled);
                return pooled;
            }
        }

        var newProjectile = CreateNewProjectile(prefab);
        _activeProjectiles.Add(newProjectile);
        return newProjectile;
    }

    public void ReturnProjectile(GameObject projectile)
    {
        if (projectile == null) return;
        
        _activeProjectiles.Remove(projectile);
        projectile.SetActive(false);
        string key = GetPrefabKey(projectile);
        
        if (!string.IsNullOrEmpty(key) && _pools.ContainsKey(key))
        {
            _pools[key].Enqueue(projectile);
        }
        else
        {
            Object.Destroy(projectile);
        }
    }

    public void CheckAndReturnExpiredProjectiles()
    {
        var expiredProjectiles = new List<GameObject>();
        
        foreach (var projectile in _activeProjectiles)
        {
            if (projectile == null) continue;
            
            var timedProjectile = projectile.GetComponent<TimedProjectile>();
            if (timedProjectile != null && timedProjectile.IsExpired)
            {
                expiredProjectiles.Add(projectile);
                continue;
            }
            
            var regularProjectile = projectile.GetComponent<Projectile>();
            if (regularProjectile != null && regularProjectile.IsExpired)
            {
                expiredProjectiles.Add(projectile);
            }
        }
        
        foreach (var expired in expiredProjectiles)
        {
            ReturnProjectile(expired);
        }
    }

    private GameObject CreateNewProjectile(GameObject prefab)
    {
        var instance = _container.InstantiatePrefab(prefab);
        
        var poolableProjectile = instance.GetComponent<PoolableProjectile>();
        if (poolableProjectile == null)
        {
            poolableProjectile = instance.AddComponent<PoolableProjectile>();
        }
        poolableProjectile.SetPool(this);
        poolableProjectile.SetPrefabKey(prefab.name);
        
        return instance;
    }

    private void ResetProjectile(GameObject projectile)
    {
        var rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        var timedProjectile = projectile.GetComponent<TimedProjectile>();
        if (timedProjectile != null)
        {
            timedProjectile.Initialize();
        }

        var regularProjectile = projectile.GetComponent<Projectile>();
        if (regularProjectile != null)
        {
            regularProjectile.Initialize(0f);
        }
    }

    private string GetPrefabKey(GameObject projectile)
    {
        var poolable = projectile.GetComponent<PoolableProjectile>();
        return poolable?.PrefabKey;
    }
} 