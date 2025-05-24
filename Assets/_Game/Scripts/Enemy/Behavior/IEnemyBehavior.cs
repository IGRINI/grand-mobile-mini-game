using UnityEngine;

public interface IEnemyBehavior
{
    void Initialize(Enemy enemy, Transform target);
    void SetTransform(Transform transform);
    void SetAimTargets(Transform spineTarget, Transform headTarget);
    void SetHandTargets(Transform leftHandTarget, Transform rightHandTarget);
    void SetWeaponView(IWeaponView weaponView);
    void SetTurnParameters(float thresholdAngle, float rotationSpeed);
    void SetAimTargetDistance(float distance);
    void Update(float deltaTime);
    Vector3 GetNextPosition();
    bool CanAttack();
    void ResetAttackCooldown();
    void SetAimTargetHeightOffset(float offset);
    void SetFullAimTarget(Transform fullAimTarget);
} 