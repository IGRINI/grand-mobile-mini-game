using System;
using System.Collections.Generic;

public class HealthService : IHealthService
{
    private readonly Dictionary<object, IHealth> _entities = new Dictionary<object, IHealth>();
    
    public IReadOnlyDictionary<object, IHealth> RegisteredEntities => _entities;
    
    public event Action<object, IHealth> EntityRegistered;
    public event Action<object, IHealth> EntityDied;
    public event Action<object> EntityUnregistered;
    
    public void RegisterEntity(object entity, IHealth health)
    {
        if (entity == null || health == null) return;
        
        if (_entities.ContainsKey(entity))
        {
            _entities[entity].Died -= () => OnEntityDied(entity);
        }
        
        _entities[entity] = health;
        health.Died += () => OnEntityDied(entity);
        EntityRegistered?.Invoke(entity, health);
    }
    
    public void UnregisterEntity(object entity)
    {
        if (entity == null || !_entities.ContainsKey(entity)) return;
        
        var health = _entities[entity];
        health.Died -= () => OnEntityDied(entity);
        _entities.Remove(entity);
        EntityUnregistered?.Invoke(entity);
    }
    
    public IHealth GetHealth(object entity)
    {
        return entity != null && _entities.ContainsKey(entity) ? _entities[entity] : null;
    }
    
    public bool HasHealth(object entity)
    {
        return entity != null && _entities.ContainsKey(entity);
    }
    
    public void DamageEntity(object entity, float damage)
    {
        var health = GetHealth(entity);
        health?.TakeDamage(damage);
    }
    
    public void HealEntity(object entity, float amount)
    {
        var health = GetHealth(entity);
        health?.Heal(amount);
    }
    
    private void OnEntityDied(object entity)
    {
        if (!_entities.ContainsKey(entity)) return;
        
        var health = _entities[entity];
        EntityDied?.Invoke(entity, health);
    }
} 