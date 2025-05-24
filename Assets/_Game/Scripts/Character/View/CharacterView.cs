using UnityEngine;
using Zenject;

public class CharacterView : MonoBehaviour, ICharacterView
{
    private Character _model;
    private IEnemyService _enemyService;
    private IAttackSystem _attackSystem;
    private IWeaponView _weaponView;
    private float _lastAttackTime;
    
    [SerializeField] private Transform weaponPivot;
    [SerializeField] private Transform hitTarget;

    public Transform WeaponPivot => weaponPivot;
    public Transform HitTarget => hitTarget;

    [Inject]
    public void Construct(IEnemyService enemyService, IAttackSystem attackSystem)
    {
        _enemyService = enemyService;
        _attackSystem = attackSystem;
    }

    public void Initialize(Character model)
    {
        _model = model;
        gameObject.name = model.Name;
        WeaponSpawnHelper.SpawnWeapon(weaponPivot, model.DefaultWeapon);
        _weaponView = GetComponentInChildren<IWeaponView>();
        _lastAttackTime = 0f;
        if (hitTarget != null)
            hitTarget.position = transform.position;
    }

    private void Update()
    {
        if (_model == null || !_model.Health.IsAlive) return;

        _lastAttackTime += Time.deltaTime;

        var target = _enemyService.GetClosestEnemy(transform.position);
        if (target != null && CanAttackNow())
        {
            var targetPosition = target is IEnemyView ev && ev.HitTarget != null ? ev.HitTarget.position : _enemyService.GetEnemyPosition(target);
            
            if (_attackSystem.CanAttack(transform, targetPosition, _model.DefaultWeapon))
            {
                PerformAttack(target, targetPosition);
                _lastAttackTime = 0f;
            }
        }
    }

    private bool CanAttackNow()
    {
        var fireRate = _model.DefaultWeapon?.FireRate ?? 1f;
        var interval = 1f / fireRate;
        return _lastAttackTime >= interval;
    }

    private void PerformAttack(Enemy target, Vector3 targetPosition)
    {
        _attackSystem.Attack(transform, _weaponView, targetPosition, _model.DefaultWeapon, target);
    }
} 