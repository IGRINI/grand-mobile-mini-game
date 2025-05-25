using UnityEngine;
using Zenject;

public class AttackSystem : IAttackSystem
{
    private readonly IProjectileService _projectileService;
    private readonly IHealthService _healthService;
    private IUpgradeEffectProvider _upgradeEffectProvider;

    public AttackSystem(IProjectileService projectileService, IHealthService healthService)
    {
        _projectileService = projectileService;
        _healthService = healthService;
    }
    
    [Inject]
    public void Construct(IUpgradeEffectProvider upgradeEffectProvider)
    {
        _upgradeEffectProvider = upgradeEffectProvider;
    }

    public bool CanAttack(Transform attacker, Vector3 targetPosition, WeaponData weaponData)
    {
        if (attacker == null || weaponData == null) return false;
        
        float range = weaponData.Range;
        if (_upgradeEffectProvider != null)
        {
            float rangeMultiplier = _upgradeEffectProvider.GetSpeedMultiplier(UpgradeType.IncreaseRange);
            range *= rangeMultiplier;
        }
        
        float sqrDistance = (targetPosition - attacker.position).sqrMagnitude;
        float sqrRange = range * range;
        
        return sqrDistance <= sqrRange;
    }

    public void Attack(Transform attackerTransform, IWeaponView weaponView, Vector3 targetPosition, WeaponData weaponData, object target = null)
    {
        if (weaponData?.ProjectilePrefab == null) return;
        
        Transform spawnPoint = weaponView != null && weaponView.SpawnPoint != null ? weaponView.SpawnPoint : attackerTransform;
        if (spawnPoint == null) return;
        var spawnPos = spawnPoint.position;
        var direction = (targetPosition - spawnPos).normalized;
        
        float damage = weaponData.Damage;
        
        if (_upgradeEffectProvider != null)
        {
            float damageMultiplier = _upgradeEffectProvider.GetDamageMultiplier(UpgradeType.IncreaseCharacterDamage);
            damage *= damageMultiplier;
            
            float critChance = _upgradeEffectProvider.GetCriticalChance();
            if (Random.Range(0f, 100f) < critChance)
            {
                float critMultiplier = _upgradeEffectProvider.GetCriticalDamageMultiplier();
                damage *= critMultiplier;
                Debug.Log($"Критический удар! Урон: {damage:F1}");
            }
        }
        
        var projectileGO = _projectileService.FireProjectile(
            spawnPos,
            direction,
            weaponData.ProjectileSpeed,
            damage,
            weaponData.ProjectilePrefab,
            weaponData.ProjectileLifetime
        );
        
        float distance = Vector3.Distance(spawnPos, targetPosition);
        float travelTime = distance / weaponData.ProjectileSpeed;
        var scheduler = projectileGO.AddComponent<ProjectileDamageScheduler>();
        scheduler.Construct(_healthService);
        Debug.Log($"AttackSystem: Создаем снаряд с целью {target?.GetType().Name ?? "null"} и уроном {damage:F1}");
        scheduler.Initialize(target, damage, travelTime);
    }
} 