using System;
using System.Collections.Generic;

public interface IHealthService
{
    IReadOnlyDictionary<object, IHealth> RegisteredEntities { get; }
    
    event Action<object, IHealth> EntityRegistered;
    event Action<object, IHealth> EntityDied;
    event Action<object> EntityUnregistered;
    
    void RegisterEntity(object entity, IHealth health);
    void UnregisterEntity(object entity);
    IHealth GetHealth(object entity);
    bool HasHealth(object entity);
    void DamageEntity(object entity, float damage);
    void HealEntity(object entity, float amount);
} 