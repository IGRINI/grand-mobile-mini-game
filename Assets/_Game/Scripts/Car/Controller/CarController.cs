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

    public CarController(ICarModel model, ICarView view, IInputService input)
    {
        _model = model;
        _view = view;
        _input = input;
        _wheels = view.Wheels;
        _steerPivots = view.SteerPivots;
        _maxSteerAngle = view.MaxSteerAngle;
    }

    public void Tick()
    {
        UpdateCar(Time.deltaTime);
    }

    private void UpdateCar(float deltaTime)
    {
        var moveDirection = _input.MoveDirection;
        
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
            if (_steerPivots != null)
            {
                foreach (var pivot in _steerPivots)
                {
                    pivot.localRotation = Quaternion.Euler(0, steerAngle, 0);
                }
            }
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

        if (_wheels != null)
        {
            float spinDelta = _model.CurrentSpeed * deltaTime * 360f;
            foreach (var wheel in _wheels)
            {
                wheel.Rotate(_view.WheelSpinAxis, spinDelta, Space.Self);
            }
        }
    }

    public void Dispose()
    {
        // нет ресурсов для очистки
    }
} 