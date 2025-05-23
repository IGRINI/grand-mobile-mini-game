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
        var steering = _input.Steering;
        // высчитываем угол рулевых колес: между текущим направлением и направлением джойстика
        float steerAngle = 0f;
        if (steering.sqrMagnitude > 0f)
        {
            var desiredDir = new Vector3(steering.x, 0, steering.y).normalized;
            // угол между впередом машины и направлением джойстика
            steerAngle = Vector3.SignedAngle(_view.Transform.forward, desiredDir, Vector3.up);
            steerAngle = Mathf.Clamp(steerAngle, -_maxSteerAngle, _maxSteerAngle);
            // поворачиваем машину в сторону джойстика
            var targetRot = Quaternion.LookRotation(desiredDir);
            _view.Transform.rotation = Quaternion.RotateTowards(_view.Transform.rotation, targetRot, _model.TurnSpeed * deltaTime);
        }
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
        _view.Transform.position += _view.Transform.forward * _model.CurrentSpeed * deltaTime;

        // вращение колес
        if (_wheels != null)
        {
            float spinDelta = _model.CurrentSpeed * deltaTime * 360f;
            foreach (var wheel in _wheels)
            {
                wheel.Rotate(_view.WheelSpinAxis, spinDelta, Space.Self);
            }
        }
        // поворот рулевых частей по рассчитанному углу
        if (_steerPivots != null)
        {
            foreach (var pivot in _steerPivots)
            {
                pivot.localRotation = Quaternion.Euler(0, steerAngle, 0);
            }
        }
    }

    public void Dispose()
    {
        // нет ресурсов для очистки
    }
} 