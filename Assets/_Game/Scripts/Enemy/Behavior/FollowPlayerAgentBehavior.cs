using UnityEngine;
using UnityEngine.AI;

public class FollowPlayerAgentBehavior : IEnemyBehavior
{
    private Enemy _enemy;
    private Transform _target;
    private Transform _transform;
    private NavMeshAgent _agent;
    private float _lastAttackTime;
    private float _pathUpdateTimer;
    private Transform _spineTarget;
    private Transform _headTarget;
    private Transform _leftHandTarget;
    private Transform _rightHandTarget;
    private IWeaponView _weaponView;
    private float _turnThresholdAngle = 180f;
    private float _turnRotationSpeed = 360f;
    private float _aimTargetDistance = 0f;
    private float _aimTargetHeightOffset = 0f;
    private Transform _fullAimTarget;

    public void Initialize(Enemy enemy, Transform target)
    {
        _enemy = enemy;
        _target = target;
        _lastAttackTime = 0f;
        _pathUpdateTimer = 0f;
    }
    
    public void UpdateTarget(Transform newTarget)
    {
        _target = newTarget;
    }

    public void SetTransform(Transform transform)
    {
        _transform = transform;
        _agent = _transform.GetComponent<NavMeshAgent>();
        if (_agent == null)
        {
            _agent = _transform.gameObject.AddComponent<NavMeshAgent>();
        }

        if (_enemy != null)
        {
            _agent.speed = _enemy.MoveSpeed;
            _agent.stoppingDistance = _enemy.AttackRange;
            _agent.acceleration = _enemy.MoveSpeed * 2; // Примерное значение, можно вынести в EnemyData
            _agent.angularSpeed = 360; // Примерное значение, можно вынести в EnemyData
        }
        _agent.updateRotation = false;
    }

    public void SetAimTargets(Transform spineTarget, Transform headTarget)
    {
        _spineTarget = spineTarget;
        _headTarget = headTarget;
    }

    public void SetHandTargets(Transform leftHandTarget, Transform rightHandTarget)
    {
        _leftHandTarget = leftHandTarget;
        _rightHandTarget = rightHandTarget;
    }

    public void SetWeaponView(IWeaponView weaponView)
    {
        _weaponView = weaponView;
    }

    public void SetTurnParameters(float thresholdAngle, float rotationSpeed)
    {
        _turnThresholdAngle = thresholdAngle;
        _turnRotationSpeed = rotationSpeed;
    }

    public void SetAimTargetDistance(float distance)
    {
        _aimTargetDistance = distance;
    }

    public void SetAimTargetHeightOffset(float offset)
    {
        _aimTargetHeightOffset = offset;
    }

    public void SetFullAimTarget(Transform fullAimTarget)
    {
        _fullAimTarget = fullAimTarget;
    }

    public void Update(float deltaTime)
    {
        if (_fullAimTarget != null && _target != null)
            _fullAimTarget.position = _target.position;
        _lastAttackTime += deltaTime;
        _pathUpdateTimer += deltaTime;
        if (_pathUpdateTimer >= 0.1f)
        {
            _pathUpdateTimer -= 0.1f;
            if (_target != null && _agent != null && _agent.isOnNavMesh)
            {
                _agent.SetDestination(_target.position);
            }
        }
        if (_agent != null && _agent.isOnNavMesh)
        {
            Vector3 velocity = _agent.velocity;
            velocity.y = 0f;
            
            if (velocity.sqrMagnitude > 0.01f)
            {
                // плавный поворот при движении
                Quaternion moveRot = Quaternion.LookRotation(velocity.normalized);
                _transform.rotation = Quaternion.RotateTowards(_transform.rotation, moveRot, _turnRotationSpeed * deltaTime);
            }
            else if (_agent.remainingDistance <= _agent.stoppingDistance && _target != null)
            {
                Vector3 dirToTarget = _target.position - _transform.position;
                dirToTarget.y = 0f;
                
                if (dirToTarget.sqrMagnitude > 0.001f)
                {
                    float angle = Vector3.Angle(_transform.forward, dirToTarget.normalized);
                    if (angle > _turnThresholdAngle)
                    {
                        Quaternion targetRot = Quaternion.LookRotation(dirToTarget.normalized);
                        _transform.rotation = Quaternion.RotateTowards(_transform.rotation, targetRot, _turnRotationSpeed * deltaTime);
                    }
                }
            }
        }
        
        // установка таргетов на заданной дистанции в направлении цели
        if (_target != null && _transform != null && _aimTargetDistance > 0f)
        {
            Vector3 dir = _target.position - _transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.001f)
            {
                dir.Normalize();
                Vector3 aimPos = _transform.position + dir * _aimTargetDistance;
                aimPos.y = _transform.position.y + _aimTargetHeightOffset;
                if (_spineTarget != null)
                    _spineTarget.position = aimPos;
                if (_headTarget != null)
                    _headTarget.position = aimPos;
                if (_rightHandTarget != null)
                    _rightHandTarget.position = aimPos;
            }
        }
        
        if (_leftHandTarget != null && _weaponView != null && _weaponView.SecondHandPoint != null)
        {
            _leftHandTarget.position = _weaponView.SecondHandPoint.position;
            _leftHandTarget.rotation = _weaponView.SecondHandPoint.rotation;
        }
    }

    public Vector3 GetNextPosition()
    {
        // NavMeshAgent сам двигает Transform
        return _transform != null ? _transform.position : Vector3.zero;
    }

    public bool CanAttack()
    {
        if (_target == null || _transform == null || _enemy == null) return false;

        // NavMeshAgent сам останавливается на stoppingDistance, которая равна AttackRange
        // Проверяем, что агент достаточно близко (в пределах stoppingDistance + небольшой допуск)
        // и кулдаун атаки прошел.
        if (_agent != null && _agent.isOnNavMesh)
        {
            // Если агент активен и на навмеше, используем его remainingDistance
            // remainingDistance может быть не 0, даже если агент достиг цели,
            // если stoppingDistance > 0. Поэтому проверяем <= stoppingDistance.
            return _agent.remainingDistance <= _agent.stoppingDistance && 
                   _lastAttackTime >= _enemy.AttackCooldown;
        }
        
        // Fallback, если агент не настроен или не на NavMesh
        float sqrDistance = (_target.position - _transform.position).sqrMagnitude;
        float sqrAttackRange = _enemy.AttackRange * _enemy.AttackRange + 0.09f;
        return sqrDistance <= sqrAttackRange && _lastAttackTime >= _enemy.AttackCooldown;
    }

    public void ResetAttackCooldown()
    {
        _lastAttackTime = 0f;
    }
} 