using UnityEngine;
using Zenject;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;

public class EnemyView : MonoBehaviour, IEnemyView, IPoolable<Vector3, Quaternion>
{
    private Enemy _model;
    private IEnemyBehavior _behavior;
    private ICarView _carView;
    private IAttackSystem _attackSystem;
    private IWeaponView _weaponView;
    
    [SerializeField] private Transform weaponPivot;
    [SerializeField] private Transform healthBarBackground;
    [SerializeField] private Transform healthBarForeground;
    [SerializeField] private Transform spineTarget;
    [SerializeField] private Transform headTarget;
    [SerializeField] private Transform leftHandTarget;
    [SerializeField] private Transform rightHandTarget;
    [SerializeField] private Transform fullAimTarget;
    [SerializeField] private float turnThresholdAngle = 180f;
    [SerializeField] private float turnRotationSpeed = 360f;
    [SerializeField] private float aimTargetDistance = 1f;
    [SerializeField] private float aimTargetHeightOffset = 1f;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform hitTarget;
    [FormerlySerializedAs("rig")] [SerializeField] private Rig handsRig;
    [SerializeField] private float rigFadeDuration = 1f;
    [SerializeField] private float firstShotDelay = 0.5f;
    
    private Vector3 _previousPosition;
    private static readonly int Run = Animator.StringToHash("Run");
    private bool _wasMoving;
    private bool _fadingOut;
    private bool _fadingIn;
    private float _constraintTimer;
    private float _firstShotTimer;
    private bool _firstShotAllowed;

    public Transform Transform => transform;
    public GameObject GameObject => gameObject;
    public Transform WeaponPivot => weaponPivot;
    public Transform HitTarget => hitTarget;

    [Inject]
    public void Construct(ICarView carView, IAttackSystem attackSystem)
    {
        _carView = carView;
        _attackSystem = attackSystem;
    }

    public void Initialize(Enemy enemy)
    {
        _model = enemy;
        gameObject.name = enemy.Name;
        _weaponView = WeaponSpawnHelper.SpawnWeapon(weaponPivot, enemy.DefaultWeapon);
        enemy.Health.HealthChanged += OnHealthChanged;
        UpdateHealthBar(enemy.Health.HealthPercent);
        
        _behavior = new FollowPlayerAgentBehavior();
        var aimPoint = (_carView != null && _carView.HitTarget != null) ? _carView.HitTarget : _carView?.Transform;
        _behavior.Initialize(_model, aimPoint);
        _behavior.SetTransform(transform);
        
        if (spineTarget != null && headTarget != null)
        {
            _behavior.SetAimTargets(spineTarget, headTarget);
        }
        
        if (leftHandTarget != null && rightHandTarget != null)
        {
            _behavior.SetHandTargets(leftHandTarget, rightHandTarget);
        }
        
        if (_weaponView != null)
            _behavior.SetWeaponView(_weaponView);
        
        _behavior.SetTurnParameters(turnThresholdAngle, turnRotationSpeed);
        _behavior.SetAimTargetDistance(aimTargetDistance);
        _behavior.SetAimTargetHeightOffset(aimTargetHeightOffset);
        if (fullAimTarget != null) _behavior.SetFullAimTarget(fullAimTarget);
        if (handsRig != null)
            handsRig.weight = 1f;
        _wasMoving = false;
        _fadingOut = false;
        _fadingIn = false;
        _constraintTimer = 0f;
        _firstShotTimer = 0f;
        _firstShotAllowed = true;
        _previousPosition = transform.position;
    }

    private void Update()
    {
        if (_model == null || !_model.IsAlive || _carView?.Transform == null) return;

        _behavior.Update(Time.deltaTime);
        Vector3 currentPosition = transform.position;
        bool isMoving = (currentPosition - _previousPosition).sqrMagnitude > 0.0001f;

        if (isMoving && !_wasMoving)
        {
            _fadingOut = true;
            _fadingIn = false;
            _constraintTimer = 0f;
        }
        else if (!isMoving && _wasMoving)
        {
            _fadingOut = false;
            _fadingIn = true;
            _constraintTimer = 0f;
            _firstShotTimer = 0f;
            _firstShotAllowed = false;
        }
        _wasMoving = isMoving;

        if (_fadingOut && handsRig != null)
        {
            _constraintTimer += Time.deltaTime;
            float t = Mathf.Clamp01(_constraintTimer / rigFadeDuration);
            float weight = Mathf.Lerp(1f, 0f, t);
            handsRig.weight = weight;
            if (t >= 1f) _fadingOut = false;
        }

        if (_fadingIn && handsRig != null)
        {
            _constraintTimer += Time.deltaTime;
            float t = Mathf.Clamp01(_constraintTimer / rigFadeDuration);
            float weight = Mathf.Lerp(0f, 1f, t);
            handsRig.weight = weight;
            if (t >= 1f) _fadingIn = false;
        }

        if (!_firstShotAllowed && !isMoving)
        {
            _firstShotTimer += Time.deltaTime;
            if (_firstShotTimer >= firstShotDelay)
                _firstShotAllowed = true;
        }

        if (animator != null)
            animator.SetBool(Run, isMoving);
        _previousPosition = currentPosition;

        if (_behavior.CanAttack() && _firstShotAllowed)
        {
            PerformAttack();
            _behavior.ResetAttackCooldown();
        }
    }

    private void PerformAttack()
    {
        var targetPosition = (_carView.HitTarget != null) ? _carView.HitTarget.position : _carView.Transform.position;
        
        if (_attackSystem.CanAttack(transform, targetPosition, _model.DefaultWeapon))
        {
            _attackSystem.Attack(transform, _weaponView, targetPosition, _model.DefaultWeapon, _carView);
        }
    }

    public void UpdateHealthBar(float healthPercent)
    {
        if (healthBarForeground != null)
        {
            var scale = healthBarForeground.localScale;
            scale.x = Mathf.Clamp01(healthPercent);
            healthBarForeground.localScale = scale;
        }
    }

    private void OnHealthChanged(float currentHealth)
    {
        if (_model != null)
        {
            UpdateHealthBar(_model.Health.HealthPercent);
        }
    }

    private void OnDestroy()
    {
        if (_model != null)
        {
            _model.Health.HealthChanged -= OnHealthChanged;
        }
    }

    public void OnSpawned(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
        gameObject.SetActive(true);
        if (weaponPivot != null && _model != null)
        {
            _weaponView = WeaponSpawnHelper.SpawnWeapon(weaponPivot, _model.DefaultWeapon);
            if (_weaponView != null)
                _behavior.SetWeaponView(_weaponView);
        }
    }

    public void OnDespawned()
    {
        gameObject.SetActive(false);
    }
}