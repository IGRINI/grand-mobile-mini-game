using System;
using UnityEngine;
using Zenject;
using System.Collections.Generic;

public class CarController : IFixedTickable, IDisposable
{
    private readonly IReadOnlyList<Transform> _wheels;
    private readonly IReadOnlyList<Transform> _steerPivots;
    private readonly float _maxSteerAngle;
    private readonly ICarModel _model;
    private readonly ICarView _view;
    private readonly IInputService _input;
    private float _currentLeanAngle;
    private float _leanVelocity;
    private float _currentPitchAngle;
    private float _pitchVelocity;
    private readonly IHealthService _healthService;
    private IHealth _health;
    private float _currentWheelAngle;
    private float _currentSteerAngle;
    private readonly ICollisionDetector _collisionDetector;
    private readonly CarCollisionEffect _collisionEffect;
    private readonly ICarDamageDealer _damageDealer;
    private readonly IEnemyDetector _enemyDetector;

    public CarController(ICarModel model, ICarView view, IInputService input, IHealthService healthService, ICollisionDetector collisionDetector, IEnemyDetector enemyDetector, ICarDamageDealer damageDealer, float initialHealth)
    {
        _healthService = healthService;
        _model = model;
        _view = view;
        _input = input;
        _wheels = view.Wheels;
        _steerPivots = view.SteerPivots;
        _maxSteerAngle = view.MaxSteerAngle;
        _health = new Health(initialHealth);
        _healthService.RegisterEntity(this, _health);
        _collisionDetector = collisionDetector;
        _collisionEffect = new CarCollisionEffect(view);
        _damageDealer = damageDealer;
        _enemyDetector = enemyDetector;
    }

    public void FixedTick()
    {
        UpdateCar(Time.fixedDeltaTime);
    }

    private void UpdateCar(float deltaTime)
    {
        var moveDirection = _input.MoveDirection;
        if (_view.SteeringWheel != null)
        {
            float targetAngle = moveDirection.x * _view.MaxSteeringWheelAngle;
            _currentWheelAngle = Mathf.MoveTowards(_currentWheelAngle, targetAngle, _view.SteeringWheelSmoothSpeed * deltaTime);
            _view.SteeringWheel.localRotation = Quaternion.Euler(0f, 0f, -_currentWheelAngle);
        }
        float targetLeanAngle = 0f;
        float targetPitchAngle = 0f;
        
        float previousSpeed = _model.CurrentSpeed;

        if (_input.Gas)
        {
            _model.CurrentSpeed = Mathf.Clamp(_model.CurrentSpeed + _model.Acceleration * deltaTime, -_model.MaxReverseSpeed, _model.MaxSpeed);
        }
        else if (_input.Brake)
        {
            _model.CurrentSpeed = Mathf.Clamp(_model.CurrentSpeed - _model.BrakeForce * deltaTime, -_model.MaxReverseSpeed, _model.MaxSpeed);
        }
        else
        {
            _model.CurrentSpeed = Mathf.MoveTowards(_model.CurrentSpeed, 0, _model.BrakeForce * deltaTime);
        }

        if (moveDirection.sqrMagnitude > 0.01f && Mathf.Abs(_model.CurrentSpeed) >= _model.MinSpeedToTurn)
        {
            Vector3 inputDir = new Vector3(moveDirection.x, 0, moveDirection.y).normalized;
            if (_model.CurrentSpeed < 0f)
            {
                inputDir.x = -inputDir.x;
                inputDir.z = -inputDir.z;
            }
            Vector3 targetDirection = inputDir;
            Vector3 currentForward = _view.Transform.forward;
            
            float angleDifference = Vector3.SignedAngle(currentForward, targetDirection, Vector3.up);
            
            float speedFactor = Mathf.Abs(_model.CurrentSpeed) / _model.MaxSpeed;
            float currentTurnRate = _model.MaxTurnRate * speedFactor * _model.TurnRateSpeedFactor;
            
            float maxTurnThisFrame = currentTurnRate * deltaTime;
            float actualTurn = Mathf.Clamp(angleDifference, -maxTurnThisFrame, maxTurnThisFrame);
            Vector3 rearAxisPos = _view.RearAxisCenter.position;
            _view.Transform.RotateAround(rearAxisPos, Vector3.up, actualTurn);
            
            float steerAngleRaw = Mathf.Clamp(angleDifference / 45f, -1f, 1f) * _maxSteerAngle;
            if (_model.CurrentSpeed < 0f)
                steerAngleRaw = -steerAngleRaw;
            if (_steerPivots != null)
            {
                _currentSteerAngle = Mathf.MoveTowards(_currentSteerAngle, steerAngleRaw, _view.SteerPivotSmoothSpeed * deltaTime);
                foreach (var pivot in _steerPivots)
                {
                    pivot.localRotation = Quaternion.Euler(0, _currentSteerAngle, 0);
                }
            }

            targetLeanAngle = Mathf.Clamp(actualTurn / maxTurnThisFrame, -1f, 1f) * _view.MaxLeanAngle;
            if (_model.CurrentSpeed < 0f)
                targetLeanAngle = -targetLeanAngle;
        }
        else
        {
            if (_steerPivots != null)
            {
                _currentSteerAngle = Mathf.MoveTowards(_currentSteerAngle, 0f, _view.SteerPivotSmoothSpeed * deltaTime);
                foreach (var pivot in _steerPivots)
                {
                    pivot.localRotation = Quaternion.Euler(0, _currentSteerAngle, 0);
                }
            }
        }

        Vector3 offset = _view.Transform.rotation * _view.CollisionCenterOffset;
        Vector3 currentCenter = _view.Transform.position + offset;
        Vector3 targetCenter = currentCenter + _view.Transform.forward * _model.CurrentSpeed * deltaTime;
        
        Vector3 pushOutDirection = _collisionDetector.GetPushOutDirection(currentCenter, _view.CollisionSize, _view.Transform.rotation);
        if (pushOutDirection != Vector3.zero)
        {
            float pushForce = 2f * Time.fixedDeltaTime;
            _view.Transform.position += pushOutDirection * pushForce;
            currentCenter = _view.Transform.position + offset;
            targetCenter = currentCenter + _view.Transform.forward * _model.CurrentSpeed * deltaTime;
        }
        
        bool hasObstacleCollision = false;
        if (Mathf.Abs(_model.CurrentSpeed) > 0.01f)
        {
            hasObstacleCollision = _collisionDetector.CheckRectangleCollision(currentCenter, targetCenter, _view.CollisionSize, _view.Transform.rotation);
        }
        
        var collidingEnemy = _enemyDetector.GetCollidingEnemy(targetCenter, _view.CollisionSize, _view.Transform.rotation);
        var collidingHittableObstacle = _collisionDetector.GetCollidingHittableObstacle(targetCenter, _view.CollisionSize, _view.Transform.rotation);
        
        if (collidingHittableObstacle != null)
        {
            Debug.Log($"Found collidingHittableObstacle: {collidingHittableObstacle.GetType().Name}, CanBeHit: {collidingHittableObstacle.CanBeHit}");
        }
        
        if (collidingEnemy != null && collidingEnemy.CanBeHit)
        {
            Vector3 enemyPos = ((MonoBehaviour)collidingEnemy).transform.position;
            Vector3 hitDirection = (enemyPos - _view.Transform.position).normalized;
            
            collidingEnemy.OnHit(hitDirection, Mathf.Abs(_model.CurrentSpeed));
            _damageDealer.DealDamage(collidingEnemy, _model.CurrentSpeed, _model.MaxSpeed, _model.BaseCollisionDamage);
            
            _model.CurrentSpeed = Mathf.Max(0f, Mathf.Abs(_model.CurrentSpeed) - _model.SpeedReductionPerEnemy) * Mathf.Sign(_model.CurrentSpeed);
            
            _view.Transform.position = targetCenter - offset;
        }
        else if (collidingHittableObstacle != null && collidingHittableObstacle.CanBeHit)
        {
            Debug.Log($"Hitting hittable obstacle! Speed: {Mathf.Abs(_model.CurrentSpeed)}");
            
            Vector3 obstaclePos;
            if (collidingHittableObstacle is HittableCircleObstacle hittableCircle)
            {
                obstaclePos = ((MonoBehaviour)hittableCircle._hittableComponent).transform.position;
            }
            else
            {
                obstaclePos = ((MonoBehaviour)collidingHittableObstacle).transform.position;
            }
            
            Vector3 hitDirection = (obstaclePos - _view.Transform.position).normalized;
            
            collidingHittableObstacle.OnHit(hitDirection, Mathf.Abs(_model.CurrentSpeed));
            
            _model.CurrentSpeed = Mathf.Max(0f, Mathf.Abs(_model.CurrentSpeed) - _model.SpeedReductionPerEnemy) * Mathf.Sign(_model.CurrentSpeed);
            
            _view.Transform.position = targetCenter - offset;
        }
        else if (hasObstacleCollision)
        {
            var collidingObstacle = _collisionDetector.GetCollidingObstacleRectangle(targetCenter, _view.CollisionSize, _view.Transform.rotation);
            Vector3 collisionNormal = _collisionDetector.GetRectangleCollisionNormal(targetCenter, _view.CollisionSize, _view.Transform.rotation);
            
            float impactSpeed = Mathf.Abs(_model.CurrentSpeed);
            _collisionEffect.ApplyCollisionEffect(collisionNormal, _model.CurrentSpeed, _model.MaxSpeed);
            
            if (collidingObstacle is IDamageable damageable && damageable.CanTakeDamage)
            {
                float speedFactor = Mathf.Clamp01(impactSpeed / _model.MaxSpeed);
                float buildingDamage = _model.BaseCollisionDamage * speedFactor;
                damageable.TakeDamage(buildingDamage);
                
                float selfDamage = buildingDamage * _model.SelfDamageMultiplier;
                _health.TakeDamage(selfDamage);
            }
            
            _model.CurrentSpeed = 0f;
        }
        else
        {
            _view.Transform.position = targetCenter - offset;
        }

        _collisionEffect.UpdateCollisionEffect(deltaTime);
        
        float speedChange = (_model.CurrentSpeed - previousSpeed) / deltaTime;
        float basePitch = Mathf.Sign(_model.CurrentSpeed) * _view.BasePitchAngle;
        targetPitchAngle = basePitch - speedChange * _view.AccelerationPitchFactor + _collisionEffect.GetCollisionPitch();
        targetPitchAngle = Mathf.Clamp(targetPitchAngle, -_view.MaxPitchAngle, _view.MaxPitchAngle);

        if (_wheels != null)
        {
            float spinDelta = _model.CurrentSpeed * deltaTime * 360f;
            foreach (var wheel in _wheels)
            {
                wheel.Rotate(_view.WheelSpinAxis, spinDelta, Space.Self);
            }
        }

        if (_view.CarBody != null)
        {
            _leanVelocity += (targetLeanAngle - _currentLeanAngle) * _view.LeanSpring * deltaTime;
            _leanVelocity *= 1f - _view.LeanDamping * deltaTime;
            _currentLeanAngle += _leanVelocity * deltaTime;
            
            _pitchVelocity += (targetPitchAngle - _currentPitchAngle) * _view.PitchSpring * deltaTime;
            _pitchVelocity *= 1f - _view.PitchDamping * deltaTime;
            _currentPitchAngle += _pitchVelocity * deltaTime;
            _currentPitchAngle = Mathf.Clamp(_currentPitchAngle, -_view.MaxPitchAngle, _view.MaxPitchAngle);
            
            _view.CarBody.localRotation = Quaternion.Euler(_currentPitchAngle, 0f, _currentLeanAngle);
        }
    }

    public void Dispose()
    {
        _healthService.UnregisterEntity(this);
    }
} 