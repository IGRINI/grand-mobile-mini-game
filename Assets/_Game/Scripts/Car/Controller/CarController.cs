using System;
using UnityEngine;
using Zenject;
using System.Collections.Generic;

public class CarController : ITickable, IDisposable
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
    private readonly float _initialHealth;

    public CarController(ICarModel model, ICarView view, IInputService input, IHealthService healthService, float initialHealth)
    {
        _healthService = healthService;
        _initialHealth = initialHealth;
        _model = model;
        _view = view;
        _input = input;
        _wheels = view.Wheels;
        _steerPivots = view.SteerPivots;
        _maxSteerAngle = view.MaxSteerAngle;
        _health = new Health(initialHealth);
        _healthService.RegisterEntity(this, _health);
    }

    public void Tick()
    {
        UpdateCar(Time.deltaTime);
    }

    private void UpdateCar(float deltaTime)
    {
        var moveDirection = _input.MoveDirection;
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
            Vector3 targetDirection = new Vector3(moveDirection.x, 0, moveDirection.y).normalized;
            Vector3 currentForward = _view.Transform.forward;
            
            float angleDifference = Vector3.SignedAngle(currentForward, targetDirection, Vector3.up);
            
            float speedFactor = Mathf.Abs(_model.CurrentSpeed) / _model.MaxSpeed;
            float currentTurnRate = _model.MaxTurnRate * speedFactor * _model.TurnRateSpeedFactor;
            
            float maxTurnThisFrame = currentTurnRate * deltaTime;
            float actualTurn = Mathf.Clamp(angleDifference, -maxTurnThisFrame, maxTurnThisFrame);
            
            Vector3 rearAxisPos = _view.RearAxisCenter.position;
            _view.Transform.RotateAround(rearAxisPos, Vector3.up, actualTurn);
            
            float steerAngle = Mathf.Clamp(angleDifference / 45f, -1f, 1f) * _maxSteerAngle;
            if (_model.CurrentSpeed < 0f)
                steerAngle = -steerAngle;
            if (_steerPivots != null)
            {
                foreach (var pivot in _steerPivots)
                {
                    pivot.localRotation = Quaternion.Euler(0, steerAngle, 0);
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
                foreach (var pivot in _steerPivots)
                {
                    pivot.localRotation = Quaternion.Slerp(pivot.localRotation, Quaternion.identity, deltaTime * 5f);
                }
            }
        }

        _view.Transform.position += _view.Transform.forward * _model.CurrentSpeed * deltaTime;

        float speedChange = (_model.CurrentSpeed - previousSpeed) / deltaTime;
        float basePitch = Mathf.Sign(_model.CurrentSpeed) * _view.BasePitchAngle;
        targetPitchAngle = basePitch - speedChange * _view.AccelerationPitchFactor;
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