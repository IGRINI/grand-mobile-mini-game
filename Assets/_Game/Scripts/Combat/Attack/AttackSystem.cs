using UnityEngine;
using Zenject;

public class AttackSystem : IAttackSystem
{
    private readonly IProjectileService _projectileService;

    public AttackSystem(IProjectileService projectileService)
    {
        _projectileService = projectileService;
    }

    public bool CanAttack(Transform attacker, Vector3 targetPosition, WeaponData weaponData)
    {
        if (attacker == null || weaponData == null) return false;
        
        float sqrDistance = (targetPosition - attacker.position).sqrMagnitude;
        float sqrRange = weaponData.Range * weaponData.Range;
        
        return sqrDistance <= sqrRange;
    }

    public void Attack(Transform attackerTransform, IWeaponView weaponView, Vector3 targetPosition, WeaponData weaponData, object target = null)
    {
        if (weaponData?.ProjectilePrefab == null) return;
        
        Transform spawnPoint = weaponView != null && weaponView.SpawnPoint != null ? weaponView.SpawnPoint : attackerTransform;
        if (spawnPoint == null) return;
        var spawnPos = spawnPoint.position;
        var direction = (targetPosition - spawnPos).normalized;
        
        _projectileService.FireProjectile(
            spawnPos,
            direction,
            weaponData.ProjectileSpeed,
            weaponData.Damage,
            weaponData.ProjectilePrefab,
            weaponData.ProjectileLifetime
        );
    }
} 