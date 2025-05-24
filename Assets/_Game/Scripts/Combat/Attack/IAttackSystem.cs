using UnityEngine;

public interface IAttackSystem
{
    bool CanAttack(Transform attacker, Vector3 targetPosition, WeaponData weaponData);
    void Attack(Transform attackerTransform, IWeaponView weaponView, Vector3 targetPosition, WeaponData weaponData, object target = null);
} 