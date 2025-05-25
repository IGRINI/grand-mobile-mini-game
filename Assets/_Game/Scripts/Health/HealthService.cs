using System;
using System.Collections.Generic;
using UnityEngine;

public class HealthService : IHealthService
{
    private readonly Dictionary<object, IHealth> _entities = new Dictionary<object, IHealth>();
    
    public IReadOnlyDictionary<object, IHealth> RegisteredEntities => _entities;
    
    public event Action<object, IHealth> EntityRegistered;
    public event Action<object, IHealth> EntityDied;
    public event Action<object> EntityUnregistered;
    
    public void RegisterEntity(object entity, IHealth health)
    {
        if (entity == null || health == null) 
        {
            Debug.Log($"HealthService: Попытка регистрации null сущности или здоровья");
            return;
        }
        
        Debug.Log($"HealthService: Регистрируем сущность {entity.GetType().Name} с HP {health.CurrentHealth:F1}/{health.MaxHealth:F1}");
        
        if (_entities.ContainsKey(entity))
        {
            Debug.Log($"Сущность {entity.GetType().Name} уже зарегистрирована, перерегистрируем");
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
        if (health != null)
        {
            Debug.Log($"HealthService: Наносим урон {damage:F1} сущности {entity.GetType().Name}");
            health.TakeDamage(damage);
        }
        else
        {
            Debug.Log($"HealthService: Не найдено здоровье для сущности {entity?.GetType().Name ?? "null"}");
        }
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
        Debug.Log($"HealthService.OnEntityDied: {entity.GetType().Name} умер!");
        EntityDied?.Invoke(entity, health);
    }
} 