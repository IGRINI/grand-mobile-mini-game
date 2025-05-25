using UnityEngine;
using UnityEngine.Animations.Rigging;
using Zenject;

public class CharacterView : MonoBehaviour, ICharacterView
{
    private Character _model;
    private int _seatIndex = -1;
    private IEnemyService _enemyService;
    private IAttackSystem _attackSystem;
    private ITargetSelector _targetSelector;
    private IWeaponView _weaponView;
    private float _lastAttackTime;
    private float _constraintTimer;
    private bool _fadingOut;
    private bool _fadingIn;
    
    [SerializeField] private Transform weaponPivot;
    [SerializeField] private Transform hitTarget;
    [SerializeField] private Transform spineTarget;
    [SerializeField] private Transform headTarget;
    [SerializeField] private Transform leftHandTarget;
    [SerializeField] private Transform rightHandTarget;
    [SerializeField] private Transform fullAimTarget;
    [SerializeField] private float aimTargetDistance = 1f;
    [SerializeField] private float aimTargetHeightOffset = 1f;
    [SerializeField] private float modelHeightOffset = 0.5f;
    [SerializeField] private Rig mainRig;
    [SerializeField] private Rig leftRig;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform modelTransform;
    [SerializeField] private float rigFadeDuration = 1f;
    [SerializeField] private float aimTargetMoveSpeed = 5f;
    [SerializeField] private float modelRotateSpeed = 360f;
    private float _currentModelYAngle;
    private float _modelOriginalLocalY;
    private static readonly int Back = Animator.StringToHash("Back");

    public Transform WeaponPivot => weaponPivot;
    public Transform HitTarget => hitTarget;
    public Animator Animator => animator;
    public Transform ModelTransform => modelTransform;

    [Inject]
    public void Construct(IEnemyService enemyService, IAttackSystem attackSystem, ITargetSelector targetSelector)
    {
        _enemyService = enemyService;
        _attackSystem = attackSystem;
        _targetSelector = targetSelector;
    }

    public void Initialize(Character model)
    {
        _model = model;
        gameObject.name = model.Name;
        _weaponView = WeaponSpawnHelper.SpawnWeapon(weaponPivot, model.DefaultWeapon);
        _lastAttackTime = 0f;
        if (mainRig != null)
        {
            mainRig.weight = 1f;
            _constraintTimer = 0f;
            _fadingOut = false;
            _fadingIn = false;
        }
        if (leftRig != null)
            leftRig.weight = 1f;
        if (hitTarget != null)
            hitTarget.position = transform.position;
        if (modelTransform != null)
        {
            _currentModelYAngle = modelTransform.localEulerAngles.y;
            _modelOriginalLocalY = modelTransform.localPosition.y;
        }
    }

    public void SetSeatIndex(int seatIndex)
    {
        _seatIndex = seatIndex;
    }

    private void Update()
    {
        if (_model == null || !_model.Health.IsAlive) return;

        var target = _targetSelector.GetClosestAliveEnemy(transform.position);
        Vector3 targetPos = Vector3.zero; 
        bool inRange = false;
        if (target != null)
        {
            var targetView = _enemyService.GetEnemyView(target);
            targetPos = (targetView != null && targetView.HitTarget != null)
                ? targetView.HitTarget.position
                : _enemyService.GetEnemyPosition(target);
            float sqrDist = (targetPos - transform.position).sqrMagnitude;
            float sqrRange = _model.DefaultWeapon.Range * _model.DefaultWeapon.Range;
            inRange = sqrDist <= sqrRange;
        }
        // плавный поворот модели
        if (_seatIndex != 0 && modelTransform != null)
        {
            float targetYAngle = 0f;
            bool isBack = false;
            if (inRange && target != null)
            {
                Vector3 toTarget = targetPos - transform.position;
                toTarget.y = 0f;
                if (toTarget.sqrMagnitude > 0.001f)
                {
                    float signed = Vector3.SignedAngle(transform.forward, toTarget, Vector3.up);
                    if (Mathf.Abs(signed) > 90f)
                    {
                        targetYAngle = signed;
                        isBack = true;
                    }
                }
            }
            if (animator != null)
                animator.SetBool(Back, isBack);
            _currentModelYAngle = Mathf.MoveTowardsAngle(_currentModelYAngle, targetYAngle, modelRotateSpeed * Time.deltaTime);
            var angles = modelTransform.localEulerAngles;
            angles.y = _currentModelYAngle;
            modelTransform.localEulerAngles = angles;
            var pos = modelTransform.localPosition;
            float targetYPos = _modelOriginalLocalY + (isBack ? modelHeightOffset : 0f);
            pos.y = Mathf.MoveTowards(pos.y, targetYPos, modelRotateSpeed * Time.deltaTime * 0.1f);
            modelTransform.localPosition = pos;
        }
        // запускаем затухание, только если еще не в процессе и вес > 0 (или для leftRig, если он не водитель и вес > 0)
        bool needFadeOut = !inRange;
        bool mainCanFadeOut = mainRig != null && mainRig.weight > 0f;
        bool leftCanFadeOut = leftRig != null && _seatIndex != 0 && leftRig.weight > 0f;
        if (needFadeOut && !_fadingOut && (mainCanFadeOut || leftCanFadeOut))
        {
            _fadingOut = true; _fadingIn = false; _constraintTimer = 0f;
        }
        // запускаем нарастание, только если еще не в процессе и вес < 1
        bool needFadeIn = inRange;
        bool mainCanFadeIn = mainRig != null && mainRig.weight < 1f;
        bool leftCanFadeIn = leftRig != null && _seatIndex != 0 && leftRig.weight < 1f;
        if (needFadeIn && !_fadingIn && (mainCanFadeIn || leftCanFadeIn))
        {
            _fadingIn = true; _fadingOut = false; _constraintTimer = 0f;
        }
        if (_fadingOut)
        {
            _constraintTimer += Time.deltaTime;
            float t = Mathf.Clamp01(_constraintTimer / rigFadeDuration);
            if (mainRig != null)
                mainRig.weight = Mathf.Lerp(1f, 0f, t);
            if (leftRig != null)
            {
                if (_seatIndex == 0)
                    leftRig.weight = 1f;
                else
                    leftRig.weight = Mathf.Lerp(1f, 0f, t);
            }
            if (t >= 1f) _fadingOut = false;
        }
        if (_fadingIn)
        {
            _constraintTimer += Time.deltaTime;
            float t = Mathf.Clamp01(_constraintTimer / rigFadeDuration);
            if (mainRig != null)
                mainRig.weight = Mathf.Lerp(0f, 1f, t);
            if (leftRig != null)
            {
                if (_seatIndex == 0)
                    leftRig.weight = 1f;
                else
                    leftRig.weight = Mathf.Lerp(0f, 1f, t);
            }
            if (t >= 1f) _fadingIn = false;
        }

        _lastAttackTime += Time.deltaTime;

        if (leftHandTarget != null)
        {
            var carView = GetComponentInParent<ICarView>();
            if (carView != null && _seatIndex == 0)
            {
                leftHandTarget.position = carView.SteeringWheelPoint.position;
                leftHandTarget.rotation = carView.SteeringWheelPoint.rotation;
            }
            else if (_weaponView != null && _weaponView.SecondHandPoint != null)
            {
                leftHandTarget.position = _weaponView.SecondHandPoint.position;
                leftHandTarget.rotation = _weaponView.SecondHandPoint.rotation;
            }
        }

        if (target != null)
        {
            var targetPosition = targetPos;

            Vector3 dir = targetPosition - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.001f && aimTargetDistance > 0f)
            {
                var dirNormalized = dir.normalized;
                Vector3 aimPos = transform.position + dirNormalized * aimTargetDistance;
                aimPos.y = transform.position.y + aimTargetHeightOffset;
                if (spineTarget != null)
                {
                    var cur = spineTarget.position;
                    float d = (aimPos - cur).magnitude;
                    float step = d * aimTargetMoveSpeed * Time.deltaTime;
                    spineTarget.position = Vector3.MoveTowards(cur, aimPos, step);
                }
                if (headTarget != null)
                {
                    var cur = headTarget.position;
                    float d = (aimPos - cur).magnitude;
                    float step = d * aimTargetMoveSpeed * Time.deltaTime;
                    headTarget.position = Vector3.MoveTowards(cur, aimPos, step);
                }
                if (rightHandTarget != null)
                {
                    var cur = rightHandTarget.position;
                    float d = (aimPos - cur).magnitude;
                    float step = d * aimTargetMoveSpeed * Time.deltaTime;
                    rightHandTarget.position = Vector3.MoveTowards(cur, aimPos, step);
                }
            }
            if (fullAimTarget != null)
            {
                var cur = fullAimTarget.position;
                float d = (targetPosition - cur).magnitude;
                float step = d * aimTargetMoveSpeed * Time.deltaTime;
                fullAimTarget.position = Vector3.MoveTowards(cur, targetPosition, step);
            }

            if (CanAttackNow())
            {
                if (_attackSystem.CanAttack(transform, targetPosition, _model.DefaultWeapon))
                {
                    PerformAttack(target, targetPosition);
                    _lastAttackTime = 0f;
                }
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